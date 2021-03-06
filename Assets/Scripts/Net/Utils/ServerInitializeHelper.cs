﻿using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Client;
using Config;
using Core;
using Net.Core;
using UnityEngine;
using Utils;

namespace Net.Utils
{
    public class ServerInitializeHelper: Singleton<ServerInitializeHelper>
    {
        //TODO: Избавиться от этого - дублирование SpaceShipConfig
        [Serializable]
        public class SpaceShipDto
        {
            public float maxFuel;
            public string shipId;
            public float maxAngleSpeed;
            public float maxSpeed;
            public int maxHp;
            public int currentHp;
            public bool isDockable;
            public Vector3 position = Vector3.one;
            public Quaternion rotation = Quaternion.identity;
            public string prefabName;

            public SpaceShipDto(SpaceShipConfig config)
            {
                maxFuel = config.maxFuel;
                shipId = config.shipId;
                maxAngleSpeed = config.maxAngleSpeed;
                maxSpeed = config.maxSpeed;
                maxHp = config.maxHp;
                currentHp = config.currentHp;
                isDockable = config.isDockable;
                position = config.position;
                rotation = config.rotation;
                prefabName = config.prefabName;
            }
        }
        
        [Serializable]
        public class SpaceShipsWrapper
        {
            [SerializeField]
            public SpaceShipDto[] spaceShipConfigs;
        }

        private BinaryFormatter _binaryFormatter;
        private SpaceShipConfig[] _shipConfigs;
        
        private new void Awake()
        {
            base.Awake();
        }

        public IEnumerator InitServer()
        {
            try
            {
                Camera.main.gameObject.GetComponent<CameraMotion>().SwitchFollowMode();
                
                _shipConfigs = JsonUtility.FromJson<SpaceShipsWrapper>(File.ReadAllText(Constants.PathToShips))
                    .spaceShipConfigs.Select(x =>
                    {
                        var temp = ScriptableObject.CreateInstance<SpaceShipConfig>();
                        temp.maxFuel = x.maxFuel;
                        temp.shipId = x.shipId;
                        temp.maxAngleSpeed = x.maxAngleSpeed;
                        temp.maxSpeed = x.maxSpeed;
                        temp.maxHp = x.maxHp;
                        temp.currentHp = x.currentHp;
                        temp.isDockable = x.isDockable;
                        temp.position = x.position;
                        temp.rotation = x.rotation;
                        temp.prefabName = x.prefabName;
                        return temp;
                    }).ToArray();
            }
            catch (FileNotFoundException ex)
            {
                Debug.unityLogger.Log("ERROR");
                _shipConfigs = Resources.LoadAll<SpaceShipConfig>(Constants.PathToShipsObjects);
            }
            catch (SerializationException ex)
            {
                Debug.unityLogger.Log("ERROR");
                _shipConfigs = Resources.LoadAll<SpaceShipConfig>(Constants.PathToShipsObjects);
            }
            
            foreach (var spaceShipConfig in _shipConfigs)
            {
                try
                {
                    var playerScript = InstantiateHelper.InstantiateServerShip(spaceShipConfig);
                }
                catch(Exception ex)
                {
                    Debug.unityLogger.LogException(ex);
                }
                //TODO: Init fields from config;
                yield return null;
            }

            yield return StartCoroutine(
                Importer.AddAsteroidsOnScene(Importer.ImportAsteroids(Constants.PathToAsteroids)));
            MainServerLoop.instance.indicator.tintColor = Color.green;
            NetEventStorage.GetInstance().worldInit.Invoke(0);
        }

        public void SaveServer()
        {
            foreach (var spaceShipConfig in _shipConfigs)
            {
                var ship = GameObject.Find(
                    $"{spaceShipConfig.prefabName}{Constants.Separator}{spaceShipConfig.shipId}");
                if (ship is null) continue;
                spaceShipConfig.rotation = ship.transform.rotation;
                spaceShipConfig.position = ship.transform.position;
                //TODO: Save other fields;
            }
            
            File.WriteAllText(Constants.PathToShips, JsonUtility.ToJson(new SpaceShipsWrapper()
            {
                spaceShipConfigs = _shipConfigs.Select(x=> new SpaceShipDto(x)).ToArray()
            }));
        }
    }
}
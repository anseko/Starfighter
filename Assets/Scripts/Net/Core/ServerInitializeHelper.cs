using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Client.Core;
using Core;
using Core.Models;
using ScriptableObjects;
using UnityEngine;

namespace Net.Core
{
    public class ServerInitializeHelper: MonoBehaviour
    {
        [Serializable]
        public class SpaceShipsWrapper
        {
            [SerializeField]
            public SpaceShipDto[] spaceShipConfigs;
        }

        [Serializable]
        public class SpaceUnitWrapper
        {
            [SerializeField]
            public SpaceUnitDto[] spaceUnitConfigs;
        }
        
        [Serializable]
        public class DangerZoneWrapper
        {
            [SerializeField] 
            public DangerZoneDto[] dangerZoneConfigs;
        }
        
        private BinaryFormatter _binaryFormatter;
        private SpaceShipConfig[] _shipConfigs;
        private SpaceUnitConfig[] _unitConfigs;
        private DangerZoneConfig[] _dangerZoneConfigs;

        private void InitShips()
        {
            try
            {
                _shipConfigs = JsonUtility.FromJson<SpaceShipsWrapper>(File.ReadAllText(Constants.PathToShips))
                    .spaceShipConfigs.Select(x =>
                    {
                        var temp = ScriptableObject.CreateInstance<SpaceShipConfig>();
                        temp.maxStress = x.maxStress;
                        temp.currentStress = x.currentStress;
                        temp.shipId = x.shipId;
                        temp.maxAngleSpeed = x.maxAngleSpeed;
                        temp.maxSpeed = x.maxSpeed;
                        temp.maxHp = x.maxHp;
                        temp.currentHp = x.currentHp;
                        temp.isDockable = x.isDockable;
                        temp.position = x.position;
                        temp.rotation = x.rotation;
                        temp.prefabName = x.prefabName;
                        temp.shipState = x.shipState;
                        return temp;
                    }).ToArray();
            }
            catch (FileNotFoundException ex)
            {
                Debug.unityLogger.Log($"ERROR: {ex.Message}");
                _shipConfigs = Resources.LoadAll<SpaceShipConfig>(Constants.PathToShipsObjects);
            }
            catch (SerializationException ex)
            {
                Debug.unityLogger.Log($"ERROR {ex.Message}");
                _shipConfigs = Resources.LoadAll<SpaceShipConfig>(Constants.PathToShipsObjects);
            }
        }

        private void InitUnits()
        {
            try
            {
                _unitConfigs = JsonUtility.FromJson<SpaceUnitWrapper>(File.ReadAllText(Constants.PathToUnits))
                    .spaceUnitConfigs.Select(x =>
                    {
                        var temp = ScriptableObject.CreateInstance<SpaceUnitConfig>();
                        temp.maxAngleSpeed = x.maxAngleSpeed;
                        temp.maxSpeed = x.maxSpeed;
                        temp.maxHp = x.maxHp;
                        temp.currentHp = x.currentHp;
                        temp.isDockable = x.isDockable;
                        temp.position = x.position;
                        temp.rotation = x.rotation;
                        temp.prefabName = x.prefabName;
                        temp.id = x.id;
                        return temp;
                    }).ToArray();
            }
            catch (FileNotFoundException ex)
            {
                Debug.unityLogger.Log($"ERROR: {ex.Message}");
                _unitConfigs = Resources.LoadAll<SpaceUnitConfig>(Constants.PathToUnitsObjects);
            }
            catch (SerializationException ex)
            {
                Debug.unityLogger.Log($"ERROR {ex.Message}");
                _unitConfigs = Resources.LoadAll<SpaceUnitConfig>(Constants.PathToUnitsObjects);
            }
        }

        private void InitDangerZones()
        {
            _dangerZoneConfigs = Resources.LoadAll<DangerZoneConfig>(Constants.PathToDangerZones);
            return;
            
            try
            {
                _dangerZoneConfigs = JsonUtility.FromJson<DangerZoneWrapper>(File.ReadAllText(Constants.PathToDangerZones))
                    .dangerZoneConfigs.Select(x =>
                    {
                        var temp = ScriptableObject.CreateInstance<DangerZoneConfig>();
                        temp.Center = x.center;
                        temp.Color = x.color;
                        temp.Damage = x.damage;
                        temp.Radius = x.radius;
                        return temp;
                    }).ToArray();
            }
            catch (FileNotFoundException ex)
            {
                Debug.unityLogger.Log($"ERROR: {ex.Message}");
                _dangerZoneConfigs = Resources.LoadAll<DangerZoneConfig>(Constants.PathToDangerZones);
            }
            catch (SerializationException ex)
            {
                Debug.unityLogger.Log($"ERROR {ex.Message}");
                _dangerZoneConfigs = Resources.LoadAll<DangerZoneConfig>(Constants.PathToDangerZones);
            }
        }
        
        public IEnumerator InitServer()
        {
            InitShips();
            InitUnits();
            InitDangerZones();
            
            foreach (var dangerZone in _dangerZoneConfigs)
            {
                try
                {
                    InstantiateHelper.InstantiateDangerZone(dangerZone);
                }
                catch(Exception ex)
                {
                    Debug.unityLogger.LogException(ex);
                }
                yield return null;
            }
            
            foreach (var unitConfig in _unitConfigs)
            {
                try
                {
                    InstantiateHelper.InstantiateObject(unitConfig);
                }
                catch(Exception ex)
                {
                    Debug.unityLogger.LogException(ex);
                }
                yield return null;
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
                yield return null;
            }
            
            yield return StartCoroutine(
                Importer.AddAsteroidsOnScene(Importer.ImportAsteroids(Constants.PathToAsteroids)));
            gameObject.GetComponent<MainServerLoop>().indicator.color = Color.green;
            NetEventStorage.GetInstance().WorldInit.Invoke(0);
        }

        public void SaveServer()
        {
            foreach (var spaceShipConfig in _shipConfigs)
            {
                var ship = GameObject.Find(
                    $"{spaceShipConfig.prefabName}{Constants.Separator}{spaceShipConfig.shipId}");
                if (ship is null) continue;
                var ps = ship.GetComponent<PlayerScript>();
                spaceShipConfig.rotation = ship.transform.rotation;
                spaceShipConfig.position = ship.transform.position;
                spaceShipConfig.currentStress = ps.currentStress.Value;
                spaceShipConfig.currentHp = ps.currentHp.Value;
                //Save other fields;
                spaceShipConfig.shipState = ship.GetComponent<PlayerScript>().GetState();
                Debug.unityLogger.Log($"Saving ships {spaceShipConfig.prefabName} state {spaceShipConfig.shipState}");
            }
            
            File.WriteAllText(Constants.PathToShips, JsonUtility.ToJson(new SpaceShipsWrapper()
            {
                spaceShipConfigs = _shipConfigs.Select(x=> new SpaceShipDto(x)).ToArray()
            }));
            
            foreach (var unitConfig in _unitConfigs)
            {
                var ship = GameObject.Find(
                    $"{unitConfig.prefabName}{Constants.Separator}{unitConfig.id}");
                if (ship is null) continue;
                unitConfig.rotation = ship.transform.rotation;
                unitConfig.position = ship.transform.position;
                
                //Save other fields;
            }
            
            File.WriteAllText(Constants.PathToUnits, JsonUtility.ToJson(new SpaceUnitWrapper()
            {
                spaceUnitConfigs = _unitConfigs.Select(x=> new SpaceUnitDto(x)).ToArray()
            }));
        }
    }
}
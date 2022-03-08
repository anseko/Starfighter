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
            public SpaceUnitDto[] spaceShipConfigs;
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


        private void InitShips(out SpaceShipConfig[] shipConfigs)
        {
            try
            {
                shipConfigs = JsonUtility.FromJson<SpaceShipsWrapper>(File.ReadAllText(Constants.PathToShips))
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
                        temp.baseColor = x.baseColor;
                        temp.acceleration = x.acceleration;
                        temp.radarRange = x.radarRange;
                        temp.accelerationCoefficient = x.accelerationCoefficient;
                        temp.physResistanceCoefficient = x.physResistanceCoefficient;
                        temp.radResistanceCoefficient = x.radResistanceCoefficient;
                        temp.radarRangeCoefficient = x.radarRangeCoefficient;
                        return temp;
                    }).ToArray();
            }
            catch (FileNotFoundException ex)
            {
                Debug.unityLogger.Log($"ERROR: {ex.Message}");
                shipConfigs = Resources.LoadAll<SpaceShipConfig>(Constants.PathToShipsObjects);
            }
            catch (SerializationException ex)
            {
                Debug.unityLogger.Log($"ERROR {ex.Message}");
                shipConfigs = Resources.LoadAll<SpaceShipConfig>(Constants.PathToShipsObjects);
            }
        }

        private void InitUnits(out SpaceUnitConfig[] unitConfigs)
        {
            try
            {
                unitConfigs = JsonUtility.FromJson<SpaceUnitWrapper>(File.ReadAllText(Constants.PathToUnits))
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
                unitConfigs = Resources.LoadAll<SpaceUnitConfig>(Constants.PathToUnitsObjects);
            }
            catch (SerializationException ex)
            {
                Debug.unityLogger.Log($"ERROR {ex.Message}");
                unitConfigs = Resources.LoadAll<SpaceUnitConfig>(Constants.PathToUnitsObjects);
            }
        }

        private void InitDangerZones(out DangerZoneConfig[] dangerZoneConfigs)
        {
            try
            {
                dangerZoneConfigs = JsonUtility
                    .FromJson<DangerZoneWrapper>(File.ReadAllText(Constants.PathToDangerZones))
                    .dangerZoneConfigs.Select(x =>
                    {
                        var temp = ScriptableObject.CreateInstance<DangerZoneConfig>();
                        temp.center = x.center;
                        temp.color = x.color;
                        temp.stressDamage = x.stressDamage;
                        temp.hpDamage = x.hpDamage;
                        temp.radius = x.radius;
                        temp.id = x.id;
                        temp.type = x.type;
                        return temp;
                    }).ToArray();
            }
            catch (FileNotFoundException ex)
            {
                Debug.unityLogger.Log($"ERROR: {ex.Message}");
                dangerZoneConfigs = Resources.LoadAll<DangerZoneConfig>(Constants.PathToDangerZonesObjects);
            }
            catch (SerializationException ex)
            {
                Debug.unityLogger.Log($"ERROR {ex.Message}");
                dangerZoneConfigs = Resources.LoadAll<DangerZoneConfig>(Constants.PathToDangerZonesObjects);
            }
        }
        
        public IEnumerator InitServer()
        {
            InitShips(out var shipConfigs);
            InitDangerZones(out var dangerZoneConfigs);
            InitUnits(out var unitConfigs);

            foreach (var dangerZone in dangerZoneConfigs)
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
            
            foreach (var unitConfig in unitConfigs)
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
            
            foreach (var spaceShipConfig in shipConfigs)
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
            
            gameObject.GetComponent<MainServerLoop>().indicator.color = Color.green;
            NetEventStorage.GetInstance().WorldInit.Invoke(0);
        }

        public void SaveServer()
        {
            var shipsConfigs = FindObjectsOfType<PlayerScript>()
                .Select(x=> x.NetworkUnitConfig.Export())
                .ToList();
            
            foreach (var shipConfig in shipsConfigs)
            {
                var ship = GameObject.Find(
                    $"{shipConfig.prefabName}{Constants.Separator}{shipConfig.shipId}");
                if (ship is null) continue;
                shipConfig.rotation = ship.transform.rotation;
                shipConfig.position = ship.transform.position;
                Debug.unityLogger.Log($"Saving ships {shipConfig.prefabName} state {shipConfig.shipState}");
            }
            
            File.WriteAllText(Constants.PathToShips, JsonUtility.ToJson(new SpaceShipsWrapper()
            {
                spaceShipConfigs = shipsConfigs.ToArray()//_shipConfigs.Select(x=> new SpaceUnitDto(x)).ToArray()
            }));
            
            var configs = FindObjectsOfType<UnitScript>()
                .Where(x=> !(x is PlayerScript))
                .Select(x=> x.NetworkUnitConfig.Export())
                .ToList();
            
            foreach (var unitConfig in configs)
            {
                var unit = GameObject.Find(
                    $"{unitConfig.prefabName}{Constants.Separator}{unitConfig.id}");
                if (unit is null) continue;
                unitConfig.rotation = unit.transform.rotation;
                unitConfig.position = unit.transform.position;
            }
            
            File.WriteAllText(Constants.PathToUnits, JsonUtility.ToJson(new SpaceUnitWrapper()
            {
                spaceUnitConfigs = configs.ToArray()
            }));

            var zones = FindObjectsOfType<DangerZone>()
                .Select(x => x.Export())
                .ToList();
            
            File.WriteAllText(Constants.PathToDangerZones, JsonUtility.ToJson(new DangerZoneWrapper()
            {
                dangerZoneConfigs = zones.ToArray()
            }));
        }
    }
}
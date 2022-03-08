using System;
using Client.Core;
using Core;
using Core.Models;
using ScriptableObjects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Net.Core
{
    public static class InstantiateHelper
    {
        public static PlayerScript InstantiateServerShip(SpaceShipConfig ship)
        {
            var shipGO = GameObject.Find($"{ship.prefabName}{Constants.Separator}{ship.shipId}");
            
            if(shipGO != null)
            {
                return shipGO.GetComponent<PlayerScript>();
            }
            
            var shipPrefab = Resources.Load(Constants.PathToShipsPrefabs + ship.prefabName);

            var shipInstance = Object.Instantiate(shipPrefab, position: ship.position,
                                rotation: ship.rotation) as GameObject;
             
            shipInstance.name = ship.prefabName + Constants.Separator + ship.shipId;
            shipInstance.tag = Constants.DynamicTag;
            
            var playerScript = shipInstance.GetComponent<PlayerScript>() ?? shipInstance.AddComponent<PlayerScript>();
            playerScript.NetworkUnitConfig.Init(new SpaceUnitDto(ship));

            shipInstance.SetActive(true);
            return playerScript;
        }

        public static UnitScript InstantiateObject(SpaceUnitConfig worldObject)
        {
            worldObject.id = worldObject.id == Guid.Empty ? Guid.NewGuid() : worldObject.id;
            var prefabName = worldObject.prefabName;
            var goToInstantiate = Resources.Load(Constants.PathToPrefabs + prefabName);
            var instance =
                Object.Instantiate(goToInstantiate, worldObject.position, worldObject.rotation) as
                    GameObject;
            instance.name = worldObject.prefabName + Constants.Separator + worldObject.id;

            var unitScript = instance.GetComponent<UnitScript>() ?? instance.AddComponent<UnitScript>();
            unitScript.NetworkUnitConfig.Init(new SpaceUnitDto(worldObject));
            instance.SetActive(true);
            return unitScript;
        }

        public static DangerZone InstantiateDangerZone(DangerZoneConfig dangerZoneConfig)
        {
            dangerZoneConfig.id = dangerZoneConfig.id == Guid.Empty ? Guid.NewGuid() : dangerZoneConfig.id;
            var goToInstantiate = Resources.Load(Constants.PathToPrefabs + "DangerZone") as GameObject;
            var instance = Object.Instantiate(goToInstantiate, dangerZoneConfig.center, Quaternion.Euler(90, 0, 0));
            var dangerZone = instance.GetComponent<DangerZone>();
            dangerZone.Guid = dangerZoneConfig.id;
            dangerZone.zoneColor.Value = dangerZoneConfig.color;
            dangerZone.zoneStressDamage.Value = dangerZoneConfig.stressDamage;
            dangerZone.zoneHpDamage.Value = dangerZoneConfig.hpDamage;
            dangerZone.zoneRadius.Value = dangerZoneConfig.radius;
            dangerZone.zoneType.Value = dangerZoneConfig.type;
            instance.name = "DangerZone" + Constants.Separator + dangerZoneConfig.id;
            instance.SetActive(true);
            return dangerZone;
        }
    }
}
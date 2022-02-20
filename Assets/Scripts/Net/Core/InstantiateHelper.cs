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

        public static void InstantiateObject(SpaceUnitConfig worldObject)
        {
            worldObject.id = worldObject.id == Guid.Empty ? Guid.NewGuid() : worldObject.id;
            var prefabName = worldObject.prefabName;
            var goToInstantiate = Resources.Load(Constants.PathToPrefabs + prefabName);
            var instance =
                Object.Instantiate(goToInstantiate, worldObject.position, worldObject.rotation) as
                    GameObject;
            instance.name = worldObject.prefabName + Constants.Separator + worldObject.id;
            instance.GetComponent<UnitScript>()?.NetworkUnitConfig.Init(new SpaceUnitDto(worldObject));
            instance.SetActive(true);
        }

        public static void InstantiateDangerZone(DangerZoneConfig dangerZone)
        {
            var goToInstantiate = Resources.Load(Constants.PathToPrefabs + "DangerZone") as GameObject;
            var instance = Object.Instantiate(goToInstantiate, dangerZone.Center, Quaternion.Euler(90, 0, 0));
            instance.GetComponent<DangerZone>().id = dangerZone.Id;
            dangerZone.Id = Guid.NewGuid();
            instance.GetComponent<DangerZone>().zoneColor.Value = dangerZone.Color;
            instance.GetComponent<DangerZone>().zoneStressDamage.Value = dangerZone.StressDamage;
            instance.GetComponent<DangerZone>().zoneHpDamage.Value = dangerZone.HpDamage;
            instance.GetComponent<DangerZone>().zoneRadius.Value = dangerZone.Radius;
            instance.SetActive(true);
        }
    }
}
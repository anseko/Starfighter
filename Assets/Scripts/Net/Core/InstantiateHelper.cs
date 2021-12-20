using System;
using Client.Core;
using Core;
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
            
            var shipPrefab = Resources.Load(Constants.PathToPrefabs + ship.prefabName);

            var shipInstance = Object.Instantiate(shipPrefab, position: ship.position,
                                rotation: ship.rotation) as GameObject;
             
            shipInstance.name = ship.prefabName + Constants.Separator + ship.shipId;
            shipInstance.tag = Constants.DynamicTag;
            
            var playerScript = shipInstance.GetComponent<PlayerScript>() ?? shipInstance.AddComponent<PlayerScript>();
            playerScript.unitConfig = ship;
            shipInstance.SetActive(true);
            return playerScript;
        }

        public static GameObject InstantiateObject(SpaceUnitConfig worldObject)
        {
            worldObject.id = worldObject.id == Guid.Empty ? Guid.NewGuid() : worldObject.id;
            var prefabName = worldObject.prefabName;
            // Debug.unityLogger.Log($"Try to load resource: {Constants.PathToPrefabs + prefabName}");
            var goToInstantiate = Resources.Load(Constants.PathToPrefabs + prefabName);
            var instance =
                Object.Instantiate(goToInstantiate, worldObject.position, worldObject.rotation) as
                    GameObject;
            instance.name = worldObject.prefabName + Constants.Separator + worldObject.id;
            instance.tag = Constants.DynamicTag;
            instance.SetActive(true);
            return instance;
        }

        public static void InstantiateDangerZone(DangerZoneConfig dangerZone)
        {
            var goToInstantiate = Resources.Load(Constants.PathToPrefabs + "DangerZone") as GameObject;
            var instance = Object.Instantiate(goToInstantiate, dangerZone.Center, Quaternion.Euler(90, 0, 0));
            instance.GetComponent<DangerZone>().zoneColor.Value = dangerZone.Color;
            instance.GetComponent<DangerZone>().zoneDamage.Value = dangerZone.Damage;
            instance.GetComponent<DangerZone>().zoneRadius.Value = dangerZone.Radius;
            instance.SetActive(true);
        }
    }
}
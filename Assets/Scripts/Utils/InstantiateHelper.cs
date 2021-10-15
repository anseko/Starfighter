using System;
using Client;
using Client.Core;
using Core;
using Core.ClassExtensions;
using MLAPI;
using Net.PackageData;
using ScriptableObjects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils
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
            playerScript.movementAdapter = MovementAdapter.RemoteNetworkControl;
            playerScript.unitConfig = ship;
            playerScript.unitStateMachine = new UnitStateMachine(playerScript.gameObject, ship.shipState);
            shipInstance.SetActive(true);
            return playerScript;
        }

        public static GameObject InstantiateObject(WorldObject worldObject)
        {
            var prefabName = worldObject.name.Split(Constants.Separator)[0];
            // Debug.unityLogger.Log($"Try to load resource: {Constants.PathToPrefabs + prefabName}");
            var goToInstantiate = Resources.Load(Constants.PathToPrefabs + prefabName);
            var instance =
                Object.Instantiate(goToInstantiate, worldObject.position, worldObject.rotation) as
                    GameObject;
            instance.name = worldObject.name;
            instance.tag = Constants.DynamicTag;
            if (instance.GetComponent<PlayerScript>() != null && worldObject is SpaceShip ship)
            {
                var ps = instance.GetComponent<PlayerScript>();
                ps.unitConfig = ship.dto.ToConfig();
                ps.unitStateMachine = new UnitStateMachine(ps.gameObject, ship.shipState);
            }
            instance.SetActive(true);
            return instance;
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
    }
}
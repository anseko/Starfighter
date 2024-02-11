using System;
using System.Linq;
using Client.Core;
using Core;
using Mirror;
using Net;
using Net.Core;
using ScriptableObjects;
using UnityEngine;

namespace Client.UI.Admin
{
    public class Spawner: NetworkBehaviour
    {
        public Camera _cam;
        public GameObject selectedPrefab;
        [SerializeField] private ShipInfoCollector _shipInfoCollector;
        [SerializeField] private UnitInfoCollector _unitInfoCollector;
        [SerializeField] private DangerZoneInfoCollector _zoneInfoCollector;


        public void Init()
        {
            _cam = FindObjectOfType<Camera>(false);
            _shipInfoCollector = FindObjectOfType<ShipInfoCollector>();
            _unitInfoCollector = FindObjectOfType<UnitInfoCollector>();
            _zoneInfoCollector = FindObjectOfType<DangerZoneInfoCollector>();
        }
        
        public void Spawn(string pathToConfig, string prefabName, string newShipId)
        {
            var position = new Vector3(_cam.transform.position.x, 1, _cam.transform.position.z);

            SpawnServerRpc(pathToConfig, prefabName, newShipId, position);
        }

        public void Despawn()
        {
            if (selectedPrefab.TryGetComponent<NetworkIdentity>(out var netObj))
            {
                DespawnServerRpc(netObj.netId);
            }
        }

        [Command]
        private void SpawnServerRpc(string pathToConfig, string prefabName, string newShipId, Vector3 position)
        {
            Debug.unityLogger.Log($"Server spawn: {pathToConfig}{prefabName}.");
            
            GameObject goToSpawn;
            //BUG: Bad practice - look through all resources to find one. Maybe store them somewhere?
            if (pathToConfig == Constants.PathToShipsObjects)
            {
                var config = Resources.LoadAll<SpaceShipConfig>(pathToConfig).FirstOrDefault(x => x.prefabName == prefabName);
                if (config is null)
                {
                    Debug.unityLogger.Log($"Can't find any shipConfig for {prefabName}");
                    return;
                }

                var configInstance = Instantiate(config);
                configInstance.shipId = newShipId;
                goToSpawn = InstantiateHelper.InstantiateServerShip(configInstance).gameObject;
            }
            else if(pathToConfig == Constants.PathToUnitsObjects)
            {
                var config = Resources.LoadAll<SpaceUnitConfig>(pathToConfig).FirstOrDefault(x => x.prefabName == prefabName);
                if (config is null)
                {
                    Debug.unityLogger.Log($"Can't find any unitConfig for {prefabName}");
                    return;
                }
                var configInstance = Instantiate(config);
                goToSpawn = InstantiateHelper.InstantiateObject(configInstance).gameObject;
                config.id = Guid.Empty;
            }
            else
            {
                var config = Resources.Load<DangerZoneConfig>(pathToConfig + prefabName);
                if (config is null)
                {
                    Debug.unityLogger.Log($"Can't find any dangerZoneConfig for {prefabName}");
                    return;
                }

                var configInstance = Instantiate(config);
                if (float.TryParse(newShipId, out var newRadius)) configInstance.radius = newRadius;
                goToSpawn = InstantiateHelper.InstantiateDangerZone(configInstance).gameObject;
                config.id = Guid.Empty;
            }
            
            if (goToSpawn.TryGetComponent<NetworkIdentity>(out var netObj) && !netObj.SpawnedFromInstantiate)
            {
                NetworkServer.Spawn(goToSpawn);
                netObj.gameObject.transform.position = position;
            }
            else
            {
                Debug.unityLogger.Log($"Can't spawn. Is already spawned? {netObj.SpawnedFromInstantiate}");
                return;
            }

            PostSpawnClientRpc(netObj.netId);
        }

        [ClientRpc]
        private void PostSpawnClientRpc(uint objectId)
        {
            selectedPrefab = NetworkServer.spawned[objectId].gameObject;
                
            if (selectedPrefab.TryGetComponent<PlayerScript>(out var ps))
            {
                _shipInfoCollector.Add(ps);
            }
            else if (selectedPrefab.TryGetComponent<UnitScript>(out var us))
            {
                _unitInfoCollector.Add(us);
            }
            else if(selectedPrefab.TryGetComponent<DangerZone>(out var dz))
            {
                _zoneInfoCollector.Add(dz);
            }
        }
        
        [Command]
        private void DespawnServerRpc(uint objectId)
        {
            var server = FindObjectOfType<StarfighterNetworkManager>();
            var objectToDespawn = NetworkServer.spawned[objectId].gameObject;

            //Если деспавним кораблик из игры, то отключаем все аккаунты, с ним связанные...
            // if (objectToDespawn.TryGetComponent<PlayerScript>(out var ps))
            // {
            //     var accounts = server.accountObjects
            //         .Where(acc => (acc.ship != null ? acc.ship.shipId : null) == ps.networkUnitConfig.shipId).ToList();
            //         accounts.Where(x=>x.connectionId != null).ToList()
            //             .ForEach(acc=> 
            //                 DisconnectClient(acc.connectionId!)
            //                 );
            //         server.accountObjects.RemoveAll(x => accounts.Contains(x));
            // }
            
            NetworkServer.Destroy(objectToDespawn);
        }

        [Command]
        public void AddAccountServerRpc(string login, string password, string newShipId, string prefabName)
        {
            Debug.unityLogger.Log($"AddAcc {login}:{password} for {newShipId}");
            var acc = ScriptableObject.CreateInstance<ClientAccountObject>();
            acc.login = login;
            acc.password = password;
            acc.connectionId = null;
            acc.type = UserType.Pilot;
            var configToInstance = Resources.LoadAll<SpaceShipConfig>(Constants.PathToShipsObjects).FirstOrDefault(x => x.prefabName == prefabName);
            var config = Instantiate(configToInstance);
            config.shipId = newShipId;
            acc.ship = config;

            var server = FindObjectOfType<StarfighterNetworkManager>();
            if (server.accountObjects.Any(x => x.ship?.shipId == acc.ship.shipId && x.type == acc.type))
            {
                Debug.unityLogger.Log($"There is such account in list already {acc.ship.shipId}:{acc.type}");
                return;
            }
            
            server.accountObjects.Add(acc);
        }
    }
}
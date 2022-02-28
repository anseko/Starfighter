using System;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

namespace Client.UI.Station
{
    public class PoiSpawner: NetworkBehaviour
    {
        [SerializeField] private GameObject _poiPrefab;
        private OrdersScript _ordersScript;

        private void Awake()
        {
            
        }

        [ServerRpc(RequireOwnership = false)]
        public void HandleUnitServerRpc(OrdersScript.OrderUnit unit, ulong callbackClientId)
        {
            switch (unit.operation)
            {
                case OrdersScript.OrderOperation.Add:
                    var poiCopy = Instantiate(_poiPrefab);
                    poiCopy.GetComponent<POIFrameInit>().FrameInit(unit.identifier, unit.position, unit.size, 
                        unit.text, true);
                    var networkObjectComp = poiCopy.GetComponent<NetworkObject>();
                    networkObjectComp.Spawn();
                    unit.networkObjectId = networkObjectComp.NetworkObjectId;

                    var settings = new ClientRpcParams()
                    {
                        Send = new ClientRpcSendParams()
                        {
                            TargetClientIds = new[] { callbackClientId }
                        }
                    };
                    
                    CreatePoiClientRpc(unit, settings);
                    ResizePoiClientRpc(unit);
                    break;
                
                case OrdersScript.OrderOperation.Edit:
                    ResizePoiClientRpc(unit);
                    unit.orderPlane.GetComponent<POIFrameInit>().FrameInit(unit.identifier, unit.position, unit.size, 
                        unit.text, true);
                    break;
                
                case OrdersScript.OrderOperation.Remove:
                    GetNetworkObject(unit.networkObjectId).Despawn(true);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        private void CreatePoiClientRpc(OrdersScript.OrderUnit unit, ClientRpcParams clientRpcParams = default)
        {
            FindObjectOfType<OrdersScript>(true).AfterSpawnPoi(unit);
        }

        [ClientRpc]
        private void ResizePoiClientRpc(OrdersScript.OrderUnit unit)
        {
            var go = GetNetworkObject(unit.networkObjectId).gameObject;
            go.GetComponent<POIFrameInit>().FrameInit(unit.identifier, unit.position, unit.size, 
                unit.text, true);
        }
    }
}
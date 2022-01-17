using System;
using System.Collections.Generic;
using System.Linq;
using Client;
using Client.Core;
using Client.UI;
using Core;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace Net.Components
{
    public class DockComponent: NetworkBehaviour
    {
        [SerializeField]
        private PlayerScript _unit;
        public bool isDockable = true;
        public UnitScript lastThingToDock;
        public NetworkVariableBool readyToDock;
        [SerializeField]
        private List<DockingTrigger> _dockingMarkers;
        [SerializeField]
        private DockCheckZone _dockCheckZone;

        
        private void Awake()
        {
            readyToDock = new NetworkVariableBool(new NetworkVariableSettings {WritePermission = NetworkVariablePermission.OwnerOnly}, false);
            ClientEventStorage.GetInstance().DockIndicatorStateRequest.AddListener(IndicatorUpdate);
        }

        private void Start()
        {
            // _dockCheckZone = GetComponentInChildren<DockCheckZone>();
            if (_dockingMarkers != null)
            {
                _dockingMarkers.ForEach(x => x.Init(this));
            }
            else
            {
                _dockingMarkers = new List<DockingTrigger>();
            }
        }

        private void Update()
        {
            if (!IsOwner) return;
            
            readyToDock.Value = _dockingMarkers.Any(x => x.dockAvailable);
            
            if(_unit is null) return;
            
            if (Input.GetKeyDown(_unit.keyConfig.dock) && readyToDock.Value)
            {
                TryToDockServerRpc(GetComponent<NetworkObject>().NetworkObjectId, lastThingToDock.GetComponent<NetworkObject>().NetworkObjectId);
            }
        }

        private void IndicatorUpdate()
        {
            if (!IsOwner) return;
            
            if (GetState() == UnitState.IsDocked)
            {
                ClientEventStorage.GetInstance().IsDocked.Invoke();
                return;
            }

            if (readyToDock.Value)
            {
                ClientEventStorage.GetInstance().DockingAvailable.Invoke();
                return;
            }

            if (_dockCheckZone.IsAnyInZone())
            {
                ClientEventStorage.GetInstance().DockableUnitsInRange.Invoke();
                return;
            }
            
            ClientEventStorage.GetInstance().NoOneToDock.Invoke();
        }
        
        public UnitState GetState()
        {
            return _unit?.GetState() ?? UnitState.InFlight;
        }

        [ServerRpc]
        public void EmergencyUndockServerRpc(ulong myObjectId, ulong otherObjectId, bool changeSelfState = false)
        {
            var myObj = FindObjectsOfType<NetworkObject>().FirstOrDefault(x => x.NetworkObjectId == myObjectId);
            var otherObj = FindObjectsOfType<NetworkObject>().FirstOrDefault(x => x.NetworkObjectId == otherObjectId);
            if (myObj is null || otherObj is null) return;

            var otherUnit = otherObj.GetComponent<PlayerScript>();
            
            if(changeSelfState) _unit.unitStateMachine.ChangeState(UnitState.InFlight);
            
            if (otherUnit != null)
            {
                otherUnit.unitStateMachine.ChangeState(UnitState.InFlight);
            }
            transform.SetParent(null);
            
            EmergencyUndockClientRpc();
            otherObj.GetComponent<DockComponent>().EmergencyUndockClientRpc();
        }
        
        [ServerRpc]
        private void TryToDockServerRpc(ulong myObjectId, ulong otherObjectId)
        {
            var myObj = FindObjectsOfType<NetworkObject>().FirstOrDefault(x => x.NetworkObjectId == myObjectId);
            var otherObj = FindObjectsOfType<NetworkObject>().FirstOrDefault(x => x.NetworkObjectId == otherObjectId);
            if (myObj is null || otherObj is null) return;

            var otherIsReady = false;
            otherIsReady = otherObj.IsOwnedByServer || otherObj.GetComponent<DockComponent>().readyToDock.Value;

            if (!otherIsReady) return;

            var otherUnit = otherObj.GetComponent<PlayerScript>();
            
            switch (_unit.GetState())
            {
                case UnitState.InFlight:
                    _unit.unitStateMachine.ChangeState(UnitState.IsDocked);
                    if (otherUnit != null)
                    {
                        //если мы стыкуемся к объекту с PlayerScript
                        otherUnit.unitStateMachine.ChangeState(UnitState.IsDocked);
                    }
                    else
                    {
                        //если мы стыкуемся к объекту без PlayerScript
                        transform.SetParent(otherObj.transform);
                    }
                    break;
                case UnitState.IsDocked:
                    _unit.unitStateMachine.ChangeState(UnitState.InFlight);
                    if (otherUnit != null)
                    {
                        otherUnit.unitStateMachine.ChangeState(UnitState.InFlight);
                    }
                    //если мы отходим от объекта без PlayerScript
                    transform.SetParent(null);
                    break;
                case UnitState.IsDead:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
                
            SwitchDockStateClientRpc();
            otherObj.GetComponent<DockComponent>().SwitchDockStateClientRpc();
        }

        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        private void SwitchDockStateClientRpc()
        {
            if (!IsOwner || IsServer || _unit is null) return;
            //Клиент не может быть владельцем unitScript 
            switch (_unit.GetState())
            {
                case UnitState.InFlight:
                    _unit.unitStateMachine.ChangeState(UnitState.IsDocked);
                    if (!lastThingToDock.TryGetComponent<PlayerScript>(out var ps))
                    {
                        transform.SetParent(lastThingToDock.transform);
                    }
                    break;
                case UnitState.IsDocked:
                    _unit.unitStateMachine.ChangeState(UnitState.InFlight);
                    transform.SetParent(null);
                    break;
                case UnitState.IsDead:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        private void EmergencyUndockClientRpc()
        {
            if (!IsOwner || IsServer || _unit is null) return;
            //Клиент не может быть владельцем unitScript 
            switch (_unit.GetState())
            {
                case UnitState.IsDocked:
                    _unit.unitStateMachine.ChangeState(UnitState.InFlight);
                    transform.SetParent(null);
                    break;
                case UnitState.InFlight:
                case UnitState.IsDead:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
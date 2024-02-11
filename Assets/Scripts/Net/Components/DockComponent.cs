using System;
using System.Collections.Generic;
using System.Linq;
using Client;
using Client.Core;
using Client.UI;
using Core;
using Mirror;
using UnityEngine;

namespace Net.Components
{
    public class DockComponent: NetworkBehaviour
    {
        [SerializeField]
        private PlayerScript _unit;
        public bool isDockable = true;
        public UnitScript lastThingToDock;
        [SyncVar]
        public bool readyToDock;
        [SerializeField]
        private List<DockingTrigger> _dockingMarkers;
        [SerializeField]
        private DockCheckZone _dockCheckZone;

        
        private void Awake()
        {
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
            if (!isOwned) return;
            
            readyToDock = _dockingMarkers.Any(x => x.dockAvailable);
            
            if(_unit is null) return;
            
            if (Input.GetKeyDown(_unit.keyConfig.dock) && (readyToDock || _unit.networkUnitConfig.shipState == UnitState.IsDocked))
            {
                if (_unit.networkUnitConfig.shipState == UnitState.IsDocked)
                {
                    EmergencyUndockServerRpc(lastThingToDock?.GetComponent<NetworkIdentity>()?.netId ?? default, true);
                }
                else
                {
                    TryToDockServerRpc(lastThingToDock?.GetComponent<NetworkIdentity>()?.netId ?? default);
                }
            }
        }

        private void IndicatorUpdate()
        {
            if (!isOwned) return;
            
            if (GetState() == UnitState.IsDocked)
            {
                ClientEventStorage.GetInstance().IsDocked.Invoke();
                return;
            }

            if (readyToDock)
            {
                ClientEventStorage.GetInstance().DockingAvailable.Invoke();
                return;
            }

            if (_dockCheckZone != null && _dockCheckZone.IsAnyInZone())
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

        [Command]
        public void EmergencyUndockServerRpc(ulong otherObjectId, bool changeSelfState = false)
        {
            var otherObj = FindObjectsOfType<NetworkIdentity>().FirstOrDefault(x => x.netId == otherObjectId);
            if (otherObj is null && otherObjectId != default) return;

            var otherUnit = otherObj?.GetComponent<PlayerScript>();
            
            if (changeSelfState) _unit.networkUnitConfig.shipState = UnitState.InFlight;
            
            if (otherUnit != null && otherUnit.NetworkUnitConfig.ShipState != UnitState.IsDead) otherUnit.NetworkUnitConfig.ShipState = UnitState.InFlight;
            
            transform.SetParent(null);
        }
        
        [Command]
        private void TryToDockServerRpc(ulong otherObjectId)
        {
            var otherObj = FindObjectsOfType<NetworkIdentity>().FirstOrDefault(x => x.netId == otherObjectId);
            if (otherObj is null) return;

            var otherIsReady = false;
            otherIsReady = otherObj.IsOwnedByServer || otherObj.GetComponent<DockComponent>().readyToDock;

            if (!otherIsReady) return;

            var otherUnit = otherObj.GetComponent<PlayerScript>();
            
            switch (_unit.GetState())
            {
                case UnitState.InFlight:
                    _unit.networkUnitConfig.shipState = UnitState.IsDocked;
                    if (otherUnit != null)
                    {
                        if (otherUnit.NetworkUnitConfig.ShipState == UnitState.IsDead) break;
                        //если мы стыкуемся к объекту с PlayerScript
                        otherUnit.NetworkUnitConfig.ShipState = UnitState.IsDocked;
                    }
                    else
                    {
                        //если мы стыкуемся к объекту без PlayerScript
                        transform.SetParent(otherObj.transform);
                    }
                    break;
                case UnitState.IsDocked:
                    _unit.networkUnitConfig.shipState = UnitState.InFlight;
                    if (otherUnit != null)
                    {
                        if (otherUnit.NetworkUnitConfig.ShipState == UnitState.IsDead) break;
                        otherUnit.NetworkUnitConfig.ShipState = UnitState.InFlight;
                    }
                    //если мы отходим от объекта без PlayerScript
                    transform.SetParent(null);
                    break;
                case UnitState.IsDead:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
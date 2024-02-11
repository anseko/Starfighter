using System;
using System.Linq;
using Core;
using Mirror;
using Net.Components;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Client.Core
{
    public interface IUnitState
    {
        UnitState State { get; }
        void OnEnter(GameObject unit);
        void OnExit(GameObject unit);
        void Update(GameObject unit);
    }

    public class InFlightState : IUnitState
    {
        public UnitState State => UnitState.InFlight;
        private PlayerScript _playerScript;
        
        public void OnEnter(GameObject unit)
        {
            _playerScript = unit.GetComponent<PlayerScript>();
            if (_playerScript.TryGetComponent<MoveComponent>(out var moveComponent)) moveComponent.enabled = true;
        }

        public void Update(GameObject unit)
        {
            if (!_playerScript.isOwned) return;
            _playerScript.shipSpeed = _playerScript.Rigidbody.velocity;
            _playerScript.shipRotation = _playerScript.Rigidbody.angularVelocity;
        }

        public void OnExit(GameObject unit)
        {
            if (_playerScript.TryGetComponent<MoveComponent>(out var moveComponent)) moveComponent.enabled = false;
        }
    }
    
    public class IsDockedState : IUnitState
    {
        public UnitState State => UnitState.IsDocked;
        
        public void OnEnter(GameObject unit)
        {
            unit.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            if(unit.TryGetComponent<GrappleComponent>(out var grappler))
                grappler.enabled = false;

            var dockComp = unit.GetComponent<DockComponent>();
            
            if (dockComp.lastThingToDock != null && dockComp.lastThingToDock.TryGetComponent<AIComponent>(out var aiComponent))
            {
                aiComponent.Pause();
            }
            
            ClientEventStorage.GetInstance().IsDocked.Invoke();
        }

        public void Update(GameObject unit)
        {
            
        }
        
        public void OnExit(GameObject unit)
        {
            unit.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
            if(unit.TryGetComponent<GrappleComponent>(out var grappler))
                grappler.enabled = true;

            var dockComp = unit.GetComponent<DockComponent>();
            
            if (dockComp.lastThingToDock != null && dockComp.lastThingToDock.TryGetComponent<AIComponent>(out var aiComponent))
            {
                aiComponent.Resume();
            }
            
            ClientEventStorage.GetInstance().DockingAvailable.Invoke();
        }
    }

    public class IsDeadState : IUnitState
    {
        public UnitState State => UnitState.IsDead;
        
        public void OnEnter(GameObject unit)
        {
            //Если были пристыкованы - отстыковаться
            var unitPS = unit.GetComponent<PlayerScript>();
            unitPS.Rigidbody.velocity = Vector3.zero;
            unitPS.Rigidbody.angularVelocity = Vector3.zero;
            
            if (unitPS.unitStateMachine?.previousState == UnitState.IsDocked &&
                unit.TryGetComponent<DockComponent>(out var dockComp) &&
                dockComp.lastThingToDock.TryGetComponent<PlayerScript>(out var ps))
            {
                dockComp.EmergencyUndockServerRpc(dockComp.lastThingToDock.GetComponent<NetworkIdentity>().netId, false);
            }

            if (unit.TryGetComponent<GrappleComponent>(out var grappleComponent))
            {
                var grappler = Object.FindObjectsOfType<Grappler>()
                    .FirstOrDefault(x => x.isOwned);
                grappler?.DestroyOnServer();
            }
            
            var beacon = unit.GetComponentInChildren<BeaconComponent>(true);
            if (beacon == null) return; 
            beacon.ChangeState(true);
            
            if (unitPS.isOwned) unitPS.GiveAwayShipOwnershipServerRpc();
        }

        public void Update(GameObject unit)
        {
        }
        
        public void OnExit(GameObject unit)
        {
            var beacon = unit.GetComponentInChildren<BeaconComponent>(true);
            if (beacon == null) return; 
            beacon.ChangeState(false);
            
            var unitPS = unit.GetComponent<PlayerScript>();
            if (unitPS.isGrappled)
            {
                var id = unit.GetComponent<NetworkIdentity>().netId;
                var grappler = Object.FindObjectsOfType<Grappler>()
                    .FirstOrDefault(x => x.grappledObjectId == id);
                grappler?.DestroyOnServer();
            }
            
            unitPS.RequestShipOwnership();
        }
    }
    
    public class UnitStateMachine
    {
        public UnitState previousState;
        public IUnitState currentState;
        private readonly GameObject _unit;
        private readonly IsDeadState _isDeadState;
        private readonly IsDockedState _isDockedState;
        private readonly InFlightState _inFlightState;

        public UnitStateMachine(GameObject unitToControl, UnitState currentState = UnitState.InFlight)
        {
            _isDeadState = new IsDeadState();
            _isDockedState = new IsDockedState();
            _inFlightState = new InFlightState();
            _unit = unitToControl;

            this.currentState = currentState switch
            {
                UnitState.InFlight => _inFlightState,
                UnitState.IsDocked => _isDockedState,
                UnitState.IsDead => _isDeadState,
                _ => _inFlightState
            };
            previousState = this.currentState.State;
            this.currentState.OnEnter(_unit);
        }

        public void ChangeState(UnitState newState)
        {
            if (currentState.State == newState) return;
            
            Debug.unityLogger.Log($"State changing to {newState}");
            currentState.OnExit(_unit);
            previousState = currentState.State;
            currentState = newState switch
            {
                UnitState.InFlight => _inFlightState,
                UnitState.IsDocked => _isDockedState,
                UnitState.IsDead => _isDeadState,
                _ => throw new ArgumentOutOfRangeException(nameof(newState), newState, null)
            };

            currentState.OnEnter(_unit);
        }

        public void Update() => currentState.Update(_unit);
    }
}
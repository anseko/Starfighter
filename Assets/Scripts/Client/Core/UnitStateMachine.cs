using System;
using System.Linq;
using Core;
using MLAPI;
using Net.Components;
using ScriptableObjects;
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
            _playerScript.GetComponent<MoveComponent>().enabled = true;
        }

        public void Update(GameObject unit)
        {
            if (!_playerScript.IsOwner) return;
            _playerScript.shipSpeed.Value = _playerScript.rigidbody.velocity;
            _playerScript.shipRotation.Value = _playerScript.rigidbody.angularVelocity;
        }

        public void OnExit(GameObject unit)
        {
            _playerScript.GetComponent<MoveComponent>().enabled = false;
        }
    }
    
    public class IsDockedState : IUnitState
    {
        public UnitState State => UnitState.IsDocked;
        
        public void OnEnter(GameObject unit)
        {
            //запретить перемещения, подписаться на триггер столкновения для принудительного разрыва
            unit.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            if(unit.TryGetComponent<GrappleComponent>(out var grappler))
                grappler.enabled = false;
            ClientEventStorage.GetInstance().IsDocked.Invoke();
            //TODO: поменять UI?
        }

        public void Update(GameObject unit)
        {
            
        }
        
        public void OnExit(GameObject unit)
        {
            //разрешить перемещения, отписаться от триггера столкновений для принудительного разрыва
            unit.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
            if(unit.TryGetComponent<GrappleComponent>(out var grappler))
                grappler.enabled = true;
            ClientEventStorage.GetInstance().DockingAvailable.Invoke();
            //TODO: поменять UI?
        }
    }

    public class IsDeadState : IUnitState
    {
        public UnitState State => UnitState.IsDead;
        
        public void OnEnter(GameObject unit)
        {
            //Если были пристыкованы - отстыковаться
            var dockComp = unit.GetComponent<DockComponent>();
            var unitPS = unit.GetComponent<PlayerScript>();
            unit.GetComponent<Rigidbody>().velocity = Vector3.zero;
            unit.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            
            if (unitPS.unitStateMachine?.previousState == UnitState.IsDocked &&
                dockComp.lastThingToDock.TryGetComponent<PlayerScript>(out var ps))
            {
                dockComp.EmergencyUndockServerRpc(unit.GetComponent<NetworkObject>().NetworkObjectId,
                    dockComp.lastThingToDock.GetComponent<NetworkObject>().NetworkObjectId);
            }
            
            //TODO: отключить весь функционал, кроме аварийного
            if(unitPS.volume != null && unitPS.IsOwner) unitPS.volume.gameObject.SetActive(true);
            if(unitPS.IsOwner) unitPS.GiveAwayShipOwnershipServerRpc();

            var beacon = unit.GetComponentInChildren<BeaconComponent>(true);
            if (beacon == null) return; 
            beacon.ChangeState(true);
        }

        public void Update(GameObject unit)
        {
            var ps = unit.GetComponent<PlayerScript>();
            if (ps.currentHp.Value > 0 && NetworkManager.Singleton.IsServer) //Нет нужды вызывать отдельно явно на клиентах и сервере, так как currentHp будет обновлено на всех
            {
                Debug.unityLogger.Log("Trying to resurrect self");
                // ps.unitStateMachine.ChangeState(UnitState.InFlight);
                ps.currentState.Value = UnitState.InFlight;
            }
        }
        
        public void OnExit(GameObject unit)
        {
            var beacon = unit.GetComponentInChildren<BeaconComponent>(true);
            if (beacon == null) return; 
            beacon.ChangeState(false);
            
            var unitPS = unit.GetComponent<PlayerScript>();
            if(unitPS.volume != null) unitPS.volume.gameObject.SetActive(false);
            if (unitPS.isGrappled.Value)
            {
                var id = unit.GetComponent<NetworkObject>().NetworkObjectId;
                var grappler = Object.FindObjectsOfType<Grappler>()
                    .FirstOrDefault(x => x.grappledObjectId.Value == id);
                grappler?.DestroyOnServer(id);
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
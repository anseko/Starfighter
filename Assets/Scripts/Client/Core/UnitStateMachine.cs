using System;
using Core;
using MLAPI;
using Net.Components;
using UnityEngine;

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
            if (unit.GetComponent<PlayerScript>().unitStateMachine.previousState.State == UnitState.IsDocked &&
                dockComp.lastThingToDock.TryGetComponent<PlayerScript>(out var ps))
            {
                dockComp.EmergencyUndockServerRpc(unit.GetComponent<NetworkObject>().NetworkObjectId,
                    dockComp.lastThingToDock.GetComponent<NetworkObject>().NetworkObjectId);
            }
            
            //TODO: отключить весь функционал, кроме аварийного?
            unit.GetComponent<PlayerScript>().GiveAwayShipOwnershipServerRpc();
        }

        public void Update(GameObject unit)
        {
            //TODO: испускать маяком сигнал
        }
        
        public void OnExit(GameObject unit)
        {
            //TODO: полагаю, выставить новые параметры
            unit.GetComponent<PlayerScript>().RequestShipOwnership(); 
        }
    }
    
    public class UnitStateMachine
    {
        public IUnitState previousState;
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
            previousState = this.currentState;
            this.currentState.OnEnter(_unit);
        }

        public void ChangeState(UnitState newState)
        {
            Debug.unityLogger.Log($"State changing to {newState}");
            currentState.OnExit(_unit);
            previousState = currentState;
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
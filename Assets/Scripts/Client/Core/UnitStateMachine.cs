using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Client.Core
{
    public interface IUnitState
    {
        UnitState State { get; }
        void OnEnter(GameObject unit);
        void OnExit(GameObject unit);
    }

    public class InFlightState : IUnitState
    {
        public UnitState State => UnitState.InFlight;
        
        public void OnEnter(GameObject unit)
        {

        }

        public void OnExit(GameObject unit)
        {

        }
    }
    
    public class IsDockedState : IUnitState
    {
        public UnitState State => UnitState.IsDocked;
        
        public void OnEnter(GameObject unit)
        {
            //TODO: запретить перемещения, подписаться на триггер столкновения для принудительного разрыва
            unit.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            unit.GetComponent<PlayerScript>().lastThingToDock.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            //TODO: поменять UI
        }

        public void OnExit(GameObject unit)
        {
            //TODO: разрешить перемещения, отписаться от триггера столкновений для принудительного разрыва
            unit.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            unit.GetComponent<PlayerScript>().lastThingToDock.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            //TODO: поменять UI
        }
    }

    public class IsDeadState : IUnitState
    {
        public UnitState State => UnitState.IsDead;
        
        public void OnEnter(GameObject unit)
        {
            //TODO: ХП в ноль, отключить весь функционал, кроме аварийного?
        }

        public void OnExit(GameObject unit)
        {
            //TODO: полагаю, выставить новые параметры
        }
    }
    
    public class UnitStateMachine
    {
        public IUnitState currentState;
        public GameObject unit;
        private readonly IsDeadState _isDeadState;
        private readonly IsDockedState _isDockedState;
        private readonly InFlightState _inFlightState;

        public UnitStateMachine(GameObject unitToControl, UnitState currentState = UnitState.InFlight)
        {
            _isDeadState = new IsDeadState();
            _isDockedState = new IsDockedState();
            _inFlightState = new InFlightState();
            unit = unitToControl;

            switch (currentState)
            {
                case UnitState.InFlight:
                    this.currentState = _inFlightState; 
                    break;
                case UnitState.IsDocked:
                    this.currentState = _isDockedState;
                    break;
                case UnitState.IsDead:
                    this.currentState = _isDeadState;
                    break;
                default:
                    this.currentState = _inFlightState;
                    break;
            }
        }

        public void ChangeState(UnitState newState)
        {
            Debug.unityLogger.Log($"State changing to {newState}");
            currentState.OnExit(unit);
            unit.GetComponent<PlayerScript>().ShipsBrain.OnStateChange(newState);
            switch (newState)
            {
                case UnitState.InFlight:
                    currentState = _inFlightState;
                    break;
                case UnitState.IsDocked:
                    currentState = _isDockedState;
                    break;
                case UnitState.IsDead:
                    currentState = _isDeadState;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
            
            currentState.OnEnter(unit);
        }
        
    }
}
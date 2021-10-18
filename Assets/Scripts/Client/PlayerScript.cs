using System;
using System.Collections.Generic;
using Client.Core;
using Client.Movement;
using Client.UI;
using Core;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using ScriptableObjects;
using UnityEngine;

namespace Client
{
    public class PlayerScript : UnitScript
    {
        public DockingTrigger dockingTrigger;
        public Vector3 shipSpeed, shipRotation;
        public UnitStateMachine unitStateMachine;
        public UnitScript lastThingToDock;
        public bool readyToDock = false;
        public bool localUsage = false;
        
        private Transform _front, _back, _left, _right;
        private Rigidbody _ship;
        private ParticleSystem _tlm, _trm, _blm, _brm, _te;
        private ConstantForce _thrustForce;
        
        
        private void Start()
        {
            if (localUsage)
            {
                unitConfig = Resources.Load<SpaceShipConfig>(Constants.PathToShipsObjects + "SpaceShipConfig");
                ClientEventStorage.GetInstance().InitPilot.Invoke(this);
            }
            
            unitStateMachine = new UnitStateMachine(gameObject, (unitConfig as SpaceShipConfig).shipState);
            dockingTrigger.Init(this);
            Debug.unityLogger.Log($"PS {(unitConfig as SpaceShipConfig).shipState}");

            _front = gameObject.transform.Find("Front");
            _back = gameObject.transform.Find("Back");
            _left = gameObject.transform.Find("Left");
            _right = gameObject.transform.Find("Right");
            _ship = GetComponent<Rigidbody>();
            _thrustForce = GetComponent<ConstantForce>();
            shipSpeed = Vector3.zero;
            shipRotation = Vector3.zero;
            
            _trm = gameObject.transform.Find("TopRightEmition").GetComponent<ParticleSystem>();
            _tlm = gameObject.transform.Find("TopLeftEmition").GetComponent<ParticleSystem>();
            _brm = gameObject.transform.Find("BotRightEmition").GetComponent<ParticleSystem>();
            _blm = gameObject.transform.Find("BotLeftEmition").GetComponent<ParticleSystem>();
            _te = gameObject.transform.Find("ThurstsEmition").GetComponent<ParticleSystem>();

            #region Reset movement animation
            
            _te.Stop();
            _tlm.Stop();
            _trm.Stop();
            _brm.Stop();
            _blm.Stop();
            
            #endregion

            // if (IsOwner && IsClient)
            // {
            //     ShipsBrain = new PlayerControl(GetState());
            // }
            // else
            // {
            //     ShipsBrain = new RemoteNetworkControl();
            // }
        }
        
        public UnitState GetState()
        {
            return unitStateMachine.currentState.State;
        }
        
        private void FixedUpdate()
        {
            unitStateMachine.Update();
        }
        
        // public void UpdateMovement()
        // {
        //     if (!IsOwner) return; 
        //     // расчет вектора тяги
        //     var thrustForceVector = _front.transform.position - _back.transform.position; //вектор фронтальной тяги
        //     var maneurForceVector = _right.transform.position - _left.transform.position; //вектор боковой тяги
        //     _thrustForce.force = 
        //         (thrustForceVector.normalized) * (ShipsBrain.GetThrustSpeed() + ShipsBrain.GetStraightManeurSpeed()) +
        //         (maneurForceVector.normalized) * ShipsBrain.GetSideManeurSpeed();
        //     _thrustForce.torque = new Vector3(0, ShipsBrain.GetShipAngle(), 0);
        // }
        //
        // [ServerRpc]
        // public void AnimateMovementServerRpc()
        // {
        //     Debug.unityLogger.Log($"Gonna animate movement of {gameObject.name}");
        //     #region Reset movement animation
        //     
        //     _te.Stop();
        //     _tlm.Stop();
        //     _trm.Stop();
        //     _brm.Stop();
        //     _blm.Stop();
        //     
        //     #endregion
        //     
        //     var engines = ShipsBrain.getMovement();
        //     if (engines.Thrust)
        //     {
        //         _te.Play(true);
        //     }
        //
        //     if (engines.TopRight)
        //     {
        //         _trm.Play(true);
        //     }
        //
        //     if (engines.TopLeft)
        //     {
        //         _tlm.Play(true);
        //     }
        //
        //     if (engines.BotLeft)
        //     {
        //         _blm.Play(true);
        //     }
        //
        //     if (engines.BotRight)
        //     {
        //         _brm.Play(true);
        //     }
        //     
        //     AnimateMovementClientRpc();
        // }
        //
        // [ClientRpc]
        // public void AnimateMovementClientRpc()
        // {
        //     #region Reset movement animation
        //     
        //     _te.Stop();
        //     _tlm.Stop();
        //     _trm.Stop();
        //     _brm.Stop();
        //     _blm.Stop();
        //     
        //     #endregion
        //     
        //     var engines = ShipsBrain.getMovement();
        //     if (engines.Thrust)
        //     {
        //         _te.Play(true);
        //     }
        //
        //     if (engines.TopRight)
        //     {
        //         _trm.Play(true);
        //     }
        //
        //     if (engines.TopLeft)
        //     {
        //         _tlm.Play(true);
        //     }
        //
        //     if (engines.BotLeft)
        //     {
        //         _blm.Play(true);
        //     }
        //
        //     if (engines.BotRight)
        //     {
        //         _brm.Play(true);
        //     }
        // }
    }
}
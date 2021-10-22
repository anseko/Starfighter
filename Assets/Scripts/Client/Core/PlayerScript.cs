using System;
using Core;
using MLAPI.NetworkVariable;
using ScriptableObjects;
using UnityEngine;

namespace Client.Core
{
    public class PlayerScript : UnitScript
    {
        public KeyConfig keyConfig;
        public NetworkVariableVector3 shipSpeed, shipRotation;
        public UnitStateMachine unitStateMachine;
        public bool localUsage = false;
        public Rigidbody rigidbody;

        private void Awake()
        {
            shipSpeed = new NetworkVariableVector3(new NetworkVariableSettings(){WritePermission = NetworkVariablePermission.OwnerOnly}, Vector3.zero);
            shipRotation = new NetworkVariableVector3(new NetworkVariableSettings(){WritePermission = NetworkVariablePermission.OwnerOnly}, Vector3.zero);
        }

        private void Start()
        {
            if (localUsage)
            {
                unitConfig = Resources.Load<SpaceShipConfig>(Constants.PathToShipsObjects + "SpaceShipConfig");
                GetComponent<ClientInitManager>().InitPilot(this);
            }
            
            unitStateMachine = new UnitStateMachine(gameObject, (unitConfig as SpaceShipConfig).shipState);
            Debug.unityLogger.Log($"PS {(unitConfig as SpaceShipConfig).shipState}");

            
            rigidbody = GetComponent<Rigidbody>();
        }
        
        public override UnitState GetState()
        {
            return unitStateMachine.currentState.State;
        }
        
        private void FixedUpdate()
        {
            unitStateMachine.Update();
        }
    }
}
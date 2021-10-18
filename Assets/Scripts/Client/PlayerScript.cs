using Client.Core;
using Client.UI;
using Core;
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
        public Rigidbody rigidbody;
        
        private void Start()
        {
            if (localUsage)
            {
                unitConfig = Resources.Load<SpaceShipConfig>(Constants.PathToShipsObjects + "SpaceShipConfig");
                GetComponent<ClientInitManager>().InitPilot(this);
            }
            
            unitStateMachine = new UnitStateMachine(gameObject, (unitConfig as SpaceShipConfig).shipState);
            dockingTrigger.Init(this);
            Debug.unityLogger.Log($"PS {(unitConfig as SpaceShipConfig).shipState}");

            shipSpeed = Vector3.zero;
            shipRotation = Vector3.zero;
            rigidbody = GetComponent<Rigidbody>();
        }
        
        public UnitState GetState()
        {
            return unitStateMachine.currentState.State;
        }
        
        private void FixedUpdate()
        {
            unitStateMachine.Update();
        }
    }
}
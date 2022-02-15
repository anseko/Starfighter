using Core;
using Core.Models;
using MLAPI.NetworkVariable;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Rendering;

namespace Client.Core
{
    public class PlayerScript : UnitScript
    {
        public Volume volume;
        public KeyConfig keyConfig;
        public NetworkVariableVector3 shipSpeed, shipRotation;
        public UnitStateMachine unitStateMachine;
        public bool localUsage = false;
        public Rigidbody Rigidbody;
        public float FOVRadius;
        public SpaceShipDto ShipConfig
        {
            get => unitConfig.Value;
            set => unitConfig.Value = value;
        }
        
        private void Awake()
        {
            shipSpeed = new NetworkVariableVector3(new NetworkVariableSettings()
            {
                WritePermission = NetworkVariablePermission.Custom,
                WritePermissionCallback = id => IsOwner || IsServer
            }, Vector3.zero);
            
            shipRotation = new NetworkVariableVector3(new NetworkVariableSettings(){ 
                WritePermission = NetworkVariablePermission.Custom,
                WritePermissionCallback = id => IsOwner || IsServer
            }, Vector3.zero);
        }

        private void Start()
        {
            #if UNITY_EDITOR
                if (localUsage)
                {
                    ShipConfig = new SpaceShipDto(Resources.Load<SpaceShipConfig>(Constants.PathToShipsObjects + "SpaceShipConfig"));
                    // GetComponent<ClientInitManager>().InitPilot(this);
                    MLAPI.NetworkManager.Singleton.StartHost();
                }
            #endif
            
            unitStateMachine = new UnitStateMachine(gameObject, ShipConfig.shipState);
            
            unitConfig.OnValueChanged += (value, newValue) =>
            {
                if (newValue.currentHp <= 0)
                {
                    unitStateMachine.ChangeState(UnitState.IsDead);
                }
            };
            
            Debug.unityLogger.Log($"PS {ShipConfig.shipState}");
            volume = FindObjectOfType<Volume>(true);
            Rigidbody = GetComponent<Rigidbody>();
        }
        
        public override UnitState GetState()
        {
            return unitStateMachine.currentState.State;
        }
        
        private void Update()
        {
            unitStateMachine.Update();
        }
    }
}
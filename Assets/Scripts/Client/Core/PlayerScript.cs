using Core;
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
        public NetworkVariable<float> currentStress;
        public UnitStateMachine unitStateMachine;
        public bool localUsage = false;
        public Rigidbody rigidbody;
        public float FOVRadius;

        private void Awake()
        {
            shipSpeed = new NetworkVariableVector3(new NetworkVariableSettings()
            {
                WritePermission = NetworkVariablePermission.Custom,
                WritePermissionCallback = id => { return IsOwner || IsServer; }
            }, Vector3.zero);
            
            shipRotation = new NetworkVariableVector3(new NetworkVariableSettings(){ 
                WritePermission = NetworkVariablePermission.Custom,
                WritePermissionCallback = id => { return IsOwner || IsServer; } 
            }, Vector3.zero);
            
            currentStress = new NetworkVariable<float>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.ServerOnly
            });
            

            if (!localUsage)
            {
                NetworkManager.OnServerStarted += () =>
                {
                    if (IsServer)
                    {
                        currentStress.Value = ((SpaceShipConfig) unitConfig).currentStress;
                        currentHp.Value = ((SpaceShipConfig) unitConfig).currentHp;
                    }
                };
            }
        }

        private void Start()
        {
            #if UNITY_EDITOR
                if (localUsage)
                {
                    unitConfig = Resources.Load<SpaceShipConfig>(Constants.PathToShipsObjects + "SpaceShipConfig");
                    // GetComponent<ClientInitManager>().InitPilot(this);
                    MLAPI.NetworkManager.Singleton.StartHost();
                }
            #endif
            
            unitStateMachine = new UnitStateMachine(gameObject, ((SpaceShipConfig) unitConfig).shipState);
            
            currentHp.OnValueChanged += (value, newValue) =>
            {
                if (newValue <= 0)
                {
                    unitStateMachine.ChangeState(UnitState.IsDead);
                }
            };
            
            Debug.unityLogger.Log($"PS {((SpaceShipConfig) unitConfig).shipState}");
            volume = FindObjectOfType<Volume>(true);
            rigidbody = GetComponent<Rigidbody>();
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
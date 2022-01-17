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
        public NetworkVariable<float> currentStress;
        public UnitStateMachine unitStateMachine;
        public bool localUsage = false;
        public Rigidbody rigidbody;
        public float FOVRadius;

        private void Awake()
        {
            shipSpeed = new NetworkVariableVector3(new NetworkVariableSettings(){WritePermission = NetworkVariablePermission.OwnerOnly}, Vector3.zero);
            shipRotation = new NetworkVariableVector3(new NetworkVariableSettings(){WritePermission = NetworkVariablePermission.OwnerOnly}, Vector3.zero);
            currentStress = new NetworkVariable<float>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.ServerOnly
            });
            
            currentHp.OnValueChanged += (value, newValue) =>
            {
                if (currentHp.Value <= 0)
                {
                    currentHp.Value = 0;
                    unitStateMachine.ChangeState(UnitState.IsDead);
                }
                else
                {
                    unitStateMachine.ChangeState(UnitState.InFlight);
                }
            }; 
            
            
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
            Debug.unityLogger.Log($"PS {((SpaceShipConfig) unitConfig).shipState}");

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
using System;
using System.Linq;
using Core;
using MLAPI;
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
        public NetworkVariableColor baseColor;
        public NetworkVariableInt shipNumber;
        public UnitStateMachine unitStateMachine;
        public NetworkVariable<UnitState> currentState;
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
            
            currentState = new NetworkVariable<UnitState>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.ServerOnly
            });
            
            baseColor = new NetworkVariableColor(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.ServerOnly
            });
            
            shipNumber = new NetworkVariableInt(new NetworkVariableSettings()
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
                        currentState.Value = ((SpaceShipConfig)unitConfig).shipState;
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
            
            volume = FindObjectOfType<Volume>(true);
            rigidbody = GetComponent<Rigidbody>();
            
            unitStateMachine = new UnitStateMachine(gameObject, ((SpaceShipConfig) unitConfig).shipState);
            currentHp.OnValueChanged += (value, newValue) =>
            {
                if (!IsServer) return;
                
                if (newValue <= 0)
                {
                    currentState.Value = UnitState.IsDead;
                    return;
                }
                
                if (newValue > 0)
                {
                    Debug.unityLogger.Log("Trying to resurrect self");
                    currentState.Value = UnitState.InFlight;
                }
            };

            currentState.OnValueChanged += (state, newState) =>
            {
                unitStateMachine.ChangeState(newState);
            };

            Debug.unityLogger.Log($"PS {((SpaceShipConfig) unitConfig).shipState} | currentState:{currentState.Value}");
            if(IsClient) unitStateMachine.ChangeState(currentState.Value);
            
            transform.GetComponentsInChildren<MeshRenderer>().ToList()
                .Where(x => x.gameObject.name == "ShipModel").ToList().ForEach(x => x.sharedMaterial.color = baseColor.Value);
            GetComponentsInChildren<TextMesh>().ToList().ForEach(t => t.text = shipNumber.Value.ToString());
        }
        
        public override UnitState GetState()
        {
            return currentState.Value;
        }
        
        private void Update()
        {
            unitStateMachine.Update();
        }
    }
}
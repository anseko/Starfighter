using Core;
using Core.Models;
using System;
using System.Linq;
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
                
            FOVRadius = 1080;
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
            
            volume = FindObjectOfType<Volume>(true);
            Rigidbody = GetComponent<Rigidbody>();
            
            unitConfig.OnValueChanged += (value, newValue) =>
            {
                Debug.unityLogger.Log("HERE");
                if (newValue.currentHp <= 0 || newValue.currentStress >= ShipConfig.maxStress)
                {
                    ShipConfig.shipState = UnitState.IsDead;
                    unitStateMachine.ChangeState(UnitState.IsDead);
                    return;
                }

                if (newValue.currentHp > 0 &&
                    newValue.currentStress < ShipConfig.maxStress &&
                    ShipConfig.shipState == UnitState.IsDead)
                {
                    ShipConfig.shipState = UnitState.InFlight;
                    unitStateMachine.ChangeState(UnitState.InFlight);
                    return;
                }
                
                if (newValue.shipState != value.shipState)
                {
                    unitStateMachine.ChangeState(newValue.shipState);
                }   
            };

            Debug.unityLogger.Log($"PS {ShipConfig.shipState}");
            
            if (IsClient) unitStateMachine.ChangeState(ShipConfig.shipState);
            
            transform.GetComponentsInChildren<MeshRenderer>().ToList()
                .Where(x => x.gameObject.name == "ShipModel").ToList().ForEach(x => x.sharedMaterial.color = ShipConfig.baseColor);
            // GetComponentsInChildren<TextMesh>().ToList().ForEach(t => t.text = shipNumber.Value.ToString());
        }

        public override UnitState GetState() => ShipConfig.shipState;

        private void Update()
        {
            unitStateMachine.Update();
        }
    }
}
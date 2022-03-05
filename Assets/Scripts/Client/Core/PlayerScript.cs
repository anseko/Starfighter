using Core;
using Core.Models;
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

        
        private new void Awake()
        {
            base.Awake();
            
            shipSpeed = new NetworkVariableVector3(new NetworkVariableSettings()
            {
                WritePermission = NetworkVariablePermission.Custom,
                WritePermissionCallback = id => IsOwner || IsServer
            }, Vector3.zero);
            
            shipRotation = new NetworkVariableVector3(new NetworkVariableSettings(){ 
                WritePermission = NetworkVariablePermission.Custom,
                WritePermissionCallback = id => IsOwner || IsServer
            }, Vector3.zero);
                
            FOVRadius = NetworkUnitConfig.RadarRange * NetworkUnitConfig.RadarRangeCoefficient;
        }

        private void Start()
        {
            #if UNITY_EDITOR
                if (localUsage)
                {
                    var dto = new SpaceUnitDto(
                        Resources.Load<SpaceShipConfig>(Constants.PathToShipsObjects + "SpaceunitConfig.Value"));
                    // NetworkUnitConfig = new NetworkSpaceUnitDto(dto, id => IsOwner || IsServer);
                    // GetComponent<ClientInitManager>().InitPilot(this);
                    NetworkManager.Singleton.StartHost();
                }
            #endif
            unitStateMachine = new UnitStateMachine(gameObject, NetworkUnitConfig.ShipState);
            
            volume = FindObjectOfType<Volume>(true);
            Rigidbody = GetComponent<Rigidbody>();
            
            NetworkUnitConfig._shipState.OnValueChanged += (value, newValue) =>
            {
                unitStateMachine.ChangeState(newValue);
            }; 
            
            //
            // NetworkUnitConfig.OnValueChanged += (value, newValue) =>
            // {
            //     Debug.unityLogger.Log("HERE");
            //     if (newValue.currentHp <= 0 || newValue.currentStress >= NetworkUnitConfig.Value.maxStress)
            //     {
            //         
            //         unitStateMachine.ChangeState(UnitState.IsDead);
            //         return;
            //     }
            //
            //     if (newValue.currentHp > 0 &&
            //         newValue.currentStress < NetworkUnitConfig.MaxStress &&
            //         NetworkUnitConfig.ShipState == UnitState.IsDead)
            //     {
            //         NetworkUnitConfig.ShipState = UnitState.InFlight;
            //         unitStateMachine.ChangeState(UnitState.InFlight);
            //         return;
            //     }
            //     
            // };

            Debug.unityLogger.Log($"PS {NetworkUnitConfig.ShipState}");
            
            if (IsClient) unitStateMachine.ChangeState(NetworkUnitConfig.ShipState);
            
            transform.GetComponentsInChildren<MeshRenderer>().ToList()
                .Where(x => x.gameObject.name == "ShipModel").ToList().ForEach(x => x.sharedMaterial.color = NetworkUnitConfig.BaseColor);
            // GetComponentsInChildren<TextMesh>().ToList().ForEach(t => t.text = shipNumber.Value.ToString());
        }

        public override UnitState GetState() => NetworkUnitConfig.ShipState;

        private void Update()
        {
            unitStateMachine.Update();
        }
    }
}
using System.Linq;
using Core;
using Mirror;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Rendering;

namespace Client.Core
{
    public class PlayerScript : UnitScript
    {
        public Volume volume;
        public KeyConfig keyConfig;
        [SyncVar(hook = nameof(OnShipSpeedChange))] public Vector3 shipSpeed;
        [SyncVar] public Vector3 shipRotation;
        public UnitStateMachine unitStateMachine;
        public bool localUsage = false;
        public Rigidbody Rigidbody;

        private void OnShipSpeedChange(Vector3 oldValue, Vector3 newValue)
        {
            ClientEventStorage.GetInstance().OnShipSpeedChange.Invoke(newValue);
        }

        private new void Awake()
        {
            base.Awake();
            
            // shipSpeed = new NetworkVariableVector3(new NetworkVariableSettings()
            // {
            //     WritePermission = NetworkVariablePermission.Custom,
            //     WritePermissionCallback = id => IsOwner || IsServer
            // }, Vector3.zero);
            //
            // shipRotation = new NetworkVariableVector3(new NetworkVariableSettings(){ 
            //     WritePermission = NetworkVariablePermission.Custom,
            //     WritePermissionCallback = id => IsOwner || IsServer
            // }, Vector3.zero);
        }

        private void Start()
        {
            #if UNITY_EDITOR
                if (localUsage)
                {
                  NetworkManager.singleton.StartHost();
                }
            #endif
            
            volume = FindObjectOfType<Volume>(true);
            Rigidbody = GetComponent<Rigidbody>();
            
            unitStateMachine = new UnitStateMachine(gameObject, networkUnitConfig.shipState);
            
            ClientEventStorage.GetInstance().OnShipStateChange.AddListener((oldState, newState) =>
            {
                unitStateMachine.ChangeState(newState);
            });

            Debug.unityLogger.Log($"PS {networkUnitConfig.shipState}");
            
            if (isClient) unitStateMachine.ChangeState(networkUnitConfig.shipState);
            
            transform.GetComponentsInChildren<MeshRenderer>().ToList()
                .Where(x => x.gameObject.name == "ShipModel").ToList().ForEach(x => x.sharedMaterial.color = networkUnitConfig.baseColor);
            
            GetComponentsInChildren<TextMesh>().ToList().ForEach(t =>
            {
                int.TryParse(networkUnitConfig.shipId.Replace("ship", ""), out var num);
                var shipNumber = num == 10 ? "X" : num.ToString();
                t.text = shipNumber;
            });
        }

        public override UnitState GetState() => networkUnitConfig.shipState;

        private void Update()
        {
            unitStateMachine.Update();
        }
    }
}
using Core;
using System.Linq;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

namespace Client.Core
{
    public class PlayerScript : UnitScript
    {
        public Volume volume;
        public KeyConfig keyConfig;
        public NetworkVariable<Vector3> shipSpeed, shipRotation;
        public UnitStateMachine unitStateMachine;
        public bool localUsage = false;
        public Rigidbody Rigidbody;


        private new void Awake()
        {
            base.Awake();
            
            shipSpeed = new NetworkVariable<Vector3>(Vector3.zero);

            shipRotation = new NetworkVariable<Vector3>(Vector3.zero);
        }

        private void Start()
        {
            #if UNITY_EDITOR
                if (localUsage)
                {
                  NetworkManager.Singleton.StartHost();
                }
            #endif
            
            volume = FindObjectOfType<Volume>(true);
            Rigidbody = GetComponent<Rigidbody>();
            
            unitStateMachine = new UnitStateMachine(gameObject, NetworkUnitConfig.ShipState);
            NetworkUnitConfig._shipState.OnValueChanged += (value, newValue) =>
            {
                unitStateMachine.ChangeState(newValue);
            };

            Debug.unityLogger.Log($"PS {NetworkUnitConfig.ShipState}");
            
            if (IsClient) unitStateMachine.ChangeState(NetworkUnitConfig.ShipState);
            
            transform.GetComponentsInChildren<MeshRenderer>().ToList()
                .Where(x => x.gameObject.name == "ShipModel").ToList().ForEach(x => x.sharedMaterial.color = NetworkUnitConfig.BaseColor);
            
            GetComponentsInChildren<TextMesh>().ToList().ForEach(t =>
            {
                int.TryParse(NetworkUnitConfig.ShipId.Replace("ship", ""), out var num);
                var shipNumber = num == 10 ? "X" : num.ToString();
                t.text = shipNumber;
            });
        }

        public override UnitState GetState() => NetworkUnitConfig.ShipState;

        private void Update()
        {
            unitStateMachine.Update();
        }
    }
}
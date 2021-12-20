using MLAPI;
using MLAPI.NetworkVariable;
using Net.Components;
using UnityEngine;

namespace Net
{
    public class DangerZone: NetworkBehaviour
    {
        public NetworkVariable<Color> zoneColor;
        public NetworkVariable<float> zoneDamage;
        public NetworkVariable<float> zoneRadius;

        private void Awake()
        {
            zoneColor = new NetworkVariable<Color>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.ServerOnly
            });
            zoneRadius = new NetworkVariable<float>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.ServerOnly
            });
            zoneDamage = new NetworkVariable<float>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.ServerOnly
            });
        }

        private void Start()
        {
            var renderer = GetComponent<SpriteRenderer>() ?? gameObject.AddComponent<SpriteRenderer>();
            renderer.color = zoneColor.Value;
            transform.localScale *= zoneRadius.Value;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer) return;
            if (other.gameObject.TryGetComponent<StressComponent>(out var stressComponent))
            {
                stressComponent.stressDelta.Value += zoneDamage.Value;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsServer) return;
            if (other.gameObject.TryGetComponent<StressComponent>(out var stressComponent))
            {
                stressComponent.stressDelta.Value -= zoneDamage.Value;
            }
        }
    }
}
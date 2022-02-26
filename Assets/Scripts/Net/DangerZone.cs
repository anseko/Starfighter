using System;
using MLAPI;
using MLAPI.NetworkVariable;
using Net.Components;
using UnityEngine;

namespace Net
{
    public class DangerZone: NetworkBehaviour
    {
        public NetworkVariable<Color> zoneColor;
        public NetworkVariable<float> zoneStressDamage;
        public NetworkVariable<float> zoneHpDamage;
        public NetworkVariable<float> zoneRadius;
        public Guid id;

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
            zoneStressDamage = new NetworkVariable<float>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.ServerOnly
            });
            zoneHpDamage = new NetworkVariable<float>(new NetworkVariableSettings()
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
            if (IsClient) return;
            if (other.gameObject.TryGetComponent<StressComponent>(out var stressComponent))
            {
                stressComponent.stressDelta.Value += zoneStressDamage.Value;
            }
            if (other.gameObject.TryGetComponent<HealthComponent>(out var hpComponent))
            {
                hpComponent.hpDelta.Value += zoneHpDamage.Value;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsServer) return;
            if (other.gameObject.TryGetComponent<StressComponent>(out var stressComponent))
            {
                stressComponent.stressDelta.Value -= zoneStressDamage.Value;
            }
            if (other.gameObject.TryGetComponent<HealthComponent>(out var hpComponent))
            {
                hpComponent.hpDelta.Value -= zoneHpDamage.Value;
            }
        }
    }
}
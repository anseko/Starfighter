using System;
using Core;
using Core.Models;
using MLAPI;
using MLAPI.NetworkVariable;
using Net.Components;
using Net.Core;
using ScriptableObjects;
using UnityEngine;

namespace Net
{
    public class DangerZone: NetworkBehaviour
    {
        public NetworkVariable<Color> zoneColor;
        public NetworkVariable<float> zoneStressDamage;
        public NetworkVariable<float> zoneHpDamage;
        public NetworkVariable<float> zoneRadius;
        public NetworkVariable<ZoneType> zoneType;
        private NetworkVariable<string> _id;

        public Guid Guid
        {
            get { return Guid.Parse(_id.Value); }
            set { _id.Value = value.ToString(); }
        }


        public DangerZoneDto Export()
        {
            var dto = new DangerZoneDto()
            {
                center = transform.position,
                radius = zoneRadius.Value,
                color = zoneColor.Value,
                hpDamage = zoneHpDamage.Value,
                stressDamage = zoneStressDamage.Value,
                id = Guid,
                type = zoneType.Value
            };
            return dto;
        }
        
        private void Awake()
        {
            var permissionDelegate =
                new NetworkVariablePermissionsDelegate(id =>
                    IsOwner || IsServer || FindObjectOfType<ConnectionHelper>().userType.Value == UserType.Admin);
            
            zoneColor = new NetworkVariable<Color>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.Custom,
                WritePermissionCallback = permissionDelegate
            });
            zoneRadius = new NetworkVariable<float>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.Custom,
                WritePermissionCallback = permissionDelegate
            });
            zoneStressDamage = new NetworkVariable<float>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.Custom,
                WritePermissionCallback = permissionDelegate
            });
            zoneHpDamage = new NetworkVariable<float>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.Custom,
                WritePermissionCallback = permissionDelegate
            });
            _id = new NetworkVariable<string>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.Custom,
                WritePermissionCallback = permissionDelegate
            });
            zoneType = new NetworkVariable<ZoneType>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.Custom,
                WritePermissionCallback = permissionDelegate
            });

            zoneRadius.OnValueChanged += (value, newValue) => { transform.localScale = Vector3.one * 10 * newValue; };
            zoneColor.OnValueChanged += (value, newValue) =>
            {
                var renderer = GetComponent<SpriteRenderer>() ?? gameObject.AddComponent<SpriteRenderer>();
                renderer.color = newValue;
            };
        }

        private void Start()
        {
            var renderer = GetComponent<SpriteRenderer>() ?? gameObject.AddComponent<SpriteRenderer>();
            renderer.color = zoneColor.Value;
            transform.localScale = Vector3.one * 10 * zoneRadius.Value;
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
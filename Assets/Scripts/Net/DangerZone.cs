using System;
using Client.Core;
using Core;
using Core.Models;
using Mirror;
using Net.Components;
using UnityEngine;

namespace Net
{
    public class DangerZone: NetworkBehaviour
    {
        [SyncVar(hook = nameof(ZoneColorChanged))]
        public Color zoneColor;
        [SyncVar]
        public float zoneStressDamage;
        [SyncVar]
        public float zoneHpDamage;
        [SyncVar(hook = nameof(ZoneRadiusChanged))]
        public float zoneRadius;
        [SyncVar]
        public ZoneType zoneType;
        [SyncVar]
        private string _id;

        public Guid Guid
        {
            get => Guid.Parse(_id);
            set => _id = value.ToString();
        }


        public DangerZoneDto Export()
        {
            var dto = new DangerZoneDto()
            {
                center = transform.position,
                radius = zoneRadius,
                color = zoneColor,
                hpDamage = zoneHpDamage,
                stressDamage = zoneStressDamage,
                id = Guid,
                type = zoneType
            };
            return dto;
        }
        
        private void Awake()
        {
            // var permissionDelegate =
            //     new NetworkVariablePermissionsDelegate(id =>
            //         isOwned || isServer || FindObjectOfType<ConnectionHelper>().userType == UserType.Admin);
            
            // zoneColor = new NetworkVariable<Color>(new NetworkVariableSettings()
            // {
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermission = NetworkVariablePermission.Custom,
            //     WritePermissionCallback = permissionDelegate
            // });
            // zoneRadius = new NetworkVariable<float>(new NetworkVariableSettings()
            // {
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermission = NetworkVariablePermission.Custom,
            //     WritePermissionCallback = permissionDelegate
            // });
            // zoneStressDamage = new NetworkVariable<float>(new NetworkVariableSettings()
            // {
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermission = NetworkVariablePermission.Custom,
            //     WritePermissionCallback = permissionDelegate
            // });
            // zoneHpDamage = new NetworkVariable<float>(new NetworkVariableSettings()
            // {
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermission = NetworkVariablePermission.Custom,
            //     WritePermissionCallback = permissionDelegate
            // });
            // _id = new NetworkVariable<string>(new NetworkVariableSettings()
            // {
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermission = NetworkVariablePermission.Custom,
            //     WritePermissionCallback = permissionDelegate
            // });
            // zoneType = new NetworkVariable<ZoneType>(new NetworkVariableSettings()
            // {
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermission = NetworkVariablePermission.Custom,
            //     WritePermissionCallback = permissionDelegate
            // });
        }

        private void ZoneColorChanged(Color oldValue, Color newValue)
        {
            var renderer = GetComponent<SpriteRenderer>() ?? gameObject.AddComponent<SpriteRenderer>();
            renderer.color = newValue;
        }

        private void ZoneRadiusChanged(float oldValue, float newValue)
        {
            transform.localScale = Vector3.one * 10 * newValue;
            ClientEventStorage.GetInstance().OnDangerZoneRadiusChange.Invoke(newValue);
        } 

        private void Start()
        {
            var renderer = GetComponent<SpriteRenderer>() ?? gameObject.AddComponent<SpriteRenderer>();
            renderer.color = zoneColor;
            transform.localScale = Vector3.one * 10 * zoneRadius;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (isClient) return;
            if (other.gameObject.TryGetComponent<StressComponent>(out var stressComponent))
            {
                stressComponent.stressDelta += zoneStressDamage;
            }
            if (other.gameObject.TryGetComponent<HealthComponent>(out var hpComponent))
            {
                hpComponent.hpDelta += zoneHpDamage;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!isServer) return;
            if (other.gameObject.TryGetComponent<StressComponent>(out var stressComponent))
            {
                stressComponent.stressDelta -= zoneStressDamage;
            }
            if (other.gameObject.TryGetComponent<HealthComponent>(out var hpComponent))
            {
                hpComponent.hpDelta -= zoneHpDamage;
            }
        }
    }
}
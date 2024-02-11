using System;
using System.Linq;
using Mirror;
using Net.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Models
{
    [Serializable]
    public class NetworkSpaceUnitDto: NetworkBehaviour
    {
        [SyncVar] public float maxAngleSpeed;
        [SyncVar] public float maxSpeed;
        [SyncVar] public float maxHp;
        [SyncVar] public float currentHp;
        [SyncVar] public bool isDockable;
        [SyncVar] public bool isMovable;
        [SyncVar] public Vector3 position;
        [SyncVar] public Quaternion rotation;
        [SyncVar] public string prefabName;
        [SyncVar] public string id;
        [SyncVar] public float maxStress;
        [SyncVar] public float currentStress;
        [SyncVar] public string shipId;
        [SyncVar] public UnitState shipState;
        [SyncVar] public Color baseColor;
        [SyncVar] public float acceleration;
        [SyncVar] public float radarRange;
        [SyncVar] public float accelerationCoefficient;
        [SyncVar] public float physResistanceCoefficient;
        [SyncVar] public float radResistanceCoefficient;
        [SyncVar] public float radarRangeCoefficient;

        public void Init(SpaceUnitDto config)
        {
            foreach (var dtoField in typeof(NetworkSpaceUnitDto).GetFields())
            {
                var value = typeof(SpaceUnitDto).GetProperties().FirstOrDefault(x =>
                    string.Equals(x.Name, dtoField.Name, StringComparison.CurrentCultureIgnoreCase));
                dtoField.SetValue(this, value?.GetValue(config));
            }
        }

        public SpaceUnitDto Export()
        {
            var dto = new SpaceUnitDto();

            foreach (var dtoField in typeof(SpaceUnitDto).GetFields())
            {
                var value = typeof(NetworkSpaceUnitDto).GetProperties().FirstOrDefault(x =>
                    string.Equals(x.Name, dtoField.Name, StringComparison.CurrentCultureIgnoreCase));
                dtoField.SetValue(dto, value?.GetValue(this));
            }

            return dto;
        }
        
        public void Awake()
        {
            // var permissionDelegate =
            //     new NetworkVariablePermissionsDelegate(id =>
            //         IsOwner ||
            //         IsServer ||
            //         FindObjectOfType<ConnectionHelper>().userType.Value == UserType.Admin ||
            //         FindObjectOfType<ConnectionHelper>().userType.Value == UserType.Mechanic);
            //
            // _maxAngleSpeed = new NetworkVariable<float>(new NetworkVariableSettings()
            // {
            //     WritePermission = NetworkVariablePermission.Custom,
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermissionCallback = permissionDelegate
            // });
            //
            // _maxSpeed = new NetworkVariable<float>(new NetworkVariableSettings()
            // {
            //     WritePermission = NetworkVariablePermission.Custom,
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermissionCallback = permissionDelegate
            // });
            //
            // _maxHp = new NetworkVariable<float>(new NetworkVariableSettings()
            // {
            //     WritePermission = NetworkVariablePermission.Custom,
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermissionCallback = permissionDelegate
            // });
            //
            // _currentHp = new NetworkVariable<float>(new NetworkVariableSettings()
            // {
            //     WritePermission = NetworkVariablePermission.Custom,
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermissionCallback = permissionDelegate
            // });
            //
            // _isDockable = new NetworkVariable<bool>(new NetworkVariableSettings()
            // {
            //     WritePermission = NetworkVariablePermission.Custom,
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermissionCallback = permissionDelegate
            // });
            //
            // _isMovable = new NetworkVariable<bool>(new NetworkVariableSettings()
            // {
            //     WritePermission = NetworkVariablePermission.Custom,
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermissionCallback = permissionDelegate
            // });
            //
            // _position = new NetworkVariable<Vector3>(new NetworkVariableSettings()
            // {
            //     WritePermission = NetworkVariablePermission.Custom,
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermissionCallback = permissionDelegate
            // });
            //
            // _rotation = new NetworkVariable<Quaternion>(new NetworkVariableSettings()
            // {
            //     WritePermission = NetworkVariablePermission.Custom,
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermissionCallback = permissionDelegate
            // });
            //
            // _prefabName = new NetworkVariable<string>(new NetworkVariableSettings()
            // {
            //     WritePermission = NetworkVariablePermission.Custom,
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermissionCallback = permissionDelegate
            // });
            //
            // _maxStress = new NetworkVariable<float>(new NetworkVariableSettings()
            // {
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermission = NetworkVariablePermission.Custom,
            //     WritePermissionCallback = permissionDelegate
            // });
            //
            // _currentStress = new NetworkVariable<float>(new NetworkVariableSettings()
            // {
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermission = NetworkVariablePermission.Custom,
            //     WritePermissionCallback = permissionDelegate
            // });
            //
            // _shipId = new NetworkVariable<string>(new NetworkVariableSettings()
            // {
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermission = NetworkVariablePermission.Custom,
            //     WritePermissionCallback = permissionDelegate
            // });
            //
            // _shipState = new NetworkVariable<UnitState>(new NetworkVariableSettings()
            // {
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermission = NetworkVariablePermission.Custom,
            //     WritePermissionCallback = permissionDelegate
            // });
            //
            // _baseColor = new NetworkVariable<Color>(new NetworkVariableSettings()
            // {
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermission = NetworkVariablePermission.Custom,
            //     WritePermissionCallback = permissionDelegate
            // });
            //
            // _id = new NetworkVariable<string>(new NetworkVariableSettings()
            // {
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermission = NetworkVariablePermission.Custom,
            //     WritePermissionCallback = permissionDelegate
            // });
            //
            // _acceleration = new NetworkVariable<float>(new NetworkVariableSettings()
            // {
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermission = NetworkVariablePermission.Custom,
            //     WritePermissionCallback = permissionDelegate
            // });
            //
            // _radarRange = new NetworkVariable<float>(new NetworkVariableSettings()
            // {
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermission = NetworkVariablePermission.Custom,
            //     WritePermissionCallback = permissionDelegate
            // });
            //
            // _accelerationCoefficient = new NetworkVariable<float>(new NetworkVariableSettings()
            // {
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermission = NetworkVariablePermission.Custom,
            //     WritePermissionCallback = permissionDelegate
            // });
            //
            // _physResistanceCoefficient = new NetworkVariable<float>(new NetworkVariableSettings()
            // {
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermission = NetworkVariablePermission.Custom,
            //     WritePermissionCallback = permissionDelegate
            // });
            //
            // _radResistanceCoefficient = new NetworkVariable<float>(new NetworkVariableSettings()
            // {
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermission = NetworkVariablePermission.Custom,
            //     WritePermissionCallback = permissionDelegate
            // });
            //
            // _radarRangeCoefficient = new NetworkVariable<float>(new NetworkVariableSettings()
            // {
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermission = NetworkVariablePermission.Custom,
            //     WritePermissionCallback = permissionDelegate
            // });
        }
    }
}
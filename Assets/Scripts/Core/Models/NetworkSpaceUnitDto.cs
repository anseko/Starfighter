using System;
using System.Linq;
using MLAPI;
using MLAPI.NetworkVariable;
using Net.Core;
using UnityEngine;

namespace Core.Models
{
    [Serializable]
    public class NetworkSpaceUnitDto: NetworkBehaviour
    {
        public NetworkVariable<float> _maxAngleSpeed;
        public NetworkVariable<float> _maxSpeed;
        public NetworkVariable<float> _maxHp;
        public NetworkVariable<float> _currentHp;
        public NetworkVariable<bool> _isDockable;
        public NetworkVariable<bool> _isMovable;
        public NetworkVariable<Vector3> _position;
        public NetworkVariable<Quaternion> _rotation;
        public NetworkVariable<string> _prefabName;
        public NetworkVariable<string> _id;
        public NetworkVariable<float> _maxStress;
        public NetworkVariable<float> _currentStress;
        public NetworkVariable<string> _shipId;
        public NetworkVariable<UnitState> _shipState;
        public NetworkVariable<Color> _baseColor;
        
        public float MaxAngleSpeed
        {
            get => _maxAngleSpeed.Value;
            set => _maxAngleSpeed.Value = value;
        }
        public float MaxSpeed
        {
            get => _maxSpeed.Value;
            set => _maxSpeed.Value = value;
        }
        public float MaxHp
        {
            get => _maxHp.Value;
            set => _maxHp.Value = value;
        }
        public float CurrentHp
        {
            get => _currentHp.Value;
            set => _currentHp.Value = value;
        }
        public bool IsDockable
        {
            get => _isDockable.Value;
            set => _isDockable.Value = value;
        }
        public bool IsMovable
        {
            get => _isMovable.Value;
            set => _isMovable.Value = value;
        }
        public Vector3 Position
        {
            get => _position.Value;
            set => _position.Value = value;
        }
        public Quaternion Rotation
        {
            get => _rotation.Value;
            set => _rotation.Value = value;
        }
        public string PrefabName
        {
            get => _prefabName.Value;
            set => _prefabName.Value = value;
        }
        public Guid ID
        {
            get => Guid.Parse(_id.Value);
            set => _id.Value = value.ToString();
        }
        public float MaxStress
        {
            get => _maxStress.Value;
            set => _maxStress.Value = value;
        }
        public float CurrentStress
        {
            get => _currentStress.Value;
            set => _currentStress.Value = value;
        }
        public string ShipId
        {
            get => _shipId.Value;
        }
        public UnitState ShipState
        {
            get => _shipState.Value;
            set => _shipState.Value = value;
        }
        public Color BaseColor
        {
            get => _baseColor.Value;
            set => _baseColor.Value = value;
        }

        public void Init(SpaceUnitDto config)
        {
            _maxAngleSpeed.Value = config.maxAngleSpeed;
            _maxSpeed.Value = config.maxSpeed;
            _maxHp.Value = config.maxHp;
            _currentHp.Value = config.currentHp;
            _isDockable.Value = config.isDockable;
            _isMovable.Value = config.isMovable;
            _position.Value = config.position;
            _rotation.Value = config.rotation;
            _prefabName.Value = config.prefabName;
            _id.Value = config.id.ToString();
            _maxStress.Value = config.maxStress;
            _currentStress.Value = config.currentStress;
            _shipId.Value = config.shipId;
            _shipState.Value = config.shipState;
            _baseColor.Value = config.baseColor;
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
            var permissionDelegate =
                new NetworkVariablePermissionsDelegate(id =>
                    IsOwner || IsServer || FindObjectOfType<ConnectionHelper>().userType.Value == UserType.Admin);

            _maxAngleSpeed = new NetworkVariable<float>(new NetworkVariableSettings()
            {
                WritePermission = NetworkVariablePermission.Custom,
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermissionCallback = permissionDelegate
            });
            
            _maxSpeed = new NetworkVariable<float>(new NetworkVariableSettings()
            {
                WritePermission = NetworkVariablePermission.Custom,
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermissionCallback = permissionDelegate
            });
            
            _maxHp = new NetworkVariable<float>(new NetworkVariableSettings()
            {
                WritePermission = NetworkVariablePermission.Custom,
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermissionCallback = permissionDelegate
            });
            
            _currentHp = new NetworkVariable<float>(new NetworkVariableSettings()
            {
                WritePermission = NetworkVariablePermission.Custom,
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermissionCallback = permissionDelegate
            });
            
            _isDockable = new NetworkVariable<bool>(new NetworkVariableSettings()
            {
                WritePermission = NetworkVariablePermission.Custom,
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermissionCallback = permissionDelegate
            });
            
            _isMovable = new NetworkVariable<bool>(new NetworkVariableSettings()
            {
                WritePermission = NetworkVariablePermission.Custom,
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermissionCallback = permissionDelegate
            });
            
            _position = new NetworkVariable<Vector3>(new NetworkVariableSettings()
            {
                WritePermission = NetworkVariablePermission.Custom,
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermissionCallback = permissionDelegate
            });
            
            _rotation = new NetworkVariable<Quaternion>(new NetworkVariableSettings()
            {
                WritePermission = NetworkVariablePermission.Custom,
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermissionCallback = permissionDelegate
            });
            
            _prefabName = new NetworkVariable<string>(new NetworkVariableSettings()
            {
                WritePermission = NetworkVariablePermission.Custom,
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermissionCallback = permissionDelegate
            });

            _maxStress = new NetworkVariable<float>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.Custom,
                WritePermissionCallback = permissionDelegate
            });
            
            _currentStress = new NetworkVariable<float>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.Custom,
                WritePermissionCallback = permissionDelegate
            });
            
            _shipId = new NetworkVariable<string>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.Custom,
                WritePermissionCallback = permissionDelegate
            });
            
            _shipState = new NetworkVariable<UnitState>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.Custom,
                WritePermissionCallback = permissionDelegate
            });
            
            _baseColor = new NetworkVariable<Color>(new NetworkVariableSettings()
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
        }
    }
}
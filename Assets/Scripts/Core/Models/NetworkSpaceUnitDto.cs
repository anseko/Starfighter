using System;
using MLAPI;
using MLAPI.NetworkVariable;
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
        public Guid _id;
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
            get => _id;
            set => _id = value;
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
            _id = config.id;
            _maxStress.Value = config.maxStress;
            _currentStress.Value = config.currentStress;
            _shipId.Value = config.shipId;
            _shipState.Value = config.shipState;
            _baseColor.Value = config.baseColor;
        }
        
        public void Awake()
        {
            _maxAngleSpeed = new NetworkVariable<float>(new NetworkVariableSettings()
            {
                WritePermission = NetworkVariablePermission.Custom,
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermissionCallback = id => IsOwner || IsServer
            });
            
            _maxSpeed = new NetworkVariable<float>(new NetworkVariableSettings()
            {
                WritePermission = NetworkVariablePermission.Custom,
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermissionCallback = id => IsOwner || IsServer
            });
            
            _maxHp = new NetworkVariable<float>(new NetworkVariableSettings()
            {
                WritePermission = NetworkVariablePermission.Custom,
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermissionCallback = id => IsOwner || IsServer
            });
            
            _currentHp = new NetworkVariable<float>(new NetworkVariableSettings()
            {
                WritePermission = NetworkVariablePermission.Custom,
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermissionCallback = id => IsOwner || IsServer
            });
            
            _isDockable = new NetworkVariable<bool>(new NetworkVariableSettings()
            {
                WritePermission = NetworkVariablePermission.Custom,
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermissionCallback = id => IsOwner || IsServer
            });
            
            _isMovable = new NetworkVariable<bool>(new NetworkVariableSettings()
            {
                WritePermission = NetworkVariablePermission.Custom,
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermissionCallback = id => IsOwner || IsServer
            });
            
            _position = new NetworkVariable<Vector3>(new NetworkVariableSettings()
            {
                WritePermission = NetworkVariablePermission.Custom,
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermissionCallback = id => IsOwner || IsServer
            });
            
            _rotation = new NetworkVariable<Quaternion>(new NetworkVariableSettings()
            {
                WritePermission = NetworkVariablePermission.Custom,
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermissionCallback = id => IsOwner || IsServer
            });
            
            _prefabName = new NetworkVariable<string>(new NetworkVariableSettings()
            {
                WritePermission = NetworkVariablePermission.Custom,
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermissionCallback = id => IsOwner || IsServer
            });

            _maxStress = new NetworkVariable<float>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.Custom,
                WritePermissionCallback = id => IsOwner || IsServer
            });
            
            _currentStress = new NetworkVariable<float>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.Custom,
                WritePermissionCallback = id => IsOwner || IsServer
            });
            
            _shipId = new NetworkVariable<string>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.Custom,
                WritePermissionCallback = id => IsOwner || IsServer
            });
            
            _shipState = new NetworkVariable<UnitState>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.Custom,
                WritePermissionCallback = id => IsOwner || IsServer
            });
            
            _baseColor = new NetworkVariable<Color>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.Custom,
                WritePermissionCallback = id => IsOwner || IsServer
            });
        }
    }
}
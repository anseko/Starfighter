using System;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
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
        public NetworkVariable<FixedString128Bytes> _prefabName;
        public NetworkVariable<FixedString128Bytes> _id;
        public NetworkVariable<float> _maxStress;
        public NetworkVariable<float> _currentStress;
        public NetworkVariable<FixedString128Bytes> _shipId;
        public NetworkVariable<UnitState> _shipState;
        public NetworkVariable<Color> _baseColor;
        public NetworkVariable<float> _acceleration;
        public NetworkVariable<float> _radarRange;
        public NetworkVariable<float> _accelerationCoefficient;
        public NetworkVariable<float> _physResistanceCoefficient;
        public NetworkVariable<float> _radResistanceCoefficient;
        public NetworkVariable<float> _radarRangeCoefficient;

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
            get => _prefabName.Value.ToString();
            set => _prefabName.Value = new FixedString128Bytes(value);
        }
        public Guid ID
        {
            get => Guid.Parse(_id.Value.ToString());
            set => _id.Value = new FixedString128Bytes(value.ToString());
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
        public string ShipId => _shipId.Value.ToString();

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
        public float Acceleration
        {
            get => _acceleration.Value;
            set => _acceleration.Value = value;
        }
        public float RadarRange
        {
            get => _radarRange.Value;
            set => _radarRange.Value = value;
        }
        public float AccelerationCoefficient
        {
            get => _accelerationCoefficient.Value;
            set => _accelerationCoefficient.Value = value;
        }
        public float PhysResistanceCoefficient
        {
            get => _physResistanceCoefficient.Value;
            set => _physResistanceCoefficient.Value = value;
        }
        public float RadResistanceCoefficient
        {
            get => _radResistanceCoefficient.Value;
            set => _radResistanceCoefficient.Value = value;
        }
        public float RadarRangeCoefficient
        {
            get => _radarRangeCoefficient.Value;
            set => _radarRangeCoefficient.Value = value;
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
            _prefabName.Value = new FixedString128Bytes(config.prefabName);
            _id.Value = new FixedString128Bytes(config.id.ToString());
            _maxStress.Value = config.maxStress;
            _currentStress.Value = config.currentStress;
            _shipId.Value = new FixedString128Bytes(config.shipId);
            _shipState.Value = config.shipState;
            _baseColor.Value = config.baseColor;
            _acceleration.Value = config.acceleration;
            _radarRange.Value = config.radarRange;
            _accelerationCoefficient.Value = config.accelerationCoefficient;
            _physResistanceCoefficient.Value = config.physResistanceCoefficient;
            _radResistanceCoefficient.Value = config.radResistanceCoefficient;
            _radarRangeCoefficient.Value = config.radarRangeCoefficient;
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

            _maxAngleSpeed = new NetworkVariable<float>();
            
            _maxSpeed = new NetworkVariable<float>();
            
            _maxHp = new NetworkVariable<float>();
            
            _currentHp = new NetworkVariable<float>();
            
            _isDockable = new NetworkVariable<bool>();
            
            _isMovable = new NetworkVariable<bool>();
            
            _position = new NetworkVariable<Vector3>();
            
            _rotation = new NetworkVariable<Quaternion>();
            
            _prefabName = new NetworkVariable<FixedString128Bytes>();

            _maxStress = new NetworkVariable<float>();
            
            _currentStress = new NetworkVariable<float>();
            
            _shipId = new NetworkVariable<FixedString128Bytes>();
            
            _shipState = new NetworkVariable<UnitState>();
            
            _baseColor = new NetworkVariable<Color>();

            _id = new NetworkVariable<FixedString128Bytes>();
            
            _acceleration = new NetworkVariable<float>();

            _radarRange = new NetworkVariable<float>();
            
            _accelerationCoefficient = new NetworkVariable<float>();
            
            _physResistanceCoefficient = new NetworkVariable<float>();
            
            _radResistanceCoefficient = new NetworkVariable<float>();
            
            _radarRangeCoefficient = new NetworkVariable<float>();
        }
    }
}
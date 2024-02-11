using System.Collections.Generic;
using Client.Core;
using Mirror;
using Net.Core;
using UnityEngine;

namespace Net.Components
{
    public struct EngineState
    {
        public bool Thrust;
        public bool TopRight;
        public bool TopLeft;
        public bool BotLeft;
        public bool BotRight;
    }
    
    [RequireComponent(typeof(ConstantForce))]
    [RequireComponent(typeof(Rigidbody))]
    public class MoveComponent: NetworkBehaviour
    {
        [SerializeField]
        private GameObject _front, _back, _right, _left;
        [SerializeField]
        private List<ParticleSystem> _frontLeftSystems;
        [SerializeField]
        private List<ParticleSystem> _frontRightSystems;
        [SerializeField]
        private List<ParticleSystem> _backLeftSystems;
        [SerializeField]
        private List<ParticleSystem> _backRightSystems;
        [SerializeField]
        private List<ParticleSystem> _trustSystems;
        private ConstantForce _thrustForce;
        [SyncVar] private MovementData _lastMovement;
        private PlayerScript _unit;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _unit = GetComponent<PlayerScript>();
            // _lastMovement = new NetworkVariable<MovementData>(new NetworkVariableSettings {WritePermission = NetworkVariablePermission.OwnerOnly});
            _lastMovement.OnValueChanged += (value, newValue) =>
            {
                if (isOwned) AnimateMovementServerRpc();
            };
            
            _thrustForce = GetComponent<ConstantForce>();
            
            _frontLeftSystems.ForEach(x=>x.Stop());
            _frontRightSystems.ForEach(x=>x.Stop());
            _backLeftSystems.ForEach(x=>x.Stop());
            _backRightSystems.ForEach(x=>x.Stop());
            _trustSystems.ForEach(x=>x.Stop());
        }


        private void OnDisable()
        {   
            _frontLeftSystems.ForEach(x=>x.Stop());
            _frontRightSystems.ForEach(x=>x.Stop());
            _backLeftSystems.ForEach(x=>x.Stop());
            _backRightSystems.ForEach(x=>x.Stop());
            _trustSystems.ForEach(x=>x.Stop());
            _thrustForce.force = Vector3.zero;
            _thrustForce.torque = Vector3.zero;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            if (_unit.isOwned)
            {
                _unit.shipSpeed = _unit.Rigidbody.velocity;
                _unit.shipRotation = _unit.Rigidbody.angularVelocity;
            }
            
            _lastMovement = new MovementData()
            {
                rotationValue = 0,
                sideManeurValue = 0,
                straightManeurValue = 0,
                thrustValue = 0
            };
        }

        private void Update()
        {
            UpdateMovement();
        }
        
        private void UpdateMovement()
        {
            if (!isOwned || isServerOnly) return; 
            
            _lastMovement = new MovementData()
            {
                rotationValue = Input.GetAxis("Rotation") * _unit.networkUnitConfig.acceleration * _unit.networkUnitConfig.accelerationCoefficient,
                sideManeurValue = Input.GetAxis("Horizontal") * _unit.networkUnitConfig.accelerationCoefficient,
                straightManeurValue = Input.GetAxis("Vertical") * _unit.networkUnitConfig.accelerationCoefficient,
                thrustValue = Input.GetAxis("Jump") * _unit.networkUnitConfig.acceleration * _unit.networkUnitConfig.accelerationCoefficient
            };
            
            // расчет вектора тяги
            var thrustForceVector = _front.transform.position - _back.transform.position; //вектор фронтальной тяги
            var maneurForceVector = _right.transform.position - _left.transform.position; //вектор боковой тяги
            _thrustForce.force = 
                (thrustForceVector.normalized) * (_lastMovement.thrustValue + _lastMovement.straightManeurValue) +
                (maneurForceVector.normalized) * _lastMovement.sideManeurValue;
            _thrustForce.torque = new Vector3(0, _lastMovement.rotationValue, 0);
            
            if (Mathf.Abs(_rigidbody.angularVelocity.magnitude * Mathf.Rad2Deg) >= _unit.networkUnitConfig.maxAngleSpeed)
            {
                var angularVelocity = _rigidbody.angularVelocity;
                _thrustForce.torque = -(angularVelocity.normalized * ((Mathf.Abs(angularVelocity.magnitude * Mathf.Rad2Deg) - _unit.networkUnitConfig.maxAngleSpeed) * Mathf.Deg2Rad));
            }

            if (Mathf.Abs(_rigidbody.velocity.magnitude) >= _unit.networkUnitConfig.maxSpeed)
            {
                var velocity = _rigidbody.velocity;
                _thrustForce.force = -(velocity.normalized * (Mathf.Abs(velocity.magnitude) - _unit.networkUnitConfig.maxSpeed));
            }
        }

        [Command]
        private void AnimateMovementServerRpc()
        {
            #if !UNITY_SERVER
            #region Reset movement animation
            
            _trustSystems.ForEach(x=>x.Stop());
            _frontLeftSystems.ForEach(x=>x.Stop());
            _frontRightSystems.ForEach(x=>x.Stop());
            _backRightSystems.ForEach(x=>x.Stop());
            _backLeftSystems.ForEach(x=>x.Stop());
            
            #endregion
            
            var engines = GetEngines();
            if (engines.Thrust)
            {
                _trustSystems.ForEach(x=>x.Play(true));
            }

            if (engines.TopRight)
            {
                _frontRightSystems.ForEach(x=>x.Play(true));
            }

            if (engines.TopLeft)
            {
                _frontLeftSystems.ForEach(x=>x.Play(true));
            }

            if (engines.BotLeft)
            {
                _backLeftSystems.ForEach(x=>x.Play(true));
            }

            if (engines.BotRight)
            {
                _backRightSystems.ForEach(x=>x.Play(true));
            }
            #endif
            
            AnimateMovementClientRpc();
        }
        
        [ClientRpc]
        private void AnimateMovementClientRpc()
        {
            #region Reset movement animation
            
            _trustSystems.ForEach(x=>x.Stop());
            _frontLeftSystems.ForEach(x=>x.Stop());
            _frontRightSystems.ForEach(x=>x.Stop());
            _backRightSystems.ForEach(x=>x.Stop());
            _backLeftSystems.ForEach(x=>x.Stop());
            
            #endregion
            
            var engines = GetEngines();
            if (engines.Thrust)
            {
                _trustSystems.ForEach(x=>x.Play(true));
            }

            if (engines.TopRight)
            {
                _frontRightSystems.ForEach(x=>x.Play(true));
            }

            if (engines.TopLeft)
            {
                _frontLeftSystems.ForEach(x=>x.Play(true));
            }

            if (engines.BotLeft)
            {
                _backLeftSystems.ForEach(x=>x.Play(true));
            }

            if (engines.BotRight)
            {
                _backRightSystems.ForEach(x=>x.Play(true));
            }
        }

        private EngineState GetEngines()
        {
            var state = new EngineState();
            if(_lastMovement.thrustValue != 0)
            {
                state.Thrust = true;
            }
            if(_lastMovement.rotationValue < 0)
            {
                state.TopRight = true;
                state.BotLeft = true;
            }
            if(_lastMovement.rotationValue > 0)
            {
                state.TopLeft = true;
                state.BotRight = true;
            }
            if(_lastMovement.straightManeurValue < 0)
            {
                state.TopLeft = true;
                state.TopRight = true;
            }
            if(_lastMovement.straightManeurValue > 0)
            {
                state.BotLeft = true;
                state.BotRight = true;
            }
            if(_lastMovement.sideManeurValue > 0)
            {
                state.TopLeft = true;
                state.BotLeft = true;
            }
            if(_lastMovement.sideManeurValue < 0)
            {
                state.TopRight = true;
                state.BotRight = true;
            }
            return state;
        }
    }
}
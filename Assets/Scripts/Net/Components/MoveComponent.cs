using System.Collections.Generic;
using Client.Core;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
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
        private NetworkVariable<MovementData> _lastMovement;
        private PlayerScript _unit;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _unit = GetComponent<PlayerScript>();
            _lastMovement = new NetworkVariable<MovementData>(new NetworkVariableSettings {WritePermission = NetworkVariablePermission.OwnerOnly});
            _lastMovement.OnValueChanged += (value, newValue) =>
            {
                if (IsOwner) AnimateMovementServerRpc();
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
            _lastMovement.Value = new MovementData()
            {
                rotationValue = 0,
                sideManeurValue = 0,
                straightManeurValue = 0,
                thrustValue = 0
            };
            _thrustForce.force = Vector3.zero;
            _thrustForce.torque = Vector3.zero;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            if (_unit.IsOwner)
            {
                _unit.shipSpeed.Value = _unit.Rigidbody.velocity;
                _unit.shipRotation.Value = _unit.Rigidbody.angularVelocity;
            }

            if (!IsServer) AnimateMovementServerRpc();
            else AnimateMovementClientRpc();
        }

        private void Update()
        {
            UpdateMovement();
        }
        
        private void UpdateMovement()
        {
            if (!IsOwner || (IsServer && !IsHost)) return; 
            _lastMovement.Value = new MovementData()
            {
                rotationValue = Input.GetAxis("Rotation") * 4.5f,
                sideManeurValue = Input.GetAxis("Horizontal"),
                straightManeurValue = Input.GetAxis("Vertical"),
                thrustValue = Input.GetAxis("Jump") * 5f
            };
            
            // расчет вектора тяги
            var thrustForceVector = _front.transform.position - _back.transform.position; //вектор фронтальной тяги
            var maneurForceVector = _right.transform.position - _left.transform.position; //вектор боковой тяги
            _thrustForce.force = 
                (thrustForceVector.normalized) * (_lastMovement.Value.thrustValue + _lastMovement.Value.straightManeurValue) +
                (maneurForceVector.normalized) * _lastMovement.Value.sideManeurValue;
            _thrustForce.torque = new Vector3(0, _lastMovement.Value.rotationValue, 0);
            
            if (Mathf.Abs(_rigidbody.angularVelocity.magnitude * Mathf.Rad2Deg) >= _unit.NetworkUnitConfig.MaxAngleSpeed)
            {
                var angularVelocity = _rigidbody.angularVelocity;
                _thrustForce.torque = -(angularVelocity.normalized * ((Mathf.Abs(angularVelocity.magnitude * Mathf.Rad2Deg) - _unit.NetworkUnitConfig.MaxAngleSpeed) * Mathf.Deg2Rad));
            }

            if (Mathf.Abs(_rigidbody.velocity.magnitude) >= _unit.NetworkUnitConfig.MaxSpeed)
            {
                var velocity = _rigidbody.velocity;
                _thrustForce.force = -(velocity.normalized * (Mathf.Abs(velocity.magnitude) - _unit.NetworkUnitConfig.MaxSpeed));
            }
        }

        [ServerRpc]
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
            if(_lastMovement.Value.thrustValue != 0)
            {
                state.Thrust = true;
            }
            if(_lastMovement.Value.rotationValue < 0)
            {
                state.TopRight = true;
                state.BotLeft = true;
            }
            if(_lastMovement.Value.rotationValue > 0)
            {
                state.TopLeft = true;
                state.BotRight = true;
            }
            if(_lastMovement.Value.straightManeurValue < 0)
            {
                state.TopLeft = true;
                state.TopRight = true;
            }
            if(_lastMovement.Value.straightManeurValue > 0)
            {
                state.BotLeft = true;
                state.BotRight = true;
            }
            if(_lastMovement.Value.sideManeurValue > 0)
            {
                state.TopLeft = true;
                state.BotLeft = true;
            }
            if(_lastMovement.Value.sideManeurValue < 0)
            {
                state.TopRight = true;
                state.BotRight = true;
            }
            return state;
        }
    }
}
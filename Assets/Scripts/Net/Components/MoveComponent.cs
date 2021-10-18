using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using Net.PackageData.EventsData;
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

        private void Awake()
        {
            _lastMovement = new NetworkVariable<MovementData>(new NetworkVariableSettings {WritePermission = NetworkVariablePermission.OwnerOnly});
            _lastMovement.OnValueChanged += ValueChanged;
            _thrustForce = GetComponent<ConstantForce>();
        }

        private void ValueChanged(MovementData previousvalue, MovementData newvalue)
        {
            if(IsOwner) AnimateMovementServerRpc();
        }

        private void Update()
        {
            UpdateMovement();
        }
        
        private void UpdateMovement()
        {
            if (!IsOwner || IsServer) return; 
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
        }

        [ServerRpc]
        private void AnimateMovementServerRpc()
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
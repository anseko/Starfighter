using System;
using Client.Core;
using Core;
using MLAPI;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace Net.Components
{
    public class HealthComponent: NetworkBehaviour
    {
        [SerializeField] private PlayerScript _playerScript;
        public NetworkVariable<float> hpDelta;

        private void Awake()
        {
            hpDelta = new NetworkVariable<float>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.ServerOnly
            });
            hpDelta.Value = 0;
        }

        private void Update()
        {
            if (!IsServer) return;
            
            _playerScript.ShipConfig.currentHp = 
                Math.Min(
                    Math.Max(
                        _playerScript.ShipConfig.currentHp + hpDelta.Value * Time.deltaTime,
                        0
                    ),
                    _playerScript.ShipConfig.maxHp
                );
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            
            if (!IsServer) return;
            
            var otherVelocity = collision.gameObject.TryGetComponent<Rigidbody>(out var rigidbody) ? rigidbody.velocity : Vector3.zero;
            var percentageDamage = CalculateDamage((_playerScript.shipSpeed.Value - otherVelocity).magnitude,  _playerScript.ShipConfig.maxSpeed, Constants.MaxPossibleDamageHp);

            _playerScript.ShipConfig.currentHp -= _playerScript.ShipConfig.currentHp * (percentageDamage * 0.01f);

            if (_playerScript.ShipConfig.currentHp <= 0)
            {
                _playerScript.ShipConfig.currentHp = 0;
                _playerScript.unitStateMachine.ChangeState(UnitState.IsDead);
                return;
            }

            Debug.unityLogger.Log(
                $"Collision speed {_playerScript.shipSpeed.Value.magnitude}, result hp percentage damage is {percentageDamage}, current hp {_playerScript.ShipConfig.currentHp}");
        }

        private float CalculateDamage(float speed, float maxSpeed, float maxPossibleDamageHp) =>
            Mathf.Lerp(0, maxPossibleDamageHp, speed / maxSpeed);
    }
}
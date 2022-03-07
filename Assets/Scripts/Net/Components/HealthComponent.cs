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
            }, 0);

            _playerScript.NetworkUnitConfig._currentHp.OnValueChanged += (value, newValue) =>
            {
                if (newValue <= 0)
                {
                    _playerScript.NetworkUnitConfig.ShipState = UnitState.IsDead;
                    return;
                }

                if (newValue > 0 &&
                    _playerScript.NetworkUnitConfig.ShipState == UnitState.IsDead &&
                    _playerScript.NetworkUnitConfig.CurrentStress < _playerScript.NetworkUnitConfig.MaxStress)
                {
                    _playerScript.NetworkUnitConfig.ShipState = UnitState.InFlight;
                }
            };
            
            NetworkManager.Singleton.OnServerStarted += () =>
            {
                if(!IsServer) return;
                
                if (_playerScript.NetworkUnitConfig.CurrentHp <= 0)
                {
                    _playerScript.NetworkUnitConfig.CurrentHp = 0;
                    _playerScript.NetworkUnitConfig.ShipState = UnitState.IsDead;
                }
            };
        }

        private void Update()
        {
            if (!IsServer) return;
            
            _playerScript.NetworkUnitConfig.CurrentHp = 
                Math.Min(
                    Math.Max(
                        _playerScript.NetworkUnitConfig.CurrentHp + hpDelta.Value * Time.deltaTime * _playerScript.NetworkUnitConfig.RadResistanceCoefficient,
                        0
                    ),
                    _playerScript.NetworkUnitConfig.MaxHp
                );
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            
            if (!IsServer) return;
            
            var otherVelocity = collision.gameObject.TryGetComponent<Rigidbody>(out var rigidbody) ? rigidbody.velocity : Vector3.zero;
            var percentageDamage = CalculateDamage((_playerScript.shipSpeed.Value - otherVelocity).magnitude, _playerScript.NetworkUnitConfig.MaxSpeed, Constants.MaxPossibleDamageHp);

            _playerScript.NetworkUnitConfig.CurrentHp -= _playerScript.NetworkUnitConfig.MaxHp * (percentageDamage * 0.01f);
        }

        private float CalculateDamage(float speed, float maxSpeed, float maxPossibleDamageHp) =>
            Mathf.Lerp(0, maxPossibleDamageHp, speed / maxSpeed) * _playerScript.NetworkUnitConfig.PhysResistanceCoefficient;
    }
}
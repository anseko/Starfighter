using System;
using Client.Core;
using Core;
using Mirror;
using UnityEngine;

namespace Net.Components
{
    public class HealthComponent: NetworkBehaviour
    {
        [SerializeField] private PlayerScript _playerScript;
        [SyncVar]
        public float hpDelta;

        private void Awake()
        {
            // hpDelta = new NetworkVariable<float>(new NetworkVariableSettings()
            // {
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermission = NetworkVariablePermission.ServerOnly
            // }, 0);

            _playerScript.networkUnitConfig.currentHp.OnValueChanged += (value, newValue) =>
            {
                if (newValue <= 0)
                {
                    _playerScript.networkUnitConfig.shipState = UnitState.IsDead;
                    return;
                }

                if (newValue > 0 &&
                    _playerScript.networkUnitConfig.shipState == UnitState.IsDead &&
                    _playerScript.networkUnitConfig.currentStress < _playerScript.networkUnitConfig.maxStress)
                {
                    _playerScript.networkUnitConfig.shipState = UnitState.InFlight;
                }
            };
            
            NetworkManager.singleton.OnServerStarted += () =>
            {
                if(!isServer) return;
                
                if (_playerScript.networkUnitConfig.currentHp <= 0)
                {
                    _playerScript.networkUnitConfig.currentHp = 0;
                    _playerScript.networkUnitConfig.shipState = UnitState.IsDead;
                }
            };
        }

        private void Update()
        {
            if (!isServer) return;
            
            _playerScript.networkUnitConfig.currentHp = 
                Math.Min(
                    Math.Max(
                        _playerScript.networkUnitConfig.currentHp + hpDelta * Time.deltaTime * _playerScript.networkUnitConfig.radResistanceCoefficient,
                        0
                    ),
                    _playerScript.networkUnitConfig.maxHp
                );
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            
            if (!isServer) return;
            
            var otherVelocity = collision.gameObject.TryGetComponent<Rigidbody>(out var rigidbody) ? rigidbody.velocity : Vector3.zero;
            var percentageDamage = CalculateDamage((_playerScript.shipSpeed - otherVelocity).magnitude, _playerScript.networkUnitConfig.maxSpeed, Constants.MaxPossibleDamageHp);

            _playerScript.networkUnitConfig.currentHp -= _playerScript.networkUnitConfig.maxHp * (percentageDamage * 0.01f);
        }

        private float CalculateDamage(float speed, float maxSpeed, float maxPossibleDamageHp) =>
            Mathf.Lerp(0, maxPossibleDamageHp, speed / maxSpeed) * _playerScript.networkUnitConfig.physResistanceCoefficient;
    }
}
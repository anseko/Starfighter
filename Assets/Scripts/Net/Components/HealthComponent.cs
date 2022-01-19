using System;
using Client.Core;
using Core;
using MLAPI;
using MLAPI.NetworkVariable;
using ScriptableObjects;
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
            
            _playerScript.currentHp.Value = 
                Math.Min(
                    Math.Max(
                        _playerScript.currentHp.Value + hpDelta.Value * Time.deltaTime,
                        0
                    ),
                    ((SpaceShipConfig) _playerScript.unitConfig).maxHp
                );
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            
            if (!IsServer) return;
            
            var otherVelocity = collision.gameObject.GetComponent<Rigidbody>()?.velocity ?? Vector3.zero;
            var percentageDamage = CalculateDamage((_playerScript.shipSpeed.Value - otherVelocity).magnitude,  _playerScript.unitConfig.maxSpeed, Constants.MaxPossibleDamageHp);

            _playerScript.currentHp.Value -= _playerScript.currentHp.Value * (percentageDamage * 0.01f);

            if (_playerScript.currentHp.Value <= 0)
            {
                _playerScript.currentHp.Value = 0;
                _playerScript.unitStateMachine.ChangeState(UnitState.IsDead);
                return;
            }

            Debug.unityLogger.Log(
                $"Collision speed {_playerScript.shipSpeed.Value.magnitude}, result hp percentage damage is {percentageDamage}, current hp {_playerScript.currentHp.Value}");
        }

        private float CalculateDamage(float speed, float maxSpeed, float maxPossibleDamageHp) =>
            Mathf.Lerp(0, maxPossibleDamageHp, speed / maxSpeed);
    }
}
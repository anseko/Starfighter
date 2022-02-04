using System;
using Client.Core;
using Core;
using MLAPI;
using MLAPI.Messaging;
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

            NetworkManager.Singleton.OnServerStarted += () =>
            {
                if(!IsServer) return;
                
                if (_playerScript.currentHp.Value <= 0)
                {
                    _playerScript.currentHp.Value = 0;
                    _playerScript.currentState.Value = UnitState.IsDead;
                }
            };
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
            
            var otherVelocity = collision.gameObject.TryGetComponent<Rigidbody>(out var rigidbody) ? rigidbody.velocity : Vector3.zero;
            var percentageDamage = CalculateDamage((_playerScript.shipSpeed.Value - otherVelocity).magnitude ,  _playerScript.unitConfig.maxSpeed, Constants.MaxPossibleDamageHp);

            _playerScript.currentHp.Value -= _playerScript.unitConfig.maxHp * (percentageDamage * 0.01f);

            if (_playerScript.currentHp.Value <= 0)
            {
                _playerScript.currentHp.Value = 0;
                // _playerScript.unitStateMachine.ChangeState(UnitState.IsDead);
                _playerScript.currentState.Value = UnitState.IsDead;
                // ChangeToDeadClientRpc();
                return;
            }

            Debug.unityLogger.Log(
                $"Collision speed {_playerScript.shipSpeed.Value.magnitude}, result hp percentage damage is {percentageDamage}, current hp {_playerScript.currentHp.Value}");
        }

        [ClientRpc]
        private void ChangeToDeadClientRpc()
        {
            if (!IsOwner || IsServer) return;
            _playerScript.unitStateMachine.ChangeState(UnitState.IsDead);
        }
        
        private float CalculateDamage(float speed, float maxSpeed, float maxPossibleDamageHp) =>
            Mathf.Lerp(0, maxPossibleDamageHp, speed / maxSpeed);
    }
}
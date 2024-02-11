using System;
using Client.Core;
using Core;
using Mirror;
using UnityEngine;

namespace Net.Components
{
    public class StressComponent: NetworkBehaviour
    {
        [SerializeField] private PlayerScript _playerScript;
        [SyncVar] public float stressDelta;

        private void Awake()
        {
            stressDelta = 0.0114f; 
            // new NetworkVariable<float>(new NetworkVariableSettings()
            // {
            //     ReadPermission = NetworkVariablePermission.Everyone,
            //     WritePermission = NetworkVariablePermission.ServerOnly
            // }, 0.0114f);
            
            ClientEventStorage.GetInstance().OnCurrentStressChange.AddListener((newValue) =>
            {
                if (newValue >=_playerScript.networkUnitConfig.maxStress)
                {
                    _playerScript.networkUnitConfig.shipState = UnitState.IsDead;
                    return;
                }

                if (newValue < _playerScript.networkUnitConfig.maxStress &&
                    _playerScript.networkUnitConfig.shipState == UnitState.IsDead &&
                    _playerScript.networkUnitConfig.currentHp > 0)
                {
                    _playerScript.networkUnitConfig.shipState = UnitState.InFlight;
                }
            });
        }

        private void Start() => CheckForOverstressedStateCommand();
        
        private void Update()
        {
            if (!isServer) return;
            
            _playerScript.networkUnitConfig.currentStress = 
                Math.Min(
                    Math.Max(
                        _playerScript.networkUnitConfig.currentStress + stressDelta * Time.deltaTime,
                        0
                        ),
                    _playerScript.networkUnitConfig.maxStress
                    );
        }

        [Command]
        private void CheckForOverstressedStateCommand()
        {
            if (_playerScript.networkUnitConfig.currentStress >= _playerScript.networkUnitConfig.maxStress)
            {
                _playerScript.networkUnitConfig.currentStress = _playerScript.networkUnitConfig.maxStress;
                _playerScript.networkUnitConfig.shipState = UnitState.IsDead;
            }
        }
    }
}
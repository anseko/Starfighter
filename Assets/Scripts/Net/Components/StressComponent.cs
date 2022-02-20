using System;
using Client.Core;
using Core;
using MLAPI;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace Net.Components
{
    public class StressComponent: NetworkBehaviour
    {
        [SerializeField] private PlayerScript _playerScript;
        public NetworkVariable<float> stressDelta;

        private void Awake()
        {
            stressDelta = new NetworkVariable<float>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.ServerOnly
            }, 0.0114f);
            
            _playerScript.NetworkUnitConfig._currentStress.OnValueChanged += (value, newValue) =>
            {
                if (newValue >=_playerScript.NetworkUnitConfig.MaxStress)
                {
                    _playerScript.NetworkUnitConfig.ShipState = UnitState.IsDead;
                    return;
                }

                if (newValue < _playerScript.NetworkUnitConfig.MaxStress && _playerScript.NetworkUnitConfig.ShipState == UnitState.IsDead)
                {
                    _playerScript.NetworkUnitConfig.ShipState = UnitState.InFlight;
                }
            };
            
            NetworkManager.Singleton.OnServerStarted += () =>
            {
                if(!IsServer) return;
                
                if (_playerScript.NetworkUnitConfig.CurrentStress >= _playerScript.NetworkUnitConfig.MaxStress)
                {
                    _playerScript.NetworkUnitConfig.CurrentStress = _playerScript.NetworkUnitConfig.MaxStress;
                    _playerScript.NetworkUnitConfig.ShipState = UnitState.IsDead;
                }
            };
        }

        private void Update()
        {
            if (!IsServer) return;
            
            _playerScript.NetworkUnitConfig.CurrentStress = 
                Math.Min(
                    Math.Max(
                        _playerScript.NetworkUnitConfig.CurrentStress + stressDelta.Value * Time.deltaTime,
                        0
                        ),
                    _playerScript.NetworkUnitConfig.MaxStress
                    );
        }
    }
}
using System;
using Client.Core;
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
            });
        }

        private void Update()
        {
            if(IsServer)
                _playerScript.ShipConfig.currentStress = 
                    Math.Min(
                        Math.Max(
                            _playerScript.ShipConfig.currentStress + stressDelta.Value * Time.deltaTime,
                            0
                            ),
                        _playerScript.ShipConfig.maxStress
                        );
        }
    }
}
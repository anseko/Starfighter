using System;
using Core;
using UnityEngine;

namespace Client
{
    public class CollisionHpReductionScript : MonoBehaviour
    {
        private PlayerScript _playerScript;

        public void Init(PlayerScript playerScript)
        {
            _playerScript = playerScript;
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            var maxSpeed = _playerScript.shipConfig.maxSpeed;
            var speed = _playerScript.shipSpeed.magnitude;
            
            var resultHp = (float)Math.Pow(Math.E, speed / maxSpeed);
            
            _playerScript.shipConfig.currentHp -= resultHp;

            if (_playerScript.shipConfig.currentHp <= 0)
            {
                _playerScript.unitStateMachine.ChangeState(UnitState.IsDead);
                Debug.unityLogger.Log(
                    String.Format($"Ship is dead, current speed {speed}, result hp {resultHp}, current hp {_playerScript.shipConfig.currentHp}")
                    );
            }

            Debug.unityLogger.Log(
                String.Format($"Collision speed {speed}, result hp {resultHp}, current hp {_playerScript.shipConfig.currentHp}", speed, resultHp, _playerScript.shipConfig.currentHp)
                );
        }
    }
}
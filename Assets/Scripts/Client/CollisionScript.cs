using System;
using Client;
using Core;
using UnityEngine;

namespace DefaultNamespace
{
    public class CollisionScript : MonoBehaviour
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
                    String.Format("Ship is dead, current speed ${0}, result hp ${1}, current hp ${2}", speed, resultHp, _playerScript.shipConfig.currentHp)
                    );
            }

            Debug.unityLogger.Log(
                String.Format("Collision speed ${0}, result hp ${1}, current hp ${2}", speed, resultHp, _playerScript.shipConfig.currentHp)
                );
        }
    }
}
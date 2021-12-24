using System;
using Client.Core;
using Client.Util;
using Core;
using UnityEngine;

namespace Client
{
    public class CollisionHpReductionScript : MonoBehaviour
    {
        private PlayerScript _playerScript;
        private ShipDamageCalculationUtil _calculationUtil;
        
        public void Init(PlayerScript playerScript, ShipDamageCalculationUtil util)
        {
            _calculationUtil = util;
            _playerScript = playerScript;
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            var maxSpeed = _playerScript.unitConfig.maxSpeed;
            var speed = _playerScript.shipSpeed.Value.magnitude;

            var resultHp = _calculationUtil.CalculateDamage(speed, maxSpeed, Constants.MaxPossibleDamageHp);

            _playerScript.unitConfig.currentHp -= resultHp;

            if (_playerScript.unitConfig.currentHp <= 0)
            {
                _playerScript.unitStateMachine.ChangeState(UnitState.IsDead);
                Debug.unityLogger.Log(
                    String.Format($"Ship is dead, current speed {speed}, result hp {resultHp}, current hp {_playerScript.unitConfig.currentHp}")
                    );
            }

            Debug.unityLogger.Log(
                String.Format($"Collision speed {speed}, result hp {resultHp}, current hp {_playerScript.unitConfig.currentHp}", speed, resultHp, _playerScript.unitConfig.currentHp)
                );
        }
    }
}
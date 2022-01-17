using Client.Core;
using Core;
using MLAPI;
using UnityEngine;

namespace Net.Components
{
    public class HealthComponent: NetworkBehaviour
    {
        [SerializeField] private PlayerScript _playerScript;

        private void OnCollisionEnter(Collision collision)
        {
            if (!IsOwner) return;
            
            var otherVelocity = collision.gameObject.GetComponent<Rigidbody>()?.velocity ?? Vector3.zero;
            var percentageDamage = CalculateDamage((_playerScript.shipSpeed.Value - otherVelocity).magnitude,  _playerScript.unitConfig.maxSpeed, Constants.MaxPossibleDamageHp);

            _playerScript.currentHp.Value -= _playerScript.currentHp.Value * (percentageDamage * 0.01f);

            Debug.unityLogger.Log(
                $"Collision speed {_playerScript.shipSpeed.Value.magnitude}, result hp percentage damage is {percentageDamage}, current hp {_playerScript.currentHp.Value}");
        }

        private float CalculateDamage(float speed, float maxSpeed, float maxPossibleDamageHp) =>
            Mathf.Lerp(0, maxPossibleDamageHp, speed / maxSpeed);
    }
}
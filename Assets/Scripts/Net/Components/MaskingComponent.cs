using System;
using System.Collections;
using System.Threading.Tasks;
using Client.Core;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

namespace Net.Components
{
    public class MaskingComponent: NetworkBehaviour
    {
        [SerializeField] private GameObject model;
        private PlayerScript _playerScript;
        private Material _bodymat;
        private Task _dissolveTask;
        private static readonly int Value = Shader.PropertyToID("Value");

        private void Awake()
        {
            _bodymat = model.GetComponent<Renderer>().material;
            _playerScript = GetComponent<PlayerScript>();
            _bodymat.SetFloat(Value, 0);
        }

        private void Update()
        {
            if (!IsOwner || IsServer) return;
            if (Input.GetKeyDown(_playerScript.keyConfig.mask))
            {
                StartCoroutine(Dissolve(300));
            }
        }

        [ClientRpc]
        public void DissolveClientRpc()
        {
            
        }

        [ServerRpc]
        public void DissolveServerRpc()
        {
            
        }
        
        private IEnumerator Dissolve(int timeLength)
        {
            if (_bodymat.GetFloat(Value) > 0.5)
            {
                for (var i = timeLength; i >= 0; i--)
                {
                    _bodymat.SetFloat(Value, (float)i / timeLength);
                    yield return new WaitForSeconds(0.001f);
                }
                yield break;
            }
            
            for (var i = 0; i < timeLength; i++)
            {
                _bodymat.SetFloat(Value, (float)i / timeLength);
                yield return new WaitForSeconds(0.001f);
            }
            
        }
    }
}
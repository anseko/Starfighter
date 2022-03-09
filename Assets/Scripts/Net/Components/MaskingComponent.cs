using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Client.Core;
using MLAPI;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace Net.Components
{
    public class MaskingComponent: NetworkBehaviour
    {
        [SerializeField] private GameObject model;
        [SerializeField] private GameObject numbers;
        private PlayerScript _playerScript;
        private List<Material> _bodymats;
        private Coroutine _dissolveCoroutine;
        private NetworkVariableBool _isMasked;
        private static readonly int Value = Shader.PropertyToID("Value");

        private void Awake()
        {
            _isMasked = new NetworkVariableBool(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.OwnerOnly
            })
            {
                Value = false
            };
            
            _bodymats = model.GetComponents<Renderer>().SelectMany(x=>x.materials).ToList();
            _bodymats.AddRange(model.GetComponentsInChildren<Renderer>().SelectMany(x=>x.materials).ToList());
            _playerScript = GetComponent<PlayerScript>();
            _bodymats.ForEach(x => x.SetFloat(Value, 0));
        }

        private void Start()
        {
            _bodymats.ForEach(x => x.SetFloat(Value, _isMasked.Value ? 1 : 0));
            if(_dissolveCoroutine != null) StopCoroutine(_dissolveCoroutine);
            _isMasked.OnValueChanged += (value, newValue) => _dissolveCoroutine = StartCoroutine(Dissolve(300));
        }

        private void Update()
        {
            if (!IsOwner || IsServer) return;
            if (Input.GetKeyDown(_playerScript.keyConfig.mask))
            {
                _isMasked.Value = !_isMasked.Value;
            }
        }

        private IEnumerator Dissolve(int timeLength)
        {
            if (!_isMasked.Value)
            {
                for (var i = timeLength; i >= 0; i--)
                {
                    _bodymats.ForEach(x=>x.SetFloat(Value, (float)i / timeLength));
                    yield return new WaitForSeconds(0.001f);
                }
                numbers?.SetActive(true);
                yield break;
            }
            
            for (var i = 0; i < timeLength; i++)
            {
                _bodymats.ForEach(x=>x.SetFloat(Value, (float)i / timeLength));
                yield return new WaitForSeconds(0.001f);
            }
            numbers?.SetActive(false);
        }
    }
}
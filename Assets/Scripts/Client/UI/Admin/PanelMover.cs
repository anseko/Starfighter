using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI.Admin
{
    public class PanelMover : MonoBehaviour
    {
        [SerializeField] private bool isOn;
        [SerializeField] private RectTransform _panel;
        private float ratio;
        private Coroutine _coroutine;
        private void Awake()
        {
            ratio = isOn ? 1 : 0; 
            
            GetComponent<Button>().onClick.AddListener(() =>
            {
                if(_coroutine != null) StopCoroutine(_coroutine);
                _coroutine = StartCoroutine(MoveCoroutine());
            });
        }

        private IEnumerator MoveCoroutine()
        {
            var delta = isOn ? Vector3.left : Vector3.right;
            var startPosition = _panel.position;
            var targetPosition = _panel.position + delta * _panel.rect.width;
            Debug.unityLogger.Log($"Start: {startPosition}, Target: {targetPosition}");
            var i = 0f;
            while (i < 1)
            {
                _panel.position = Vector3.Lerp(startPosition, targetPosition, i);
                i += Time.deltaTime;
                yield return null;
            }

            _panel.position = targetPosition;
            isOn = !isOn;
        }
    }
}
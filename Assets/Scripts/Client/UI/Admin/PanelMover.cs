using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI.Admin
{
    public class PanelMover : MonoBehaviour
    {
        public enum HideDirection
        {
            Up,
            Down,
            Left,
            Right
        }
        
        
        [SerializeField] private bool isOn;
        [SerializeField] private HideDirection _hideDirection;
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
            var delta = _hideDirection switch
            {
                HideDirection.Up => (isOn ? Vector3.up : Vector3.down) * _panel.rect.height,
                HideDirection.Down => (isOn ? Vector3.down : Vector3.up) * _panel.rect.height,
                HideDirection.Left => (isOn ? Vector3.left : Vector3.right) * _panel.rect.width,
                HideDirection.Right => (isOn ? Vector3.right : Vector3.left) * _panel.rect.width,
                _ => throw new ArgumentOutOfRangeException()
            };

            var startPosition = _panel.position;
            var targetPosition = _panel.position + delta;
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
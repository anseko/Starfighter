using UnityEngine;

namespace Client
{
    public class Scaler : MonoBehaviour
    {
        private Camera _cam;
        [SerializeField] private float _scaleMultiplier = 20;
        
        private void Start()
        {
            _cam = FindObjectOfType<Camera>();
        }
        
        // Update is called once per frame
        private void LateUpdate()
        {
            //TODO: допилить скалирование по триггеру
            var scale = _cam.orthographicSize / _scaleMultiplier;
            transform.localScale = Vector3.one * scale;
        }
    }
}

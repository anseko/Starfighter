using UnityEngine;

namespace Client
{
    public class ParallaxScript : MonoBehaviour
    {
        private MeshRenderer _mr;
        private Material _mat;
        private Vector2 _offset;

        public Camera camera;
        public float ParallaxRate;
        
        
        private void Start()
        {
            _mr = GetComponent<MeshRenderer>();
            _mat = _mr.material;
            _offset = _mat.mainTextureOffset;
            var cameraHeight = camera.orthographicSize * 2;
            var cameraSize = new Vector2(camera.aspect * cameraHeight, cameraHeight);
            var scale = transform.localScale;
            scale *= cameraSize.x / _mr.bounds.size.x;
            transform.localScale = scale;
        }

        private void FixedUpdate()
        {
            _offset.x = transform.position.x / 10f / ParallaxRate;
            _offset.y = transform.position.z / 10f / ParallaxRate;
            _mat.mainTextureOffset = _offset;
            OnResize();
        }

        private void OnResize()
        {
            _mr = GetComponent<MeshRenderer>();
            _mat = _mr.material;
            _offset = _mat.mainTextureOffset;
            
            var cameraHeight = camera.orthographicSize * 2;
            var cameraSize = new Vector2(camera.aspect * cameraHeight, cameraHeight);
            var scale = transform.localScale;
            scale *= cameraSize.x / _mr.bounds.size.x;
            transform.localScale = scale;
        }
    }
}

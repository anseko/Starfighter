using UnityEngine;

namespace Sprites.Background
{
    public class ParallaxScript : MonoBehaviour
    {
        MeshRenderer mr;
        private Material mat;
        private Vector2 offset;

        public Camera camera;
        public float ParallaxRate;
        // Start is called before the first frame update
        void Start()
        {
            mr = GetComponent<MeshRenderer>();
            mat = mr.material;
            offset = mat.mainTextureOffset;
            
            var cameraHeight = camera.orthographicSize * 2;
            var cameraSize = new Vector2(camera.aspect * cameraHeight, cameraHeight);
            var scale = transform.localScale;
            scale *= cameraSize.x / mr.bounds.size.x;
            transform.localScale = scale;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            offset.x = transform.position.x / 10f / ParallaxRate;
            offset.y = transform.position.z / 10f / ParallaxRate;
            mat.mainTextureOffset = offset;
        }
    }
}

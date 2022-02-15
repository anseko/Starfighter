using UnityEngine;

namespace Client
{
    public class Zoom : MonoBehaviour
    {
        public Camera navigatorCamera;

        private void LateUpdate()
        {
            var oldOrtho = navigatorCamera.orthographicSize;
            navigatorCamera.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * 500;
            if (navigatorCamera.orthographicSize < 20) navigatorCamera.orthographicSize = 20;
            if (navigatorCamera.orthographicSize > 800) navigatorCamera.orthographicSize = 800;
            if ((int)oldOrtho != (int)navigatorCamera.orthographicSize)
            {
                foreach (var parallaxScript in FindObjectsOfType<ParallaxScript>())
                {
                    parallaxScript.OnResize();
                }
            }
        }
    }
}
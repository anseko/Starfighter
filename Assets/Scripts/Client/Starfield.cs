using UnityEngine;

namespace Client
{
    public class Starfield: MonoBehaviour 
    {
        public Camera camera;
        private Vector2 lastScreenSize = new Vector2();

        private void OnEnable() 
        {
            if (!camera)
            {
                Debug.Log ("Camera is not set");
                enabled = false;			
            }
        }

        private void Update () 
        {
            if (Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y)
                updateSize();
        }

        private void LateUpdate()
        {
            var pos = transform.position;
            pos.x = camera.transform.position.x;
            pos.y = camera.transform.position.y;
            transform.position = pos;
        }
	
        private void updateSize()
        {
            lastScreenSize.x = Screen.width; 
            lastScreenSize.y = Screen.height;
							 
            var maxSize = lastScreenSize.x > lastScreenSize.y ? lastScreenSize.x : lastScreenSize.y;	
            maxSize /= 10;
            transform.localScale = new Vector3(maxSize, 1, maxSize);			
        }
    }
}
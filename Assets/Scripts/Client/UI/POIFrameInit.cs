using Client.Core;
using MLAPI.Messaging;
using UnityEngine;

namespace Client.UI
{
    public class POIFrameInit : MonoBehaviour
    {
        public string identificator;
        public Vector3 position;
        public Vector3 size;
        public string text;
        public float scaleX;
        public float scaleZ;

        public void FrameInit(string idInit, Vector3 positionInit, Vector3 sizeInit, string textInit, bool isClient = false)
        {
            identificator = idInit;
            position = positionInit;
            size = sizeInit;
            text = textInit;
            scaleX = size.x - position.x;
            scaleZ = size.z - position.z;
            transform.position = new Vector3(positionInit.x + scaleX / 2, -180, positionInit.z+scaleZ/2);
            transform.localScale = new Vector3( Mathf.Abs(scaleX), Mathf.Abs(scaleZ), 1);
            transform.eulerAngles = new Vector3(90,0,0);

            if (isClient)
            {
                transform.Find("DestroyButton")?.gameObject.SetActive(false);
                transform.Find("EditButton")?.gameObject.SetActive(false);
            }
        }
    }
    
    
}
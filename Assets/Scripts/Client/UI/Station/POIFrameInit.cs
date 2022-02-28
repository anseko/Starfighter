using MLAPI.NetworkVariable;
using UnityEngine;

namespace Client.UI
{
    public class POIFrameInit : MonoBehaviour
    {
        public string identificator;
        public Vector3 position;
        public Vector3 size;
        public NetworkVariable<string> text;
        public float scaleX;
        public float scaleZ;

        private void Awake()
        {
            text = new NetworkVariable<string>(new NetworkVariableSettings()
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.ServerOnly
            });
        }

        public void FrameInit(string idInit, Vector3 positionInit, Vector3 sizeInit, string textInit, bool isClient = false)
        {
            identificator = idInit;
            position = positionInit;
            size = sizeInit;
            text.Value = textInit;
            scaleX = size.x - position.x;
            scaleZ = size.z - position.z;
            transform.position = new Vector3(positionInit.x + scaleX / 2, -180, positionInit.z + scaleZ/2);
            transform.localScale = new Vector3( Mathf.Abs(scaleX), Mathf.Abs(scaleZ), 1);
            transform.eulerAngles = new Vector3(90,0,0);

            // if (isClient)
            // {
            //     transform.Find("DestroyButton")?.gameObject.SetActive(false);
            //     transform.Find("EditButton")?.gameObject.SetActive(false);
            // }
        }
    }
    
    
}

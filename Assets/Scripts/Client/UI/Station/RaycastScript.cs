using Client.UI;
using TMPro;
using UnityEngine;

namespace Client
{
    public class RaycastScript : MonoBehaviour
    {
        private Camera _camera;
        [SerializeField] private TMP_Text _textContainer;

        private void Start()
        {
            _camera = GameObject.Find("SpaceStationCamera").GetComponent<Camera>();
        }

        private void Update()
        {
            var myRay = _camera.ScreenPointToRay(Input.mousePosition);
            
            if(Physics.Raycast(myRay, out var hit, 100500, layerMask: 1 << 15))
            {
                var go = hit.collider.gameObject;
                
                if (Input.GetMouseButtonDown(0))
                {
                    switch (go.name)
                    {
                        case "DestroyButton":
                            go.transform.parent.GetComponent<Editor>().Destroy();
                            break;
                        case "EditButton":
                            go.transform.parent.GetComponent<Editor>().Edit();
                            break;
                    }
                }
                else
                {
                    switch (go.name)
                    {
                        case "OrderStaticFrame(Clone)":
                            _textContainer.transform.position = Input.mousePosition;
                            _textContainer.text = go.GetComponent<OrderFrameInit>()?.ship?.NetworkUnitConfig.ShipId
                                                  + "\n" + go.GetComponent<OrderFrameInit>()?.text;
                            break;
                        case "DestroyButton":
                        case "EditButton":
                            _textContainer.text = "";
                            break;
                    }
                }
            }
            else
            {
                _textContainer.text = "";
            }
        }
    }
}

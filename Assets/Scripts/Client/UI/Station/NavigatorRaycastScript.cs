using Client.UI;
using TMPro;
using UnityEngine;

namespace Client
{
    public class NavigatorRaycastScript : MonoBehaviour
    {
        private Camera _camera;
        [SerializeField] private TMP_Text _textContainer;

        private void Start()
        {
            _camera = GameObject.Find("NavigatorCamera").GetComponent<Camera>();
        }

        private void Update()
        {
            var myRay = _camera.ScreenPointToRay(Input.mousePosition);
            
            if(Physics.Raycast(myRay, out var hit, 100500, layerMask: 1 << 15))
            {
                var go = hit.collider.gameObject;
                
                if (go.name.Contains("OrderStaticFrame"))
                {
                    _textContainer.transform.position = Input.mousePosition;
                    _textContainer.text = go.GetComponent<OrderFrameInit>()?.text;
                }
                else
                {
                    _textContainer.text = "";
                }
            }
            else
            {
                _textContainer.text = "";
            }
        }
    }
}

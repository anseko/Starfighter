using UnityEngine;
using UnityEngine.EventSystems;

namespace Client.UI.Admin
{
    public class HoverBlocker: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Zoom _zoom;
        [SerializeField] private CameraMotion _cameraMotion;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _zoom.enabled = false;
            _cameraMotion.enabled = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _zoom.enabled = true;
            _cameraMotion.enabled = true;
        }
    }
}
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

namespace Client
{
    public class CameraMotion : MonoBehaviour
    {
        public GameObject Player;
        private Camera _camera;
        private Vector3 _offset;
        private Vector3 _translationPoint;
        [SerializeField] private bool _isFollowMode;
        [SerializeField] private Vector3 lastMousePosition;
        [SerializeField] private Vector3 currentMousePosition;
        
        // Start is called before the first frame update
        private void Start()
        {
            _camera = gameObject.GetComponent<Camera>();
            _offset = Vector3.up * 90;
        }

        private void FreeMode()
        {
            _translationPoint = new Vector3(Input.GetAxis("Horizontal") * _camera.orthographicSize, Input.GetAxis("Vertical") * _camera.orthographicSize, 0);
            _camera.transform.Translate(_translationPoint * -1 * Time.deltaTime);
        }

        

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!_isFollowMode)
            {
                lastMousePosition = _camera.ScreenToWorldPoint(eventData.position);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isFollowMode)
            {
                currentMousePosition = _camera.ScreenToWorldPoint(eventData.position);
                Vector3 difference = currentMousePosition - lastMousePosition;
                _camera.transform.Translate(_camera.transform.position+difference);;
                lastMousePosition = currentMousePosition;
            }
            
        }

        private void FollowShip()
        {
            transform.position = Player.transform.position + _offset;
        }
        
        private void LateUpdate()
        {
            if (_isFollowMode) FollowShip();
            else FreeMode();
        }

        public void SwitchFollowMode()
        {
            _isFollowMode = !_isFollowMode;
        }
        
        public bool GetFollowMode()
        {
            return _isFollowMode;
        }
        
    }
}
using UnityEngine;
using UnityEngine.EventSystems;

namespace Client
{
    public class CameraMotion : MonoBehaviour
    {
        public GameObject Player;
        private Camera _camera;
        private Vector3 _offset;
        private Vector3 _translationPoint;
        [SerializeField] private bool _isFollowMode;
        [SerializeField] private Vector3 _startPosition;
        public bool _isDragable;

        // Start is called before the first frame update
        private void Start()
        {
            _camera = gameObject.GetComponent<Camera>();
            _offset = Vector3.up * 90;
            _isDragable = true;
        }

        private void FreeMode()
        {
            if (_isDragable)
            {
                _translationPoint = new Vector3(Input.GetAxis("Horizontal") * _camera.orthographicSize,
                    Input.GetAxis("Vertical") * _camera.orthographicSize, 0);
                _camera.transform.Translate(_translationPoint * -1 * Time.deltaTime);
                
                if (Input.GetMouseButtonDown(0))
                {
                    _startPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
                }
                
                if (Input.GetMouseButton(0))
                {
                    Vector3 direction = _startPosition - _camera.ScreenToWorldPoint(Input.mousePosition);
                    _camera.transform.position += direction;
                }
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
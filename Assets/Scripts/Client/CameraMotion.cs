using UnityEngine;

namespace Client
{
    public class CameraMotion : MonoBehaviour
    {
        public GameObject Player;
        private Camera _camera;
        private Vector3 _offset;
        private Vector3 _translationPoint;
        private Vector3 _startPoint;
        private Vector2 _mousePosition;
        [SerializeField]
        private bool _isFollowMode;
        [SerializeField]
        private Vector3 _dragVector;
        
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

        private void OnMouseDrag()
        {
            _startPoint = _camera.transform.position;
           // _mousePosition.x = 
           // _dragVector = _camera.ScreenToWorldPoint();
           // _camera.transform.Translate(- _dragVector);
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
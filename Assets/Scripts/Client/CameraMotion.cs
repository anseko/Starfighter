using UnityEngine;

namespace Client
{
    public class CameraMotion : MonoBehaviour
    {
        public GameObject Player;
        private Camera _camera;
        private Vector3 _offset;
        private Vector3 _translationPoint;
        [SerializeField]
        private bool _isFollowMode;
        
        // Start is called before the first frame update
        private void Start()
        {
            _camera = gameObject.GetComponent<Camera>();
            _offset = Vector3.up * 90;
        }

        private void FreeMode()
        {
            _translationPoint = new Vector3(Input.GetAxis("Horizontal") * _camera.orthographicSize / 30, Input.GetAxis("Vertical") * _camera.orthographicSize / 30, 0);
            _camera.transform.Translate(_translationPoint * -1);
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
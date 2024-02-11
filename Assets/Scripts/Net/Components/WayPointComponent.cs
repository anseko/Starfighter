using Client;
using Core;
using Mirror;
using UnityEngine;
using UnityEngine.Timeline;

namespace Net.Components
{
    public class WayPointComponent: NetworkBehaviour
    {
        [SerializeField]
        private GameObject _pointPrefab;
        private GameObject _point;
        private GameObject _localPoint;
        private Camera _camera;
        [SerializeField]
        private bool _isSetter;

        public void Init(bool isSetter)
        {
            _isSetter = isSetter;
            _camera = FindObjectOfType<Camera>(includeInactive: false);
        }
        
        private void Update()
        {
            if (Input.GetMouseButtonUp(1) && _isSetter)
            {
                //TOOD: redo with teams
                // var navigatorClientId = NetworkManager.singleton.LocalClientId; connectionToServer.connectionId;
                // var pilotClientId = GetComponent<NetworkIdentity>().OwnerClientId; 
                // MovePointServerRpc(_camera.ScreenToWorldPoint(Input.mousePosition), navigatorClientId, pilotClientId);
            }

            if (Input.GetMouseButtonUp(2) && _isSetter)
            {
                var position = _camera.ScreenToWorldPoint(Input.mousePosition);
                position.Set(position.x, 0,position.z);
                if (_localPoint is null)
                {
                    _localPoint = Instantiate(_pointPrefab, position, _pointPrefab.transform.rotation);
                    _localPoint.GetComponent<SpriteRenderer>().color = Color.green;
                    _localPoint.tag = Constants.WayPointTag;
                }
                _localPoint.transform.position = position;
            }
        }

        [Command(requiresAuthority = false)]
        private void MovePointServerRpc(Vector3 position, NetworkConnectionToClient naviClientConn, NetworkConnectionToClient pilotClientConn)
        {
            //send to two clients; see Teams
            MovePointClientRpc(naviClientConn, position);
            MovePointClientRpc(pilotClientConn, position);
        }
        
        [TargetRpc]
        private void MovePointClientRpc(NetworkConnectionToClient connectionToClient, Vector3 position)
        {
            position.Set(position.x, 0,position.z);
            if (_point is null)
            {
                _point = Instantiate(_pointPrefab, position, _pointPrefab.transform.rotation);
                _point.tag = Constants.WayPointTag;
                var gpsView = FindObjectOfType<GPSView>(true);
                if (gpsView != null && !gpsView.isActiveAndEnabled)
                {
                    gpsView.SetTarget(_point);
                    gpsView.gameObject.SetActive(true);
                }
            }

            _point.transform.position = position;
        }
    }
}
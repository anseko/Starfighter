using Core;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace Net.Components
{
    public class WayPointComponent: NetworkBehaviour
    {
        [SerializeField]
        private GameObject _pointPrefab;
        private GameObject _point;
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
                var navigatorClientId = NetworkManager.Singleton.LocalClientId;
                var pilotClientId = GetComponent<NetworkObject>().OwnerClientId;
                Debug.unityLogger.Log($"Ids: {navigatorClientId}, {pilotClientId}");
                Debug.unityLogger.Log($"Camera: {_camera.gameObject.name}");
                MovePointServerRpc(_camera.ScreenToWorldPoint(Input.mousePosition), navigatorClientId, pilotClientId);
            }
        }

        [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        private void MovePointServerRpc(Vector3 position, ulong naviClientId, ulong pilotClientId)
        {
            var clientRpcParams = new ClientRpcParams()
            {
                Send = new ClientRpcSendParams()
                {
                    TargetClientIds = new[] {naviClientId, pilotClientId}
                }
            };
            
            MovePointClientRpc(position, clientRpcParams);
        }
        
        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        private void MovePointClientRpc(Vector3 position, ClientRpcParams clientRpcParams = default)
        {
            if (IsServer) return;
            
            position.Set(position.x, 0,position.z);
            if (_point is null)
            {
                _point = Instantiate(_pointPrefab, position, _pointPrefab.transform.rotation);
                _point.tag = Constants.WayPointTag;
            }

            _point.transform.position = position;
        }
    }
}
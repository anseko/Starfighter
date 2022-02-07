using Client.Core;
using JetBrains.Annotations;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

namespace Net.Components
{
    public class OrderPanelComponent: NetworkBehaviour
    {
        [SerializeField] private GameObject _orderFramePrefab;
        private GameObject _orderFrame;
        private ulong _naviClientId;
        public PlayerScript ship;
        public Vector3 position;
        public Vector3 size;
        public string text;
      
        public void Init(PlayerScript _ship, Vector3 _position, Vector3 _size, string _text)
        {
            ship = _ship;
            position = _position;
            size = _size;
            text = _text;
            GetNaviIDServerRpc(ship.name.Substring(0,ship.name.Length-7));
        }
        
        [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        private void GetNaviIDServerRpc(string _shipName)
        {
            var shipName = _shipName;
            _naviClientId = GetComponent<MainServerLoop>().GetClientIdByShipName(shipName);
            var clientRpcParams = new ClientRpcParams()
            {
                Send = new ClientRpcSendParams()
                {
                    TargetClientIds = new[] {_naviClientId}
                }
            };
            SetPanelClientRpc(position, size, text, clientRpcParams);
        }
        
        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        private void SetPanelClientRpc(Vector3 _position, Vector3 _size, string _text, 
            ClientRpcParams clientRpcParams = default)
        {
            var thisOrdersPanel = GameObject.Find("OrderStaticFrame(Clone)")??Instantiate(_orderFramePrefab);
            thisOrdersPanel.GetComponent<StaticFrameInit>().ClientFrameInit(_position,_size,_text);
            thisOrdersPanel.transform.Find("DestroyButton").gameObject.SetActive(false);
            thisOrdersPanel.transform.Find("EditButtonButton").gameObject.SetActive(false);
        }
    }
}
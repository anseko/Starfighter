using System;
using System.Collections.Generic;
using System.Linq;
using Client.Core;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Serialization;
using Net.Components;
using TMPro;
using UnityEngine;

namespace Client.UI
{
    public class OrdersScript : NetworkBehaviour
    {
        public class OrderUnit: INetworkSerializable
        {
            public string shipName;
            public Vector3 position;
            public Vector3 size;
            public string text;
            public OrderOperation operation;
            public GameObject orderPlane;

            public void NetworkSerialize(NetworkSerializer serializer)
            {
                serializer.Serialize(ref shipName);
                serializer.Serialize(ref position);
                serializer.Serialize(ref size);
                serializer.Serialize(ref text);
                serializer.Serialize(ref operation);
            }
        }
        
        public class POIUnit: INetworkSerializable
        {
            public string identifier;
            public Vector3 position;
            public Vector3 size;
            public string text;
            public OrderOperation operation;
            public GameObject orderPlane;
            
            public void NetworkSerialize(NetworkSerializer serializer)
            {
                serializer.Serialize(ref identifier);
                serializer.Serialize(ref position);
                serializer.Serialize(ref size);
                serializer.Serialize(ref text);
                serializer.Serialize(ref operation);
            }
        }
        
        public enum OrderOperation
        {
            Add,
            Edit,
            Remove
        }

        private Dictionary<string, OrderUnit> _ordersList;

        private PlayerScript _ordersPS;
        public bool isActive;
        public bool isPOI;
        private bool isDrawing;
        private Vector3 _endPositionGlobal;
        private GameObject _orderPlaneCopy;
        [SerializeField] private TMP_InputField _textField;
        [SerializeField] private GUISkin skin;
        [SerializeField] private Rect _ordersFrame;
        [SerializeField] private Vector3 _startPosition, _endPosition;
        [SerializeField] private GameObject _editPanel;
        [SerializeField] private Camera _camera;
        [SerializeField] private GameObject[] _orderPlanePrefab = new GameObject[2];
        [SerializeField] private CursorEngine _cursor;
        [SerializeField] private TMP_Dropdown _shipListDropdown;
        private List<string> _shipNamesList;
        private int _index;
        private int _chosenPrefab;
        private List<PlayerScript> _allShips;
        private POIUnit poiTransmision;

        private void Awake()
        {
            _ordersList = new Dictionary<string, OrderUnit>();
        }

        private void Start()
        {
            isPOI = false;
            _allShips = new List<PlayerScript>();
            _shipNamesList = new List<string>();
        }

        [ServerRpc]
        public void CreatePOIFrameServerRPC(POIUnit unit)
        {
            var targetFrame = FindObjectsOfType<POIFrameInit>().FirstOrDefault(x => x.identificator
                == unit.identifier);
            if (targetFrame)
            {
                unit.operation = OrderOperation.Edit;
            }
            else
            {
                unit.operation = OrderOperation.Add;
            }
            
            if (unit.operation == OrderOperation.Add)
            {
                var thisCopy = Instantiate(_orderPlaneCopy);
                thisCopy.GetComponent<POIFrameInit>().FrameInit(unit.identifier, unit.position, unit.size, 
                    unit.text, true);
                thisCopy.GetComponent<NetworkObject>().Spawn();
                isPOI = false;
            }
            else 
            {
                var currentPOIFrames = FindObjectsOfType<POIFrameInit>();
                targetFrame.FrameInit(unit.identifier, unit.position, unit.size, 
                    unit.text, true);
                isPOI = false;
            }
        }

        [ServerRpc]
        public void DeletePOIFrameServerRPC(string unit)
        {
            Destroy(FindObjectsOfType<POIFrameInit>().FirstOrDefault(x => x.identificator == unit));
        }

        public void SetOrder()
        {
            isPOI = false;
            _chosenPrefab = 0;
            isActive = true;
            Cursor.SetCursor(_cursor.cursorExclamation,new Vector2(0,0), CursorMode.Auto);
        }

        public void SetPOI()
        {
            isPOI = true;
            _chosenPrefab = 1;
            isActive = true;
            Cursor.SetCursor(_cursor.cursorQuestion, new Vector2(0,0), CursorMode.Auto);
        }
    
        public void GetShipList()
        {
            _shipListDropdown = _editPanel.transform.Find("Dropdown").GetComponent<TMP_Dropdown>();
            _shipNamesList = new List<string>();
            var ps = FindObjectsOfType<PlayerScript>();
            Debug.unityLogger.Log($"Number of ships on scene: {ps.Length}");
            
            _allShips = ps.ToList();
            _allShips.ForEach(ship => _shipNamesList.Add(ship.NetworkUnitConfig.ShipId));
            _shipListDropdown.AddOptions(_shipNamesList);
        }

        public void GetAssignedShip()
        {
            var name = _shipListDropdown.options[_shipListDropdown.value].text;
            var ship = FindObjectsOfType<PlayerScript>()
                .FirstOrDefault(ps => ps.NetworkUnitConfig.ShipId == name);
            _ordersPS = ship;
        }
    
        public void CreateOrder()
        {
            if (isPOI)
            {
                poiTransmision = new POIUnit();
                poiTransmision.identifier = _orderPlaneCopy.GetComponent<POIFrameInit>().identificator;
                poiTransmision.position = _orderPlaneCopy.GetComponent<POIFrameInit>().position;
                poiTransmision.size = _orderPlaneCopy.GetComponent<POIFrameInit>().size;
                poiTransmision.text = _textField.text;
                CreatePOIFrameServerRPC(poiTransmision);
            }
            else
            {
                _ordersPS ??= _allShips.FirstOrDefault();
                            
                Debug.unityLogger.Log("Creating Mode");
                var unit = new OrderUnit
                {
                    shipName = _ordersPS?.NetworkUnitConfig.ShipId ?? "Unknown",
                    position = _orderPlaneCopy.GetComponent<OrderFrameInit>().position,
                    size = _orderPlaneCopy.GetComponent<OrderFrameInit>().size,
                    text = _textField.text,
                    orderPlane = _orderPlaneCopy
                };
                
                var isSuccess = _ordersList.TryGetValue(unit.shipName, out var oldOrder);
                Debug.unityLogger.Log($"creating order {isSuccess}: {unit.shipName} : {_ordersList.Count}");
                            
                if (isSuccess)
                {
                    Destroy(oldOrder.orderPlane);
                    _ordersList.Remove(oldOrder.shipName);
                    unit.operation = OrderOperation.Edit;
                }
                else
                {
                    unit.operation = OrderOperation.Add;
                }
                            
                _ordersList.Add(unit.shipName, unit);
                if (_ordersPS.TryGetComponent<OrderComponent>(out var orderComponent))
                    orderComponent.lastOrder.Value = unit;
                            
                _orderPlaneCopy.GetComponent<OrderFrameInit>().FrameInit(_ordersPS, unit.position, unit.size, unit.text);
            }
            
            _textField.text = "";
            isPOI = false;
            isActive = false;
            _editPanel.SetActive(false);
            _camera.GetComponent<CameraMotion>()._isDragable = true;
        }

        private void EnterOrder()
        {
            // _ordersPS ??= _allShips.FirstOrDefault();
            
            isActive = false;
            isDrawing = false;
            _orderPlaneCopy = Instantiate(_orderPlanePrefab[_chosenPrefab]);
            if (_orderPlaneCopy.name.Contains("OrderStaticFrame"))
            {
                _orderPlaneCopy.GetComponent<OrderFrameInit>().FrameInit(
                                _ordersPS,
                                _camera.ScreenToWorldPoint(_startPosition), 
                                _endPositionGlobal, 
                                "");
            }
            else
            {
                isPOI = true;
                var identificator = Guid.NewGuid().ToString();
                _orderPlaneCopy.GetComponent<POIFrameInit>().FrameInit(
                    identificator,
                    _camera.ScreenToWorldPoint(_startPosition), 
                    _endPositionGlobal, 
                    "");
            }
            Debug.unityLogger.Log("editing");
            _editPanel.SetActive(true);
            Debug.unityLogger.Log("panel is active");
        }
    
        public void CancelOrder()
        {
            if (_orderPlaneCopy.name.Contains("POI"))
            {
                string thisPlane = _orderPlaneCopy.GetComponent<POIFrameInit>().identificator;
                DeletePOIFrameServerRPC(thisPlane);
            }
            
            if (_ordersPS is null) return;
            
            if (_ordersList.TryGetValue(_ordersPS.NetworkUnitConfig.ShipId, out var orderUnit))
            {
                if (_ordersPS.TryGetComponent<OrderComponent>(out var orderComponent))
                {
                    orderUnit.operation = OrderOperation.Remove;
                    orderComponent.lastOrder.Value = orderUnit;
                }

                _ordersList.Remove(_ordersPS.NetworkUnitConfig.ShipId);
            }

            Destroy(_orderPlaneCopy);
            _editPanel.SetActive(false);
            isActive = false;
            isPOI = false;
            _camera.GetComponent<CameraMotion>()._isDragable = true;
        }

        public void EditOrder(OrderFrameInit parent)
        {
            _orderPlaneCopy = parent.gameObject;
            _editPanel.SetActive(true);
            _textField.text = parent.text;
        }

        private void OnGUI()
        {
            GUI.skin = skin;
            GUI.depth = 99;

            if (!isActive) return;
            
            if (Input.GetMouseButtonDown(0) && !_editPanel.activeInHierarchy)
            {
                Debug.unityLogger.Log("start drawing");
                _startPosition = Input.mousePosition;
                isDrawing = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                Debug.unityLogger.Log("end drawing");
                isDrawing = false;
                Cursor.SetCursor(_cursor.cursor, new Vector2(0,0), CursorMode.Auto);
                if (_ordersFrame.size.magnitude >= 50)
                {
                    EnterOrder();
                }
                else
                {
                    CancelOrder();
                }
            }

            if (isDrawing)
            {
                _camera.GetComponent<CameraMotion>()._isDragable = false;
                _endPosition = Input.mousePosition;
                _ordersFrame = new Rect(Mathf.Min(_endPosition.x, _startPosition.x),
                    Screen.height - Mathf.Max(_endPosition.y, _startPosition.y),
                    Mathf.Max(_endPosition.x, _startPosition.x) - Mathf.Min(_endPosition.x, _startPosition.x),
                    Mathf.Max(_endPosition.y, _startPosition.y) - Mathf.Min(_endPosition.y, _startPosition.y)
                );
                _endPositionGlobal = _camera.ScreenToWorldPoint(_endPosition);
                GUI.Box(_ordersFrame, "");
            }
        }
    }
}
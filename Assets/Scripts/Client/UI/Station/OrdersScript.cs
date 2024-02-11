using System;
using System.Collections.Generic;
using System.Linq;
using Client.Core;
using Mirror;
using Net.Components;
using TMPro;
using UnityEngine;

namespace Client.UI
{
    public class OrdersScript : NetworkBehaviour
    {
        [Serializable]
        public class OrderUnit
        {
            public string shipName;
            public Vector3 position;
            public Vector3 size;
            public string text;
            public OrderOperation operation;
            public GameObject orderPlane;

            //should be unnecessary
            // public void NetworkSerialize(NetworkSerializer serializer)
            // {
            //     serializer.Serialize(ref shipName);
            //     serializer.Serialize(ref position);
            //     serializer.Serialize(ref size);
            //     serializer.Serialize(ref text);
            //     serializer.Serialize(ref operation);
            // }
        }

        public enum OrderOperation
        {
            Add,
            Edit,
            Remove
        }

        public enum EditorState
        {
            IsInactive,
            IsActive,
            IsDrawing
        }

        public EditorState state;

        private Dictionary<string, OrderUnit> _ordersList;
        private PlayerScript _ordersPS;
        private Vector3 _endPositionGlobal;
        private GameObject _orderPlaneCopy;
        [SerializeField] private TMP_InputField _textField;
        [SerializeField] private GUISkin skin;
        [SerializeField] private Rect _ordersFrame;
        [SerializeField] private Vector3 _startPosition, _endPosition;
        [SerializeField] private GameObject _editPanel;
        [SerializeField] private Camera _camera;
        [SerializeField] private GameObject _orderPlanePrefab;
        [SerializeField] private CursorEngine _cursor;
        [SerializeField] private TMP_Dropdown _shipListDropdown;

        private List<string> _shipNamesList;
        private int _index;
        private List<PlayerScript> _allShips;

        private void Awake()
        {
            _ordersList = new Dictionary<string, OrderUnit>();
        }

        private void Start()
        {
            _allShips = new List<PlayerScript>();
            _shipNamesList = new List<string>();
        }

        public void SetOrder()
        {
            state = EditorState.IsActive;
            Cursor.SetCursor(_cursor.cursorExclamation, new Vector2(0, 0), CursorMode.Auto);
        }


        public void GetShipList()
        {
            _shipListDropdown = _editPanel.transform.Find("Dropdown").GetComponent<TMP_Dropdown>();
            _shipNamesList = new List<string>();
            var ps = FindObjectsOfType<PlayerScript>();

            _allShips = ps.ToList();
            _allShips.ForEach(ship => _shipNamesList.Add(ship.networkUnitConfig.shipId));
            _shipListDropdown.AddOptions(_shipNamesList);
        }

        public void GetAssignedShip()
        {
            var name = _shipListDropdown.options[_shipListDropdown.value].text;
            var ship = FindObjectsOfType<PlayerScript>()
                .FirstOrDefault(ps => ps.networkUnitConfig.shipId == name);
            _ordersPS = ship;
        }

        public void CreateOrder()
        {
            state = EditorState.IsInactive;
            if (_ordersPS is null) GetAssignedShip();

            var unit = new OrderUnit
            {
                shipName = _ordersPS?.networkUnitConfig.shipId ?? "Unknown",
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
                orderComponent.lastOrder = unit;

            _orderPlaneCopy.GetComponent<OrderFrameInit>().FrameInit(_ordersPS, unit.position, unit.size, unit.text);

            _textField.text = "";
            _editPanel.SetActive(false);
            _camera.GetComponent<CameraMotion>()._isDragable = true;
        }

        private void EnterOrder()
        {
            if (_ordersPS is null) GetAssignedShip();

            state = EditorState.IsInactive;
            _orderPlaneCopy = Instantiate(_orderPlanePrefab);

            _orderPlaneCopy.GetComponent<OrderFrameInit>().FrameInit(
                _ordersPS,
                _camera.ScreenToWorldPoint(_startPosition),
                _endPositionGlobal,
                "");

            _editPanel.SetActive(true);
        }

        public void CancelOrder()
        {
            if (_ordersPS is null) return;

            state = EditorState.IsInactive;

            if (_ordersList.TryGetValue(_ordersPS.networkUnitConfig.shipId, out var orderUnit))
            {
                if (_ordersPS.TryGetComponent<OrderComponent>(out var orderComponent))
                {
                    orderUnit.operation = OrderOperation.Remove;
                    orderComponent.lastOrder = orderUnit;
                }

                _ordersList.Remove(_ordersPS.networkUnitConfig.shipId);
            }

            Destroy(_orderPlaneCopy);
            _editPanel.SetActive(false);
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

            switch (state)
            {
                case EditorState.IsInactive:
                    return;

                case EditorState.IsActive:

                    if (Input.GetMouseButtonDown(0) && !_editPanel.activeInHierarchy)
                    {
                        _startPosition = Input.mousePosition;
                        state = EditorState.IsDrawing;
                    }

                    break;

                case EditorState.IsDrawing:

                    _camera.GetComponent<CameraMotion>()._isDragable = false;
                    _endPosition = Input.mousePosition;
                    _ordersFrame = new Rect(Mathf.Min(_endPosition.x, _startPosition.x),
                        Screen.height - Mathf.Max(_endPosition.y, _startPosition.y),
                        Mathf.Max(_endPosition.x, _startPosition.x) - Mathf.Min(_endPosition.x, _startPosition.x),
                        Mathf.Max(_endPosition.y, _startPosition.y) - Mathf.Min(_endPosition.y, _startPosition.y)
                    );
                    _endPositionGlobal = _camera.ScreenToWorldPoint(_endPosition);
                    GUI.Box(_ordersFrame, "");

                    if (Input.GetMouseButtonUp(0))
                    {
                        Cursor.SetCursor(_cursor.cursor, new Vector2(0, 0), CursorMode.Auto);
                        if (_ordersFrame.size.magnitude >= 50)
                        {
                            EnterOrder();
                        }
                        else
                        {
                            CancelOrder();
                        }
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }


        }
    }
}
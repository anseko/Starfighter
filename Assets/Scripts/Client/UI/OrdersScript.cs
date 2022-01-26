using System.Collections.Generic;
using Client;
using Client.Core;
using TMPro;
using UnityEngine;

public class OrdersScript : MonoBehaviour
{
    //private PlayerScript _ordersPS;
    public bool isActive;
    public bool isOrder;
    private bool isDrawing;
    private Vector3 _endPositionGlobal;
    private GameObject _orderPlaneCopy;
    [SerializeField] private TMP_InputField _textField;
    [SerializeField] private GUISkin skin;
    [SerializeField] private Rect _ordersFrame;
    [SerializeField] private Vector3 _startPosition, _endPosition;
    [SerializeField] private GameObject _editPanel;
    [SerializeField] private Camera _camera;
    [SerializeField] private StaticFrameInit _orderPlanePrefab;
    [SerializeField] private TMP_Dropdown _dropdown;
    private List<PlayerScript> _shipList;
    private List<TMP_Dropdown.OptionData> _shipListData;
    private int _index;

    void Start()
    {
        _shipListData = GetShipList();
        _dropdown.AddOptions(_shipListData);
    }

    private List<TMP_Dropdown.OptionData> GetShipList()
    {
        _shipList = new List<PlayerScript>(GameObject.FindObjectsOfType<PlayerScript>());
        for (int i = 0; i < _shipList.Count; i++)
        {
            _shipListData[i].text = _shipList[i].gameObject.name.Substring(0,
                _shipList[i].gameObject.name.Length - 6);
        }

        return _shipListData;
    }
    
    public void CreateOrder()
    {
        Debug.Log("Creating Mode");
        _orderPlaneCopy.GetComponent<StaticFrameInit>().FrameInit(
            _orderPlaneCopy.GetComponent<StaticFrameInit>().position,
            _orderPlaneCopy.GetComponent<StaticFrameInit>().size,
            _textField.text);
        _textField.text = "";
        isOrder = false;
        _editPanel.SetActive(false);
        isActive = false;
        _camera.GetComponent<CameraMotion>()._isDragable = true;
    }

    void EnterOrder()
    {
        isOrder = true;
        isActive = false;
        isDrawing = false;
        _orderPlaneCopy = Instantiate(_orderPlanePrefab.gameObject);
        _orderPlaneCopy.GetComponent<StaticFrameInit>().FrameInit( /*_ordersPS,*/
            _camera.ScreenToWorldPoint(_startPosition), 
            _endPositionGlobal, 
            "");
        Debug.Log("editing");
        _editPanel.SetActive(true);
        Debug.Log("panel is active");
    }
    
    public void CancelOrder()
    {
        _orderPlaneCopy.GetComponent<Destructor>().Destroy();
        _editPanel.SetActive(false);
        isActive = false;
        _camera.GetComponent<CameraMotion>()._isDragable = true;
    }

    public void EditOrder([SerializeField] StaticFrameInit _parent)
    {
        GetComponent<POIScript>().isPOI = false;
        isOrder = true;
        _orderPlaneCopy = _parent.gameObject;
        _editPanel.SetActive(true);
        _textField.text = _parent.text;
        Debug.Log(_parent.text,_parent);
        Debug.Log(_textField.text,_textField);
        
        _dropdown.SetValueWithoutNotify(_index);
    }

    private void OnGUI()
    {
        GUI.skin = skin;
        GUI.depth = 99;

        if (isActive)
        {
            
            if (Input.GetMouseButtonDown(0) && !_editPanel.activeInHierarchy)
            {
                Debug.Log("start drawing");
                _startPosition = Input.mousePosition;
                isDrawing = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("end drawing");
                isDrawing = false;
                Cursor.SetCursor(GetComponent<CursorEngine>().cursor, new Vector2(0,0), CursorMode.Auto);
                if (!(_ordersFrame.size.magnitude < 50))
                {
                    EnterOrder();
                }
                else CancelOrder();
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

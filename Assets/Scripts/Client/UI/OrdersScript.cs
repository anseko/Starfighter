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
    [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private CursorEngine _cursor;
    private List<PlayerScript> _shipList;
    private List<TMP_Dropdown.OptionData> _shipListData;
    private int _index;
    private int _chosenPrefab;
    
    void Start()
    {
        _shipListData = GetShipList();
        _dropdown.AddOptions(_shipListData);
        isOrder = false;
        isPOI = false;
    }

    public void SetOrder()
    {
        isOrder = true;
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
    
    private List<TMP_Dropdown.OptionData> GetShipList()
    {
        _shipList = new List<PlayerScript>(GameObject.FindObjectsOfType<PlayerScript>());
        for (int i = 0; i < _shipList.Count; i++)
        {
            _shipListData[i].text = _shipList[i].gameObject.name.Substring(0,
                _shipList[i].gameObject.name.Length - 7);
        }

        Debug.Log(_shipListData);
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
        isPOI = false;
        isActive = false;
        _editPanel.SetActive(false);
        _camera.GetComponent<CameraMotion>()._isDragable = true;
    }

    void EnterOrder()
    {
        isActive = false;
        isDrawing = false;
        _orderPlaneCopy = Instantiate(_orderPlanePrefab[_chosenPrefab]);
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
        _orderPlaneCopy.GetComponent<Editor>().Destroy();
        _editPanel.SetActive(false);
        isActive = false;
        isOrder = false;
        isPOI = false;
        _camera.GetComponent<CameraMotion>()._isDragable = true;
    }

    public void EditOrder(StaticFrameInit _parent)
    {
        GetComponent<OrdersScript>().isOrder = false;
        _orderPlaneCopy = _parent.gameObject;
        _editPanel.SetActive(true);
        _textField.text = _parent.text;

        //_dropdown.SetValueWithoutNotify(_index);
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
                Cursor.SetCursor(_cursor.cursor, new Vector2(0,0), CursorMode.Auto);
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

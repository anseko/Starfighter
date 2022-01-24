using System.Collections.Generic;
using Client;
using Client.Core;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class OrdersScript : MonoBehaviour
{
    //private PlayerScript _ordersPS;
    public bool isActive;
    private bool isDrawing;
    private Vector3 _endPositionGlobal;
    private GameObject _orderPlaneCopy;
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private GUISkin skin;
    [SerializeField] private string _ordersText;
    [SerializeField] private Rect _ordersFrame;
    [SerializeField] private Vector3 _startPosition, _endPosition;
    [SerializeField] private GameObject _editPanel;
    [SerializeField] private Camera _camera;
    [SerializeField] private OrderStaticFrameInit _orderPlanePrefab;
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
        _orderPlaneCopy = Instantiate(_orderPlanePrefab.gameObject);
        _orderPlaneCopy.GetComponent<OrderStaticFrameInit>().OrderFrameInit(/*_ordersPS,*/ 
            _camera.ScreenToWorldPoint(_startPosition), _endPositionGlobal, _ordersText);
        textField.text = "";
        _editPanel.SetActive(false);
        isActive = false;
        _camera.GetComponent<CameraMotion>()._isDragable = true;
    }

    void EnterOrder()
    {
        isDrawing = false;
        Debug.Log("editing");
        _editPanel.SetActive(true);
        Debug.Log("panel is active");
        _ordersText = textField.text;
    }
    
    public void CancelOrder()
    {
        _editPanel.SetActive(false);
        isActive = false;
        _camera.GetComponent<CameraMotion>()._isDragable = true;
    }

    public void EditOrder(string _text/*, PlayerScript _ship*/)
    {
        _editPanel.SetActive(true);
        textField.text = _text;

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
                if (!(_ordersFrame.size.magnitude < 1))
                {
                    EnterOrder();
                }
            }

            if (isDrawing)
            {
                _camera.GetComponent<CameraMotion>()._isDragable = false;
                _endPosition = Input.mousePosition;
                //if(_startPosition == _endPosition) return;

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

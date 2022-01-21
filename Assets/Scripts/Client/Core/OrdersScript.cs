using Client;
using Client.Core;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

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

    public void EditOrder()
    {
        
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

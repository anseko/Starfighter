using TMPro;
using UnityEngine;

public class raycastScript : MonoBehaviour
{
    RaycastHit hit;
    private Camera _camera;
    [SerializeField] private TMP_Text _textContainer;
    
    void Start()
    {
        _camera = GameObject.Find("SpaceStationCamera").GetComponent<Camera>();
    }
    
    void Update()
    {
        Ray myRay = _camera.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(myRay, out hit, 100500,layerMask:1 << 15))
            {
                GameObject go = hit.collider.gameObject;
                
                if (go.name == "DestroyButton")
                {
                    go.transform.parent.GetComponent<Editor>().Destroy();
                }

                if (go.name == "EditButton")
                {
                    go.transform.parent.GetComponent<Editor>().Edit();
                }
            }
        }
        
        if (Physics.Raycast(myRay, out hit, 100500, layerMask:1 << 15))
        {
            GameObject go = hit.collider.gameObject;
            Debug.Log(go.name);
            if ((go.name == "OrderStaticFrame(Clone)") || ((go.name == "POIStaticFrame(Clone)")))
            {
                _textContainer.transform.position = Input.mousePosition;
                _textContainer.text = go.GetComponent<StaticFrameInit>().text;
            }
            else if (go.name == "DestroyButton")
            {
                _textContainer.text = "";
            }
            else if (go.name == "EditButton")
            {
                _textContainer.text = "";
            }
            
        }
        else
        {
            _textContainer.text = "";
        }
    }
}

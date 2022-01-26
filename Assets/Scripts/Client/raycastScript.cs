using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class raycastScript : MonoBehaviour
{
    RaycastHit hit;
    private Camera _camera;
    
    void Start()
    {
        _camera = GameObject.Find("SpaceStationCamera").GetComponent<Camera>();
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray myRay = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(myRay, out hit, 100500))
            {
                GameObject go = hit.collider.gameObject;
                if (go.name == "DestroyButton")
                {
                    go.transform.parent.GetComponent<Destructor>().Destroy();
                }

                if (go.name == "EditButton")
                {
                    go.transform.parent.GetComponent<Editor>().Edit();
                }
            }
        }
    }
}

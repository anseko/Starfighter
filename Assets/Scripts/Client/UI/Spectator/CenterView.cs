using TMPro;
using UnityEngine;

public class CenterView : MonoBehaviour

{
    private GameObject _ship;
    [SerializeField] private Camera _camera;
    
    private void Start()
    {
        _camera = transform.root.gameObject.GetComponentInChildren<Camera>();
        _ship = GameObject.Find(GetComponentInChildren<TextMeshProUGUI>().text+"(Clone)");
    }
    
    public void Spectate()
    {
        _camera.transform.position = new Vector3(_ship.transform.position.x,100,_ship.transform.position.z);
    }
}

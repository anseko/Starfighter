using TMPro;
using UnityEngine;

public class CenterView : MonoBehaviour

{
    private GameObject _ship;
    [SerializeField] private Camera _camera;
    
    private void Start()
    {
        _camera = transform.root.gameObject.GetComponentInChildren<Camera>();
        _ship = GameObject.Find(GetComponentInChildren<TextMeshProUGUI>().text);
    }
    
    public void Spectate()
    {
        _camera.transform.position = _ship.transform.position;
    }
}

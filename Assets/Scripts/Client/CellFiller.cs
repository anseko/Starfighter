using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CellFiller : MonoBehaviour
{
    [SerializeField] private Vector3[] _points, _position;
    [SerializeField] private GameObject _grid, _cell;
    [SerializeField] private LineRenderer _renderer;
    [SerializeField] private Camera _cam;
    
    public void Start()
    {
        _grid = GameObject.Find("Grid");
        _cam = FindObjectOfType<Camera>();
        _renderer.loop = true;
        _points = new Vector3[4];
        _points[0] = transform.position;
        _points[1] = transform.position + new Vector3(_grid.GetComponent<GridLayoutGroup>().cellSize.x,0,0);
        _points[2] = _points[1] + new Vector3(0, _grid.GetComponent<GridLayoutGroup>().cellSize.y, 0);
        _points[3] = transform.position + new Vector3(0,_grid.GetComponent<GridLayoutGroup>().cellSize.y,0);
    }

    void Update()
    {
        var camRatio = _cam.orthographicSize / 100;
        //_renderer.SetWidth(camRatio,camRatio);
        _renderer.SetPositions(_points);
    }
}

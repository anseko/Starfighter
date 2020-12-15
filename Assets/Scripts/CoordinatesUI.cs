using UnityEngine;
using UnityEngine.UI;
using System;

public class CoordinatesUI : MonoBehaviour
{
    // Start is called before the first frame update
    Text pointsText;
    private Vector3 coordinates;
    void Start()
    {
        pointsText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        coordinates = (GameObject.Find("Player").transform.position);
        pointsText.text = "Координаты: x " + Math.Round(coordinates.x,0) + " y " + Math.Round(coordinates.z,0); // вывод в UI
    }
}

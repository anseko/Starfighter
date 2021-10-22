using System;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI
{
    public class DataOutput : BasePlayerUIElement
    {
        // Start is called before the first frame update
        private Text _pointsText;
        private float _rotSpeed;
        private float _roundSpeed;

        private void Awake()
        {
        }

        private void Start()
        {
            _pointsText = GetComponent<Text>();
            _rotSpeed = 0;
            _roundSpeed = 0;
        }

        // Update is called once per frame
        private void Update()
        {
            _roundSpeed = (float)Math.Round(PlayerScript.shipSpeed.Value.magnitude, 2); // скорость полета
            _rotSpeed = (float)Math.Round(PlayerScript.shipRotation.Value.magnitude * Mathf.Rad2Deg, 2); //скорость врещения в градусах
            _pointsText.text = "Скорость: " + _roundSpeed.ToString() + "\n" + "Вращение: " + _rotSpeed.ToString();// вывод в UI
        }
    }
}

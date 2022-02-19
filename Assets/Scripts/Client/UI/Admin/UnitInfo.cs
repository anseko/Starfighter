using System.Globalization;
using Client.Core;
using MLAPI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI.Admin
{
    public class UnitInfo: NetworkBehaviour
    {
        private UnitScript _unitScript;
        [SerializeField] private Text _unitName;
        [SerializeField] private Slider _hp;
        [SerializeField] private Button _applyButton;
        [SerializeField] private Button _focusButton;
        [SerializeField] private TMP_InputField _maxSpeed;
        [SerializeField] private TMP_InputField _maxAngleSpeed;
        [SerializeField] private TMP_InputField _maxHp;
        
        private void Awake()
        {
            _applyButton.onClick.AddListener(Apply);
            _focusButton.onClick.AddListener(() =>
            {
                var camMotion = FindObjectOfType<Camera>().GetComponent<CameraMotion>();
                camMotion.Player = _unitScript.gameObject;
                camMotion.gameObject.transform.position = _unitScript.gameObject.transform.position + Vector3.up * 90;;
            });
        }

        public void Init(UnitScript playerScript)
        {
            _unitScript = playerScript;

            _unitName.text = _unitScript.NetworkUnitConfig.ShipId;
            
            _maxHp.text = _unitScript.NetworkUnitConfig.MaxHp.ToString(CultureInfo.InvariantCulture);
            _maxHp.onValueChanged.AddListener((arg0 => _hp.maxValue = float.Parse(arg0)));
            
            _hp.maxValue = _unitScript.NetworkUnitConfig.MaxHp;
            _hp.value = _unitScript.NetworkUnitConfig.CurrentHp;
            _unitScript.NetworkUnitConfig._currentHp.OnValueChanged +=
                (value, newValue) => _hp.value = newValue;
        }

        private void Apply()
        {
            _unitScript.NetworkUnitConfig.MaxHp = float.Parse(_maxHp.text);
            _unitScript.NetworkUnitConfig.CurrentHp = _hp.value;
            _unitScript.NetworkUnitConfig.MaxSpeed = float.Parse(_maxSpeed.text);
            _unitScript.NetworkUnitConfig.MaxAngleSpeed = float.Parse(_maxAngleSpeed.text);
        }
    }
}
using System.Globalization;
using Client.Core;
using Core;
using MLAPI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI.Admin
{
    public class ShipInfo: NetworkBehaviour
    {
        private PlayerScript _playerScript;
        [SerializeField] private Text _shipName;
        [SerializeField] private Slider _hp;
        [SerializeField] private Slider _stress;
        [SerializeField] private Dropdown _state;
        [SerializeField] private Button _applyButton;
        [SerializeField] private Button _refreshButton;
        [SerializeField] private Button _focusButton;
        [SerializeField] private TMP_InputField _maxSpeed;
        [SerializeField] private TMP_InputField _maxAngleSpeed;
        [SerializeField] private TMP_InputField _maxStress;
        [SerializeField] private TMP_InputField _maxHp;


        private void Awake()
        {
            _applyButton.onClick.AddListener(Apply);
            _refreshButton.onClick.AddListener(UpdateCurrentValues);
            _focusButton.onClick.AddListener(() =>
            {
                var camMotion = FindObjectOfType<Camera>().GetComponent<CameraMotion>();
                camMotion.Player = _playerScript.gameObject;
                camMotion.gameObject.transform.position = _playerScript.gameObject.transform.position + Vector3.up * 90;;
                // if (camMotion.GetFollowMode()) camMotion.SwitchFollowMode();
            });
        }

        public void Init(PlayerScript playerScript)
        {
            _playerScript = playerScript;

            _shipName.text = _playerScript.NetworkUnitConfig.ShipId;
            
            _maxHp.text = _playerScript.NetworkUnitConfig.MaxHp.ToString(CultureInfo.InvariantCulture);
            _maxHp.onValueChanged.AddListener((arg0 => _hp.maxValue = float.Parse(arg0)));
            
            _hp.maxValue = _playerScript.NetworkUnitConfig.MaxHp;
            _hp.value = _playerScript.NetworkUnitConfig.CurrentHp;
            // _playerScript.NetworkUnitConfig._currentHp.OnValueChanged +=
            //     (value, newValue) => _hp.value = newValue; 
            
            _maxStress.text = _playerScript.NetworkUnitConfig.MaxStress.ToString(CultureInfo.InvariantCulture);
            _maxStress.onValueChanged.AddListener((arg0 => _stress.maxValue = float.Parse(arg0)));
            
            _stress.maxValue = _playerScript.NetworkUnitConfig.MaxStress;
            _stress.value = _playerScript.NetworkUnitConfig.CurrentStress;
            // _playerScript.NetworkUnitConfig._currentStress.OnValueChanged +=
            //     (value, newValue) => _stress.value = newValue; 

            _state.value = (int)_playerScript.NetworkUnitConfig.ShipState;
            _playerScript.NetworkUnitConfig._shipState.OnValueChanged += (value, newValue) => _state.value = (int)newValue;
            
            _maxSpeed.text = _playerScript.NetworkUnitConfig.MaxSpeed.ToString(CultureInfo.InvariantCulture);
            _maxAngleSpeed.text = _playerScript.NetworkUnitConfig.MaxAngleSpeed.ToString(CultureInfo.InvariantCulture);
        }

        private void Apply()
        {
            _playerScript.NetworkUnitConfig.CurrentHp = _hp.value;
            _playerScript.NetworkUnitConfig.CurrentStress = _stress.value;
            _playerScript.NetworkUnitConfig.MaxSpeed = float.Parse(_maxSpeed.text);
            _playerScript.NetworkUnitConfig.MaxAngleSpeed = float.Parse(_maxAngleSpeed.text);
            _playerScript.NetworkUnitConfig.ShipState = (UnitState)_state.value;
            _playerScript.NetworkUnitConfig.MaxHp = float.Parse(_maxHp.text);
            _playerScript.NetworkUnitConfig.MaxStress = float.Parse(_maxStress.text);
        }

        private void UpdateCurrentValues()
        {
            _stress.value = _playerScript.NetworkUnitConfig.CurrentStress;
            _hp.value = _playerScript.NetworkUnitConfig.CurrentHp;
        }
    }
}
using System.Globalization;
using Client.Core;
using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI.Admin
{
    public class ShipInfo: MonoBehaviour
    {
        public PlayerScript playerScript { get; private set; }
        [SerializeField] private Text _shipName;
        [SerializeField] private Slider _hp;
        [SerializeField] private Slider _stress;
        [SerializeField] private Dropdown _state;
        [SerializeField] private Button _applyButton;
        [SerializeField] private Button _refreshButton;
        [SerializeField] private Button _despawnButton;
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
                camMotion.Player = playerScript.gameObject;
                camMotion.gameObject.transform.position = playerScript.gameObject.transform.position + Vector3.up * 90;;
                // if (camMotion.GetFollowMode()) camMotion.SwitchFollowMode();
            });
            _despawnButton.onClick.AddListener(Despawn);
        }

        public void Init(PlayerScript playerScript)
        {
            this.playerScript = playerScript;

            _shipName.text = this.playerScript.NetworkUnitConfig.ShipId;
            
            _maxHp.text = this.playerScript.NetworkUnitConfig.MaxHp.ToString(CultureInfo.InvariantCulture);
            _maxHp.onValueChanged.AddListener((arg0 => _hp.maxValue = float.Parse(arg0)));
            
            _hp.maxValue = this.playerScript.NetworkUnitConfig.MaxHp;
            _hp.value = this.playerScript.NetworkUnitConfig.CurrentHp;
            // _playerScript.NetworkUnitConfig._currentHp.OnValueChanged +=
            //     (value, newValue) => _hp.value = newValue; 
            
            _maxStress.text = this.playerScript.NetworkUnitConfig.MaxStress.ToString(CultureInfo.InvariantCulture);
            _maxStress.onValueChanged.AddListener((arg0 => _stress.maxValue = float.Parse(arg0)));
            
            _stress.maxValue = this.playerScript.NetworkUnitConfig.MaxStress;
            _stress.value = this.playerScript.NetworkUnitConfig.CurrentStress;
            // _playerScript.NetworkUnitConfig._currentStress.OnValueChanged +=
            //     (value, newValue) => _stress.value = newValue; 

            _state.value = (int)this.playerScript.NetworkUnitConfig.ShipState;
            this.playerScript.NetworkUnitConfig._shipState.OnValueChanged += (value, newValue) => _state.value = (int)newValue;
            
            _maxSpeed.text = this.playerScript.NetworkUnitConfig.MaxSpeed.ToString(CultureInfo.InvariantCulture);
            _maxAngleSpeed.text = this.playerScript.NetworkUnitConfig.MaxAngleSpeed.ToString(CultureInfo.InvariantCulture);
        }

        private void Apply()
        {
            playerScript.NetworkUnitConfig.CurrentHp = _hp.value;
            playerScript.NetworkUnitConfig.CurrentStress = _stress.value;
            playerScript.NetworkUnitConfig.MaxSpeed = float.Parse(_maxSpeed.text);
            playerScript.NetworkUnitConfig.MaxAngleSpeed = float.Parse(_maxAngleSpeed.text);
            playerScript.NetworkUnitConfig.ShipState = (UnitState)_state.value;
            playerScript.NetworkUnitConfig.MaxHp = float.Parse(_maxHp.text);
            playerScript.NetworkUnitConfig.MaxStress = float.Parse(_maxStress.text);
        }

        private void UpdateCurrentValues()
        {
            _stress.value = playerScript.NetworkUnitConfig.CurrentStress;
            _hp.value = playerScript.NetworkUnitConfig.CurrentHp;
        }

        private void Despawn()
        {
            var spawner = FindObjectOfType<Spawner>();
            spawner.selectedPrefab = playerScript.gameObject;
            spawner.Despawn();
            Destroy(gameObject);
        }
    }
}
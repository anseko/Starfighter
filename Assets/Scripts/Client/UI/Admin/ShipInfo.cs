using System.Globalization;
using Client.Core;
using Core;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI.Admin
{
    public class ShipInfo: NetworkBehaviour
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
        [SerializeField] private TMP_InputField _radarRange;
        [SerializeField] private TMP_InputField _radiationResist;
        [SerializeField] private TMP_InputField _hitResist;
        [SerializeField] private TMP_InputField _acceleration;

        private SubmitMenu _submitMenu;
        
        private void Awake()
        {
            _applyButton.onClick.AddListener(ApplyCommand);
            _refreshButton.onClick.AddListener(UpdateCurrentValuesCommand);
            _focusButton.onClick.AddListener(() =>
            {
                var camMotion = FindObjectOfType<Camera>().GetComponent<CameraMotion>();
                camMotion.Player = playerScript.gameObject;
                camMotion.gameObject.transform.position = playerScript.gameObject.transform.position + Vector3.up * 90;;
                // if (camMotion.GetFollowMode()) camMotion.SwitchFollowMode();
            });
            _despawnButton.onClick.AddListener(Despawn);
        }

        public void Init(PlayerScript playerScript, SubmitMenu submitMenu)
        {
            this.playerScript = playerScript;
            _submitMenu = submitMenu;

            _shipName.text = this.playerScript.networkUnitConfig.shipId;
            
            _maxHp.text = this.playerScript.networkUnitConfig.maxHp.ToString(CultureInfo.InvariantCulture);
            _maxHp.onValueChanged.AddListener((arg0 => _hp.maxValue = float.Parse(arg0)));
            
            _hp.maxValue = this.playerScript.networkUnitConfig.maxHp;
            _hp.value = this.playerScript.networkUnitConfig.currentHp;
            // _playerScript.NetworkUnitConfig._currentHp.OnValueChanged +=
            //     (value, newValue) => _hp.value = newValue; 
            
            _maxStress.text = this.playerScript.networkUnitConfig.maxStress.ToString(CultureInfo.InvariantCulture);
            _maxStress.onValueChanged.AddListener((arg0 => _stress.maxValue = float.Parse(arg0)));
            
            _stress.maxValue = this.playerScript.networkUnitConfig.maxStress;
            _stress.value = this.playerScript.networkUnitConfig.currentStress;
            // _playerScript.NetworkUnitConfig._currentStress.OnValueChanged +=
            //     (value, newValue) => _stress.value = newValue; 

            _state.value = (int)this.playerScript.networkUnitConfig.shipState;
            
            ClientEventStorage.GetInstance().OnShipStateChange.AddListener((oldState, newState) => _state.value = (int)newState);

            _maxSpeed.text = this.playerScript.networkUnitConfig.maxSpeed.ToString(CultureInfo.InvariantCulture);
            _maxAngleSpeed.text = this.playerScript.networkUnitConfig.maxAngleSpeed.ToString(CultureInfo.InvariantCulture);
            
            _acceleration.text = playerScript.networkUnitConfig.accelerationCoefficient.ToString();
            _radarRange.text = playerScript.networkUnitConfig.radarRange.ToString();
            _radiationResist.text = playerScript.networkUnitConfig.radResistanceCoefficient.ToString();
            _hitResist.text = playerScript.networkUnitConfig.physResistanceCoefficient.ToString();
        }

        [Command]
        private void ApplyCommand()
        {
            playerScript.networkUnitConfig.currentHp = _hp.value;
            playerScript.networkUnitConfig.currentStress = _stress.value;
            playerScript.networkUnitConfig.maxSpeed = float.Parse(_maxSpeed.text);
            playerScript.networkUnitConfig.maxAngleSpeed = float.Parse(_maxAngleSpeed.text);
            playerScript.networkUnitConfig.shipState = (UnitState)_state.value;
            playerScript.networkUnitConfig.maxHp = float.Parse(_maxHp.text);
            playerScript.networkUnitConfig.maxStress = float.Parse(_maxStress.text);
            playerScript.networkUnitConfig.accelerationCoefficient = float.Parse(_acceleration.text);
            playerScript.networkUnitConfig.radarRange = float.Parse(_radarRange.text);
            playerScript.networkUnitConfig.radResistanceCoefficient = float.Parse(_radiationResist.text);
            playerScript.networkUnitConfig.physResistanceCoefficient = float.Parse(_hitResist.text);
        }

        [Command]
        private void UpdateCurrentValuesCommand()
        {
            _stress.value = playerScript.networkUnitConfig.currentStress;
            _hp.value = playerScript.networkUnitConfig.currentHp;
            _acceleration.text = playerScript.networkUnitConfig.accelerationCoefficient.ToString(CultureInfo.InvariantCulture);
            _radarRange.text = playerScript.networkUnitConfig.radarRange.ToString(CultureInfo.InvariantCulture);
            _radiationResist.text = playerScript.networkUnitConfig.radResistanceCoefficient.ToString(CultureInfo.InvariantCulture);
            _hitResist.text = playerScript.networkUnitConfig.physResistanceCoefficient.ToString(CultureInfo.InvariantCulture);
        }

        private void Despawn()
        {
            _submitMenu.RaiseSubmit(() =>
            {
                var spawner = FindObjectOfType<Spawner>();
                spawner.selectedPrefab = playerScript.gameObject;
                spawner.Despawn();
                Destroy(gameObject);
            });
        }
    }
}
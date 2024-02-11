using System.Globalization;
using Client.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI.Admin
{
    public class UnitInfo: MonoBehaviour
    {
        public UnitScript unitScript { get; private set; }
        [SerializeField] private Text _unitName;
        [SerializeField] private Slider _hp;
        [SerializeField] private Button _applyButton;
        [SerializeField] private Button _focusButton;
        [SerializeField] private Button _despawnButton;
        [SerializeField] private TMP_InputField _maxSpeed;
        [SerializeField] private TMP_InputField _maxAngleSpeed;
        [SerializeField] private TMP_InputField _maxHp;
        
        private SubmitMenu _submitMenu;

        private void Awake()
        {
            _applyButton.onClick.AddListener(Apply);
            _focusButton.onClick.AddListener(() =>
            {
                var camMotion = FindObjectOfType<Camera>().GetComponent<CameraMotion>();
                camMotion.Player = unitScript.gameObject;
                camMotion.gameObject.transform.position = unitScript.gameObject.transform.position + Vector3.up * 90;;
            });
            _despawnButton.onClick.AddListener(Despawn);
        }

        public void Init(UnitScript unitScript, SubmitMenu submitMenu)
        {
            this.unitScript = unitScript;
            _submitMenu = submitMenu;

            _unitName.text = this.unitScript.networkUnitConfig.prefabName;
            
            _maxHp.text = this.unitScript.networkUnitConfig.maxHp.ToString(CultureInfo.InvariantCulture);
            _maxHp.onValueChanged.AddListener((arg0 => _hp.maxValue = float.Parse(arg0)));
            
            _hp.maxValue = this.unitScript.networkUnitConfig.maxHp;
            _hp.value = this.unitScript.networkUnitConfig.currentHp;
            this.unitScript.networkUnitConfig.currentHp.OnValueChanged +=
                (value, newValue) => _hp.value = newValue;
        }

        private void Apply()
        {
            unitScript.networkUnitConfig.maxHp = float.Parse(_maxHp.text);
            unitScript.networkUnitConfig.currentHp = _hp.value;
            unitScript.networkUnitConfig.maxSpeed = float.Parse(_maxSpeed.text);
            unitScript.networkUnitConfig.maxAngleSpeed = float.Parse(_maxAngleSpeed.text);
        }
        
        private void Despawn()
        {
            _submitMenu.RaiseSubmit(() =>
            {
                var spawner = FindObjectOfType<Spawner>();
                spawner.selectedPrefab = unitScript.gameObject;
                spawner.Despawn();
                Destroy(gameObject);
            });
        }
    }
}
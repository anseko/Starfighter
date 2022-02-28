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

        public void Init(UnitScript unitScript)
        {
            this.unitScript = unitScript;

            _unitName.text = this.unitScript.NetworkUnitConfig.PrefabName;
            
            _maxHp.text = this.unitScript.NetworkUnitConfig.MaxHp.ToString(CultureInfo.InvariantCulture);
            _maxHp.onValueChanged.AddListener((arg0 => _hp.maxValue = float.Parse(arg0)));
            
            _hp.maxValue = this.unitScript.NetworkUnitConfig.MaxHp;
            _hp.value = this.unitScript.NetworkUnitConfig.CurrentHp;
            this.unitScript.NetworkUnitConfig._currentHp.OnValueChanged +=
                (value, newValue) => _hp.value = newValue;
        }

        private void Apply()
        {
            unitScript.NetworkUnitConfig.MaxHp = float.Parse(_maxHp.text);
            unitScript.NetworkUnitConfig.CurrentHp = _hp.value;
            unitScript.NetworkUnitConfig.MaxSpeed = float.Parse(_maxSpeed.text);
            unitScript.NetworkUnitConfig.MaxAngleSpeed = float.Parse(_maxAngleSpeed.text);
        }
        
        private void Despawn()
        {
            var spawner = FindObjectOfType<Spawner>();
            spawner.selectedPrefab = unitScript.gameObject;
            spawner.Despawn();
            Destroy(gameObject);
        }
    }
}
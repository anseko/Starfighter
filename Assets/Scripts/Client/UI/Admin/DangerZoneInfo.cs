using System.Globalization;
using System.Linq;
using Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI.Admin
{
    public class DangerZoneInfo: MonoBehaviour
    {
        public DangerZone DangerZone { get; private set; }
        [SerializeField] private Button _applyButton;
        [SerializeField] private Button _focusButton;
        [SerializeField] private Button _despawnButton;
        [SerializeField] private TMP_InputField _radius;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _hpDamage;
        [SerializeField] private TMP_Text _stressDamage;

        private SubmitMenu _submitMenu;

        private void Awake()
        {
            _applyButton.onClick.AddListener(Apply);
            _focusButton.onClick.AddListener(() =>
            {
                var camMotion = FindObjectOfType<Camera>().GetComponent<CameraMotion>();
                camMotion.Player = DangerZone.gameObject;
                camMotion.gameObject.transform.position = DangerZone.gameObject.transform.position + Vector3.up * 90;;
            });
            _despawnButton.onClick.AddListener(Despawn);
        }

        public void Init(DangerZone dangerZone, SubmitMenu submitMenu)
        {
            DangerZone = dangerZone;
            _submitMenu = submitMenu;

            _hpDamage.text += DangerZone.zoneHpDamage.Value.ToString(CultureInfo.InvariantCulture);
            _stressDamage.text += DangerZone.zoneStressDamage.Value.ToString(CultureInfo.InvariantCulture);
            _name.text = DangerZone.zoneType.ToString().Split('.').Last();
            _radius.text = DangerZone.zoneRadius.Value.ToString(CultureInfo.InvariantCulture);

            GetComponent<Image>().color = DangerZone.zoneColor.Value;

            DangerZone.zoneRadius.OnValueChanged += (value, newValue) =>
            {
                _radius.text = newValue.ToString(CultureInfo.InvariantCulture);
            };
        }

        private void Apply()
        {
            DangerZone.zoneRadius.Value = float.Parse(_radius.text);
        }
        
        private void Despawn()
        {
            _submitMenu.RaiseSubmit(() =>
            {
                var spawner = FindObjectOfType<Spawner>();
                spawner.selectedPrefab = DangerZone.gameObject;
                spawner.Despawn();
                Destroy(gameObject);
            });
        }
    }
}
using System.Globalization;
using System.Linq;
using Client.Core;
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

            _hpDamage.text += DangerZone.zoneHpDamage.ToString(CultureInfo.InvariantCulture);
            _stressDamage.text += DangerZone.zoneStressDamage.ToString(CultureInfo.InvariantCulture);
            _name.text = DangerZone.zoneType.ToString().Split('.').Last();
            _radius.text = DangerZone.zoneRadius.ToString(CultureInfo.InvariantCulture);

            GetComponent<Image>().color = DangerZone.zoneColor;

            ClientEventStorage.GetInstance().OnDangerZoneRadiusChange.AddListener((float newValue) =>
            {
                _radius.text = newValue.ToString(CultureInfo.InvariantCulture);
            });
        }

        private void Apply()
        {
            DangerZone.zoneRadius = float.Parse(_radius.text);
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
using Core;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI.Admin
{
    public class PrefabInfo: MonoBehaviour
    {
        private Spawner _spawner;
        private SpaceUnitConfig _prefab;
        [SerializeField] private Button _spawnButton;
        
        [SerializeField] private Text _name;
        [SerializeField] private Image _type;

        [SerializeField] private TMP_InputField _login;
        [SerializeField] private TMP_InputField _password;
        [SerializeField] private TMP_InputField _shipId;
        
        [SerializeField] private Sprite _shipImg;
        [SerializeField] private Sprite _unitImg;
        [SerializeField] private Sprite _zoneImg;

        private void Awake()
        {
            _spawnButton.onClick.AddListener(Spawn);
            _spawner = FindObjectOfType<Spawner>();
        }

        public void Init(DangerZoneConfig zoneConfig)
        {
            _name.text = zoneConfig.type.ToString();
            _type.sprite = _zoneImg;
            // _zonePrefab = zoneConfig;
            _login.gameObject.SetActive(false);
            _password.gameObject.SetActive(false);
            if (_shipId.placeholder.TryGetComponent<TMP_Text>(out var placeholder))
            {
                placeholder.text = "radius";
            }
        }
        
        public void Init(SpaceShipConfig shipConfig)
        {
            _type.sprite = _shipImg;
            _prefab = shipConfig;
            _name.text = shipConfig.shipId;
        }
        
        public void Init(SpaceUnitConfig config)
        {
            _type.sprite = _unitImg;
            _name.text = config.prefabName;
            _prefab = config;
            _login.gameObject.SetActive(false);
            _password.gameObject.SetActive(false);
            _shipId.gameObject.SetActive(false);
        }

        private void Spawn()
        {
            if (_prefab is null)
            {
                _spawner.selectedPrefab = Resources.Load<GameObject>(Constants.PathToShipsPrefabs + "DangerZone");
                _spawner.Spawn(Constants.PathToDangerZonesObjects, _name.text, "");
                return;
            }
            
            if (_prefab is SpaceShipConfig config)
            {
                _spawner.selectedPrefab = Resources.Load<GameObject>(Constants.PathToShipsPrefabs + config.prefabName);
                var shipId = string.IsNullOrEmpty(_shipId.text) ? config.shipId : _shipId.text;
                _spawner.AddAccountServerRpc(_login.text, _password.text, shipId, config.prefabName);
                _spawner.Spawn(Constants.PathToShipsObjects, config.prefabName, shipId);
            }
            else
            {
                _spawner.selectedPrefab = Resources.Load<GameObject>(Constants.PathToPrefabs + _prefab.prefabName);
                _spawner.Spawn(Constants.PathToUnitsObjects, _prefab.prefabName, "");
            }
        }
    }
}
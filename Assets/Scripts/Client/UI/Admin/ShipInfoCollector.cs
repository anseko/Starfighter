using System.Collections.Generic;
using System.Linq;
using Client.Core;
using UnityEngine;

namespace Client.UI.Admin
{
    public class ShipInfoCollector: MonoBehaviour
    {
        public List<ShipInfo> ShipInfos = new List<ShipInfo>();
        [SerializeField] private ShipInfo _shipInfoPrefab;
        [SerializeField] private GameObject _view;
        [SerializeField] private SubmitMenu _submitMenu;

        public void Init()
        {
            var ships = FindObjectsOfType<PlayerScript>();
            foreach (var playerScript in ships)
            {
                var instance = Instantiate(_shipInfoPrefab, _view.transform);
                instance.Init(playerScript, _submitMenu);
                ShipInfos.Add(instance);
            }
        }

        public void Add(PlayerScript playerScript)
        {
            var instance = Instantiate(_shipInfoPrefab, _view.transform);
            instance.Init(playerScript, _submitMenu);
            ShipInfos.Add(instance);
        }

        public void Remove(PlayerScript playerScript)
        {
            var toRemove = ShipInfos.FirstOrDefault(x =>
                x.playerScript.networkUnitConfig.shipId == playerScript.networkUnitConfig.shipId);
            ShipInfos.Remove(toRemove);
            Destroy(toRemove);
        }
    }
}
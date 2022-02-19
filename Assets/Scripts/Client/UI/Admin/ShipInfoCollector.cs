using System.Collections.Generic;
using Client.Core;
using UnityEngine;

namespace Client.UI.Admin
{
    public class ShipInfoCollector: MonoBehaviour
    {
        public List<ShipInfo> ShipInfos = new List<ShipInfo>();
        [SerializeField] private ShipInfo _shipInfoPrefab;
        [SerializeField] private GameObject _view;

        public void Init()
        {
            var ships = FindObjectsOfType<PlayerScript>();
            foreach (var playerScript in ships)
            {
                var instance = Instantiate(_shipInfoPrefab, _view.transform);
                instance.Init(playerScript);
                ShipInfos.Add(instance);
            }
        }
    }
}
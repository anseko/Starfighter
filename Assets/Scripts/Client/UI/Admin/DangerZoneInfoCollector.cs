using System.Collections.Generic;
using System.Linq;
using Net;
using UnityEngine;

namespace Client.UI.Admin
{
    public class DangerZoneInfoCollector: MonoBehaviour
    {
        public List<DangerZoneInfo> ZoneInfos = new List<DangerZoneInfo>();
        [SerializeField] private DangerZoneInfo _zoneInfoPrefab;
        [SerializeField] private GameObject _view;
        [SerializeField] private SubmitMenu _submitMenu;

        public void Init()
        {
            var zones = FindObjectsOfType<DangerZone>();
            foreach (var zone in zones)
            {
                var instance = Instantiate(_zoneInfoPrefab, _view.transform);
                instance.Init(zone, _submitMenu);
                ZoneInfos.Add(instance);
            }
        }
        
        public void Add(DangerZone dangerZone)
        {
            var instance = Instantiate(_zoneInfoPrefab, _view.transform);
            instance.Init(dangerZone, _submitMenu);
            ZoneInfos.Add(instance);
        }
        
        public void Remove(DangerZone dangerZone)
        {
            var toRemoveInfo = ZoneInfos.FirstOrDefault(x => x.DangerZone == dangerZone);
            ZoneInfos.Remove(toRemoveInfo);
            Destroy(dangerZone.gameObject);
        }
    }
}
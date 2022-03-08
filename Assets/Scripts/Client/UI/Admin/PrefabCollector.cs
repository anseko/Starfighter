using System.Collections.Generic;
using System.Linq;
using Core;
using ScriptableObjects;
using UnityEngine;

namespace Client.UI.Admin
{
    public class PrefabCollector: MonoBehaviour
    {
        public List<PrefabInfo> AllInfos = new List<PrefabInfo>();
        [SerializeField] private PrefabInfo _prefabInfoPrefab;
        [SerializeField] private GameObject _view;
        
        public void Init()
        {
            var ships = Resources.LoadAll<SpaceShipConfig>(Constants.PathToShipsObjects);
            var units = Resources.LoadAll<SpaceUnitConfig>(Constants.PathToUnitsObjects);
            var zones = Resources.LoadAll<DangerZoneConfig>(Constants.PathToDangerZonesObjects).Distinct(new ZoneComparer());
            
            foreach (var info in ships)
            {
                var instance = Instantiate(_prefabInfoPrefab, _view.transform);
                instance.Init(info);
                AllInfos.Add(instance);
            }
            
            foreach (var info in units)
            {
                var instance = Instantiate(_prefabInfoPrefab, _view.transform);
                instance.Init(info);
                AllInfos.Add(instance);
            }

            foreach (var info in zones)
            {
                var instance = Instantiate(_prefabInfoPrefab, _view.transform);
                instance.Init(info);
                AllInfos.Add(instance);
            }
        }
        
        private class ZoneComparer : IEqualityComparer<DangerZoneConfig> {
            public bool Equals(DangerZoneConfig zone1, DangerZoneConfig zone2)
            {
                return zone1.type == zone2.type;
            }

            public int GetHashCode(DangerZoneConfig obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
using System.Collections.Generic;
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
        }
    }
}
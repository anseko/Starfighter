using System.Collections.Generic;
using Core;
using Unity.Burst.CompilerServices;
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
            var ships = Resources.LoadAll<GameObject>(Constants.PathToShipsPrefabs);
            var units = Resources.LoadAll<GameObject>(Constants.PathToPrefabs);
            
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
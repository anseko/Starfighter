using System.Collections.Generic;
using System.Linq;
using Client.Core;
using MLAPI;
using UnityEngine;

namespace Client.UI.Admin
{
    public class UnitInfoCollector: NetworkBehaviour
    {
        public List<UnitInfo> UnitInfos = new List<UnitInfo>();
        [SerializeField] private UnitInfo _unitInfoPrefab;
        [SerializeField] private GameObject _view;

        public void Init()
        {
            var ships = FindObjectsOfType<UnitScript>().Where(x => !(x is PlayerScript));
            foreach (var playerScript in ships)
            {
                var instance = Instantiate(_unitInfoPrefab, _view.transform);
                instance.Init(playerScript);
                UnitInfos.Add(instance);
            }
        }
        
        public void Add(UnitScript playerScript)
        {
            var instance = Instantiate(_unitInfoPrefab, _view.transform);
            instance.Init(playerScript);
            UnitInfos.Add(instance);
        }
        
        public void Remove(UnitScript playerScript)
        {
            var toRemoveInfo = UnitInfos.FirstOrDefault(x => x.unitScript == playerScript);
            UnitInfos.Remove(toRemoveInfo);
            Destroy(playerScript.gameObject);
        }
    }
}
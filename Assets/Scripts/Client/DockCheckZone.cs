using System;
using System.Collections.Generic;
using System.Linq;
using Client.Core;
using Net.Components;
using UnityEngine;

namespace Client
{
    public class DockCheckZone: MonoBehaviour
    {
        private List<GameObject> _objectsInDockZone;
        [SerializeField] 
        private PlayerScript _playerScript;

        
        private void Awake()
        {
            Debug.unityLogger.Log("Dock zone awake");
            _objectsInDockZone = new List<GameObject>();
        }

        public bool IsAnyInZone() => _objectsInDockZone.Any();
        
        private void OnTriggerEnter(Collider other)
        {
            Debug.unityLogger.Log("Dock zone triggers some");
            //This check is enough while physics collisions between Docking layer object only accounts
            if (other.gameObject.GetComponentInParent<UnitScript>() != _playerScript  &&
                !_playerScript.GetComponent<DockComponent>().readyToDock.Value)
            {
                Debug.unityLogger.Log($"{gameObject.transform.root.name} DockZone add object {other.gameObject.transform.root.name}");
                ClientEventStorage.GetInstance().DockableUnitsInRange.Invoke();
                _objectsInDockZone.Add(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            _objectsInDockZone.Remove(other.gameObject);
            if (_objectsInDockZone.Count == 0)
            {
                ClientEventStorage.GetInstance().NoOneToDock.Invoke();
            }
        }
    }
}
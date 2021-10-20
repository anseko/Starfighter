using Client.Core;
using Core;
using Net.Components;
using UnityEngine;

namespace Client.UI
{
    public class DockingTrigger: MonoBehaviour
    {
        private DockingState _state;
        private DockComponent _dockComponent;
        public bool dockAvailable;

        
        public void Init(DockComponent dockComponent)
        {
            _dockComponent = dockComponent;
            _state = FindObjectOfType<DockingState>();
            Debug.unityLogger.Log($"{dockComponent.gameObject.name}: Dock trigger INIT {_state}");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag(Constants.DockTag) || !gameObject.CompareTag(Constants.DockTag)) return;
            var otherUnit = other.gameObject.GetComponentInParent<DockComponent>();
            if (otherUnit is null) return;
            if (_state?.GetState() ?? true)
            {
                _dockComponent.lastThingToDock = otherUnit.GetComponent<UnitScript>();
                dockAvailable = true;
                if (_dockComponent.GetState() != UnitState.IsDocked)
                {
                    Debug.unityLogger.Log("DockTrigger found something");
                    ClientEventStorage.GetInstance().DockingAvailable.Invoke();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            dockAvailable = false;
            ClientEventStorage.GetInstance().DockableUnitsInRange.Invoke();
        }
        
        
    }
}

using Client.Core;
using UnityEngine;

namespace Client.UI
{
    public class DockingState : BasePlayerUIElement
    {
        private bool _dockStateUI = true;
        private Camera _camera;
        
        public override void Init(PlayerScript playerScript)
        {
            base.Init(playerScript);
            _camera = FindObjectOfType<Camera>();
            
            _camera.cullingMask |= (1 << 10);
        }

        public bool GetState()
        {
            return _dockStateUI;
        }

        public void Start()
        {
            _camera = FindObjectOfType<Camera>();
            _camera.cullingMask |= (1 << 10);
        }
        
        public void SwitchState()
        {
            _dockStateUI = !_dockStateUI;
            
            if (_dockStateUI)
            {
                _camera.cullingMask |= (1 << 10);
            }
            else
            {
                _camera.cullingMask &= ~(1 << 10);
            }
        }
    }
}

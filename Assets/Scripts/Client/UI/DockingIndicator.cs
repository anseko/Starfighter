using Client.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI
{
    public class DockingIndicator: MonoBehaviour
    {
        private readonly Color _space = new Color(24 / 255f, 184 / 255f, 1, 1);
        private Image _img;
        
        
        private void Awake()
        {
            ClientEventStorage.GetInstance().DockingAvailable.AddListener(IntoGreen);
            ClientEventStorage.GetInstance().DockableUnitsInRange.AddListener(IntoYellow);
            ClientEventStorage.GetInstance().IsDocked.AddListener(IntoBlue);
            ClientEventStorage.GetInstance().NoOneToDock.AddListener(IntoClear);
            _img = GetComponent<Image>();
        }

        private void OnEnable()
        {
            ClientEventStorage.GetInstance().DockIndicatorStateRequest.Invoke();
        }
        
        private void IntoBlue()
        {
            _img.color = _space;
        }
        
        private void IntoYellow()
        {
            if (_img.color == _space) return;
            
            _img.color = Color.yellow;
        }
        
        private void IntoGreen()
        {
            _img.color = Color.green;
        }
        
        private void IntoClear()
        {
            if(_img.color == Color.green || _img.color == _space) return;
            
            _img.color = Color.clear;
        }
        
        
    }
}
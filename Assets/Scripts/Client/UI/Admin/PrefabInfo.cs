using Client.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI.Admin
{
    public class PrefabInfo: MonoBehaviour
    {
        private UnitScript _unitScript;
        [SerializeField] private Text _name;
        [SerializeField] private Image _type;
        [SerializeField] private Sprite _shipImg;
        [SerializeField] private Sprite _unitImg;
        [SerializeField] private Sprite _asteroidImg;
        
        public void Init(GameObject gameObject)
        {
            if (gameObject.TryGetComponent<PlayerScript>(out var ps))
            {
                _unitScript = ps;
                _name.text = gameObject.name;
                _type.sprite = _shipImg;
                return;
            }
            
            if (gameObject.TryGetComponent<UnitScript>(out _unitScript))
            {
                _name.text = gameObject.name;
                _type.sprite = _unitImg;
                return;
            }

            _name.text = gameObject.name;
            _type.sprite = _asteroidImg;
        }
    }
}
using Client.Core;
using UnityEngine;

namespace Client.UI
{
    public class RotationWheelScript : BasePlayerUIElement
    {
        [SerializeField] 
        private int _rotationModifier;


        public override void Init(PlayerScript ps)
        {
            base.Init(ps);
            _rotationModifier = 1;
            transform.rotation = new Quaternion(0, 0, PlayerScript.transform.rotation.y, 0);
            // transform.Rotate(0, 0, PlayerScript.transform.rotation.y);
        }
        
        private void Update()
        {
            transform.Rotate(0,0, - PlayerScript.shipRotation.normalized.y * PlayerScript.shipRotation.magnitude * Mathf.Rad2Deg / _rotationModifier * Time.deltaTime);
        }
    }
}

using UnityEngine;

namespace Client.UI
{
    public class RotationWheelScript : BasePlayerUIElement
    {
        [SerializeField] 
        private int _rotationModifier;
    
    
        void Start()
        {
            _rotationModifier = 1;
        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(0,0, - PlayerScript.shipRotation.normalized.y * PlayerScript.shipRotation.magnitude * Mathf.Rad2Deg / _rotationModifier * Time.deltaTime);
        }
    }
}

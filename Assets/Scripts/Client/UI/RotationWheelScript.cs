using UnityEngine;

namespace Client.UI
{
    public class RotationWheelScript : BasePlayerUIElement
    {
        [SerializeField] 
        private int _rotationModifier;


        private void Start()
        {
            _rotationModifier = 1;
        }

        // Update is called once per frame
        private void Update()
        {
            transform.Rotate(0,0, - PlayerScript.shipRotation.Value.normalized.y * PlayerScript.shipRotation.Value.magnitude * Mathf.Rad2Deg / _rotationModifier * Time.deltaTime);
        }
    }
}

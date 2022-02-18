using UnityEngine;
using UnityEngine.UI;

public class ActiveFieldSwitcher : MonoBehaviour
{
    [SerializeField] private InputField _nextField;
    
    
    private void Update()
    {
        if (gameObject.GetComponent<InputField>().isFocused && Input.GetKeyDown(KeyCode.Tab))
        {
            _nextField.ActivateInputField();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
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

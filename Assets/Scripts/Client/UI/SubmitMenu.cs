using System;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI
{
    public class SubmitMenu : MonoBehaviour
    {
        [SerializeField] private Button _submitButton;
        [SerializeField] private Button _cancelButton;
        private Action _action = null;

        private void Awake()
        {
            gameObject.SetActive(false);
            _submitButton.onClick.AddListener(Submit);
            _cancelButton.onClick.AddListener(Cancel);
        }

        private void Submit()
        {
            gameObject.SetActive(false);
            _action();
            _action = null;
        }

        private void Cancel()
        {
            gameObject.SetActive(false);
            _action = null;
        }

        public void RaiseSubmit(Action action)
        {
            _action = action;
            gameObject.SetActive(true);
        }
    }
}

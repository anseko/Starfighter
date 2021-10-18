using Client;
using Client.Core;
using Core.InputManager;
using MLAPI;
using Net.PackageHandlers.ClientHandlers;
using ScriptableObjects;
using UnityEngine;

namespace Net
{
    
    [RequireComponent(typeof(ClientInitManager))]
    [RequireComponent(typeof(InputManager))]
    public class MainClientLoop : MonoBehaviour
    {
        public ClientAccountObject accountObject;
        private PlayerScript _playerScript = null;
        
        private new void Awake()
        {
            // CoreEventStorage.GetInstance().axisValueChanged.AddListener(SendMove);
            // CoreEventStorage.GetInstance().actionKeyPressed.AddListener(SendAction);
            // ClientEventStorage.GetInstance().SetPointEvent.AddListener(SetPoint);
            // QualitySettings.vSyncCount = 0;
            // Application.targetFrameRate = 120;
        }

        public void Disconnect()
        {
            Debug.unityLogger.Log("Disconnection");
            NetworkManager.Singleton.StopClient();
        }

        private void OnDestroy()
        {
            ClientHandlerManager.instance.Dispose();
            InputManager.instance.Dispose();
        }

        private void OnApplicationQuit()
        {
            Disconnect();
            OnDestroy();
        }
    }
}
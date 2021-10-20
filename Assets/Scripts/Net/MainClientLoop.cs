using Client.Core;
using Core.InputManager;
using MLAPI;
using UnityEngine;

namespace Net
{
    [RequireComponent(typeof(ClientInitManager))]
    [RequireComponent(typeof(InputManager))]
    public class MainClientLoop : MonoBehaviour
    {
        private new void Awake()
        {
            // QualitySettings.vSyncCount = 0;
            // Application.targetFrameRate = 120;
        }

        private void Disconnect()
        {
            Debug.unityLogger.Log("Disconnection");
            NetworkManager.Singleton.StopClient();
        }

        private void OnDestroy()
        {
            InputManager.instance.Dispose();
        }

        private void OnApplicationQuit()
        {
            Disconnect();
            OnDestroy();
        }
    }
}
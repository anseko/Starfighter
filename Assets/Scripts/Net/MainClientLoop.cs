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

        private void OnApplicationQuit()
        {
            NetworkManager.Singleton.StopClient();
            GetComponent<InputManager>().Dispose();
        }
    }
}
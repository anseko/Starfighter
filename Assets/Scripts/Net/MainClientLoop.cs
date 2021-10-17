using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Client;
using Client.Core;
using Core;
using Core.ClassExtensions;
using Core.InputManager;
using MLAPI;
using MLAPI.Messaging;
using Net.Core;
using Net.PackageData;
using Net.PackageData.EventsData;
using Net.PackageHandlers.ClientHandlers;
using Net.Packages;
using Net.Utils;
using ScriptableObjects;
using UnityEngine;
using Utils;
using EventType = Net.Utils.EventType;

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

        private void Start()
        {
            // AttachPlayerControl(GetComponent<PlayerScript>());  //BUG: так будет браться только самый первый попавшийся PlayerScript
        }
        

        private bool AttachPlayerControl(PlayerScript playerScript)
        {
            if (_playerScript is null && playerScript.gameObject.name.Split(Constants.Separator)[1] == accountObject.ship.shipId)
            {
                _playerScript = playerScript;
                switch (accountObject.type)
                {
                    case UserType.Admin:
                        ClientEventStorage.GetInstance().InitAdmin.Invoke(_playerScript);
                        break;
                    case UserType.Pilot:
                        ClientEventStorage.GetInstance().InitPilot.Invoke(_playerScript);
                        break;
                    case UserType.Navigator:
                        ClientEventStorage.GetInstance().InitNavigator.Invoke(_playerScript);
                        break;
                    case UserType.Spectator:
                        ClientEventStorage.GetInstance().InitSpectator.Invoke(_playerScript);
                        break;
                    case UserType.Moderator:
                        ClientEventStorage.GetInstance().InitModerator.Invoke(_playerScript);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return true;
            }
            return false;
        }
        
        private void SendAction(KeyCode code) => SendAction();
        
        private void SendMove(string axis, float value) =>SendMovement();

        private async void SendMovement()
        {
            try
            {
                // Debug.unityLogger.Log("Gonna send moves");
                var movementData = new MovementEventData()
                {
                    rotationValue = _playerScript.ShipsBrain.GetShipAngle(),
                    sideManeurValue = _playerScript.ShipsBrain.GetSideManeurSpeed(),
                    straightManeurValue = _playerScript.ShipsBrain.GetStraightManeurSpeed(),
                    thrustValue = _playerScript.ShipsBrain.GetThrustSpeed()
                };
                _playerScript.ShipsBrain.UpdateMovementActionData(movementData);
            }
            catch (Exception ex)
            {
                Debug.unityLogger.LogException(ex);
            }
        }

        private async void SendAction()
        {
            try
            {
                //TODO: Using RPC calls
                if (_playerScript.ShipsBrain.GetDockAction())
                {
                }

                if (_playerScript.ShipsBrain.GetFireAction())
                {
                }

                if (_playerScript.ShipsBrain.GetGrappleAction())
                {
                }
            }
            catch (Exception ex)
            {
                Debug.unityLogger.LogException(ex);
            }
        }

        private async void SetPoint(EventData data)
        {
        }

        public Coroutine LaunchCoroutine(IEnumerator coroutine) => StartCoroutine(coroutine);
        
        
        private void Update()
        {

        }
        
        private void FixedUpdate()
        {
            Dispatcher.Instance.InvokePending();
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
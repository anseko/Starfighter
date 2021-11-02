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
    [RequireComponent(typeof(ClientHandlerManager))]
    [RequireComponent(typeof(ClientInitManager))]
    [RequireComponent(typeof(InputManager))]
    public class MainClientLoop : Singleton<MainClientLoop>
    {
        public ClientAccountObject accountObject;
        public string serverAddress;
        [NonSerialized]
        public Stack<StatePackage> StatePackages;

        //Прием accept\decline пакетов, отправка данных и команд. Личный канал с сервером.
        private StarfighterUdpClient _udpClient;
        //Прием State пакетов от сервера. Общий канал
        private StarfighterUdpClient _multicastUdpClient;

        private PlayerScript _playerScript = null;
        
        
        private new void Awake()
        {
            base.Awake();
            NetEventStorage.GetInstance().connectToServer.AddListener(ConnectToServer);
            CoreEventStorage.GetInstance().axisValueChanged.AddListener(SendMove);
            CoreEventStorage.GetInstance().actionKeyPressed.AddListener(SendAction);
            ClientEventStorage.GetInstance().SetPointEvent.AddListener(SetPoint);
            // QualitySettings.vSyncCount = 0;
            // Application.targetFrameRate = 120;
            StatePackages = new Stack<StatePackage>();
        }

        public void Init(string serverAddress)
        {
            this.serverAddress = serverAddress;
        }

        private void SendAction(KeyCode code) => SendAction(_udpClient);
        
        private void SendMove(string axis, float value) =>SendMovement(_udpClient);

        private async void SendMovement(StarfighterUdpClient udpClient)
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
                var result = await udpClient.SendEventPackage(movementData, EventType.MoveEvent);
            }
            catch (Exception ex)
            {
                Debug.unityLogger.LogException(ex);
            }
        }

        private async void SendAction(StarfighterUdpClient udpClient)
        {
            try
            {
                if (_playerScript.ShipsBrain.GetDockAction())
                {
                    var result = await udpClient.SendEventPackage(_playerScript.gameObject.name, EventType.DockEvent);
                }

                if (_playerScript.ShipsBrain.GetFireAction())
                {
                    var result = await udpClient.SendEventPackage(_playerScript.gameObject.name, EventType.FireEvent);
                }

                if (_playerScript.ShipsBrain.GetGrappleAction())
                {
                    //TODO: Добавить нужной информации в пакет
                    var result = await udpClient.SendEventPackage(_playerScript.gameObject.name, EventType.GrappleEvent);
                }
            }
            catch (Exception ex)
            {
                Debug.unityLogger.LogException(ex);
            }
        }
        
        private async void SetPoint(EventData data) => await _udpClient.SendEventPackage(data.data, data.eventType);
        
        public Coroutine LaunchCoroutine(IEnumerator coroutine) => StartCoroutine(coroutine);
        
        public bool TryAttachPlayerControl(PlayerScript playerScript)
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
        
        private void Update()
        {
            if(StatePackages.Count == 0) return;
            
            var statePack = StatePackages.Pop();
            
            Debug.unityLogger.Log($"Skip {StatePackages.Count} state packages");
            
            StatePackages.Clear();
            
            foreach (var worldObject in statePack.data.worldState)
            {
                //TODO: перепроверить, могу ли я использовать поиск по имени, вместо сбора всех объектов по тегу и поиска по тегу. Вопрос касается WayPoint
                if (worldObject.toDestroy)
                {
                    var go = GameObject.Find(worldObject.name);
                    if (go != null)
                    {
                        Destroy(go);
                    }
                    continue;
                }
                
                if (worldObject is Asteroid)
                {
                    var go = InstantiateHelper.InstantiateObject(worldObject);
                    Debug.unityLogger.Log($"asteroid are added");
                    go.tag = Constants.AsteroidTag;
                    continue;
                }
                
                if (worldObject is WayPoint)
                {
                    var wayPointGo = GameObject.FindGameObjectsWithTag(Constants.WayPointTag)
                        .FirstOrDefault(go => go.name == worldObject.name);
                        
                    if (wayPointGo != null)
                    {
                        //Сервер однозначно определяет положение ВСЕХ объектов
                        wayPointGo.transform.position = worldObject.position;
                        wayPointGo.transform.rotation = worldObject.rotation;
                    }
                    else
                    {
                        var go = InstantiateHelper.InstantiateObject(worldObject);
                        go.tag = Constants.WayPointTag;
                        var pointer = Resources.FindObjectsOfTypeAll<GPSView>().First();
                        pointer.SetTarget(go);
                        pointer.gameObject.SetActive(true);
                    }
                
                    continue;
                }
                
                if (worldObject is SpaceShip)
                {
                    var spaceShipGo = GameObject.FindGameObjectsWithTag(Constants.DynamicTag)
                        .FirstOrDefault(go => go.name == worldObject.name);
                
                    if (spaceShipGo != null)
                    {
                        //Сервер однозначно определяет положение ВСЕХ объектов
                        spaceShipGo.transform.position = worldObject.position;
                        spaceShipGo.transform.rotation = worldObject.rotation;
                        spaceShipGo.GetComponent<PlayerScript>().shipRotation =
                            (worldObject as SpaceShip).angularVelocity;
                        spaceShipGo.GetComponent<PlayerScript>().shipSpeed =
                            (worldObject as SpaceShip).velocity;
                    }
                    else
                    {
                        var go = InstantiateHelper.InstantiateObject(worldObject);
                        var ps = go.GetComponent<PlayerScript>();
                        if (ps is null) continue;
                        if (!MainClientLoop.instance.TryAttachPlayerControl(ps))
                        {
                            ps.movementAdapter = MovementAdapter.RemoteNetworkControl;
                        }
                    }
                    continue;
                }
                
                //default: WorldObject
                {
                    var worldObjectGo = GameObject.FindGameObjectsWithTag(Constants.DynamicTag)
                        .FirstOrDefault(go => go.name == worldObject.name);
                
                    if (worldObjectGo != null)
                    {
                        //Сервер однозначно определяет положение ВСЕХ объектов
                        worldObjectGo.transform.position = worldObject.position;
                        worldObjectGo.transform.rotation = worldObject.rotation;
                    }
                    else
                    {
                        var go = InstantiateHelper.InstantiateObject(worldObject);
                    }
                }
            }
        }
        
        private void FixedUpdate()
        {
            Dispatcher.Instance.InvokePending();
        }
        
        private void StartListenServer()
        {
            _multicastUdpClient.BeginReceivingPackage();
            _udpClient.BeginReceivingPackage();
        }
        
        private void ConnectToServer(ConnectPackage result)
        {
            //надо иметь два udp клиента. Для прослушки multicast и для прослушки личного порта от сервера.
            try
            {
                accountObject = Resources.LoadAll<ClientAccountObject>(Constants.PathToAccounts)
                    .First(x=>x.login == result.data.login && x.password == result.data.password);
                
                _udpClient = new StarfighterUdpClient(IPAddress.Parse(serverAddress),
                    result.data.portToSend,
                    result.data.portToReceive);

                var multicastAddress = IPAddress.Parse(result.data.multicastGroupIp);
                _multicastUdpClient = new StarfighterUdpClient(multicastAddress,
                    Constants.ServerReceivingPort, Constants.ServerSendingPort);
                _multicastUdpClient.JoinMulticastGroup(multicastAddress);
                        
                StartListenServer();
            }
            catch (Exception ex)
            {
                Debug.unityLogger.LogException(ex);
            }
        }

        public async void Disconnect()
        {
            Debug.unityLogger.Log("Disconnection");
            await _udpClient.SendPackageAsync(new DisconnectPackage(new DisconnectData()
            {
                accountType = accountObject.type,
                login = accountObject.login,
                password = accountObject.password,
            }));
            
            _udpClient.Dispose();
            _multicastUdpClient.Dispose();
        }

        private void OnDestroy()
        {
            ClientHandlerManager.instance.Dispose();
            InputManager.instance.Dispose();
        }

        private void OnApplicationQuit()
        {
            Disconnect();
            _udpClient.Dispose();
        }
    }
}
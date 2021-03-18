﻿using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Client;
using Net.Core;
using Net.PackageData;
using Net.PackageData.EventsData;
using Net.PackageHandlers.ClientHandlers;
using Net.PackageHandlers.ServerHandlers;
using Net.Packages;
using Net.Utils;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using EventType = Net.Utils.EventType;

namespace Net
{
    [RequireComponent(typeof(ClientHandlerManager))]
    public class MainClientLoop : MonoBehaviour
    {
        public AccountType accType;
        public string login;
        public string password;
        public string serverAddress;

        //Прием accept\decline пакетов, отправка данных и команд. Личный канал с сервером.
        private StarfighterUdpClient _udpClient;
        //Прием State пакетов от сервера. Общий канал
        private StarfighterUdpClient _multicastUdpClient;
        
        private void Awake()
        {
            //It's Client, so exchange ports
            _udpClient = new StarfighterUdpClient(IPAddress.Parse(serverAddress),
                Constants.ServerReceivingPort,
                Constants.ServerSendingPort);
        }

        private void Start()
        {
            // var sphere = Resources.Load<GameObject>(Constants.PathToPrefabs + "Sphere");
            // sphere.name += "_" + Guid.NewGuid();
            // sphere.transform.position = Vector3.zero;
            // sphere.tag = Constants.PlayerTag;
            // Instantiate(sphere);
            Task.Run(ConnectToServer);
        }

        private void Update()
        {
            if (Input.anyKey)
            {
                NetEventStorage.GetInstance().sendMoves.Invoke(_udpClient);
            }
            
            // NetEventStorage.GetInstance().updateWorldState.Invoke(GetWorldStatePackage().Result);
        }
        
        private void FixedUpdate()
        {
            Dispatcher.Instance.InvokePending();
        }
        
        // private async Task<StatePackage> GetWorldStatePackage()
        // {
        //     Debug.unityLogger.Log("MainClientLoop.GetWorldStatePackage");
        //     //var gameObjects = GameObject.FindGameObjectsWithTag(Constants.PlayerTag);
        //     var worldData = new StateData()
        //     {
        //         //worldState = gameObjects.Select(go => new WorldObject(go.name, go.transform)).ToArray()
        //     };
        //     return new StatePackage(worldData);
        // }
        
        private void StartListenServer()
        {
            _multicastUdpClient.BeginReceivingPackage();
            _udpClient.BeginReceivingPackage();
        }
        
        // private async void SendWorldState(StatePackage worldState)
        // {
        //     await _udpClient.SendPackageAsync(worldState);
        // }

        private async void ConnectToServer()
        {
            var connectData = new ConnectData()
            {
                login = login, password = password, accountType = accType
            };

            await _udpClient.SendPackageAsync(new ConnectPackage(connectData));
            Debug.unityLogger.Log($"connection package sent");
            var result = await _udpClient.ReceiveOnePackageAsync();
            _udpClient.Dispose();
            Debug.unityLogger.Log($"response package received: {result.packageType}");
            switch (result.packageType)
            {
                case PackageType.ConnectPackage:
                {
                    //надо иметь два udp клиента. Для прослушки multicast и для прослушки личного порта от сервера.
                    Debug.unityLogger.Log("Server accept our connection");
                    try
                    {
                        _udpClient = new StarfighterUdpClient(IPAddress.Parse(serverAddress),
                            (result as ConnectPackage).data.portToSend,
                            (result as ConnectPackage).data.portToReceive);

                        var multicastAddress = IPAddress.Parse((result as ConnectPackage).data.multicastGroupIp);
                        _multicastUdpClient = new StarfighterUdpClient(multicastAddress,
                            Constants.ServerReceivingPort, Constants.ServerSendingPort);
                        _multicastUdpClient.JoinMulticastGroup(multicastAddress);

                        // NetEventStorage.GetInstance().updateWorldState.AddListener(SendWorldState);

                        StartListenServer();
                    }
                    catch (Exception ex)
                    {
                        Debug.unityLogger.LogError("Connect to Server error:", ex.Message);
                    }
                    
                    break;
                }
                case PackageType.DeclinePackage:
                    Debug.unityLogger.Log("Server decline our connection");
                    //TODO: Return to login screen
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"unexpected package type {result.packageType.ToString()}");
                    break;
            }
        }

        private void OnDestroy()
        {
            ClientHandlerManager.instance.Dispose();
        }

        private void OnApplicationQuit()
        {
            _udpClient.SendPackageAsync(new DisconnectPackage(new DisconnectData()
            {
                accountType = accType,
                login = login,
                password = password,
            }));
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Net.Core;
using Net.Interfaces;
using Net.PackageData;
using Net.PackageHandlers.ServerHandlers;
using Net.Packages;
using Net.Utils;
using UnityEngine;

namespace Net
{
    public class MainServerLoop : MonoBehaviour
    {

        // private ServerManager _servManager;

        private EventBus _eventBus;
        private HandlerManager _handlerManager;
        private UdpSocket _udpSocket;

        private List<IPAddress> _connectedClients;
        
        private void Awake()
        {
            _connectedClients = new List<IPAddress>();
            _eventBus = EventBus.GetInstance();
            // _servManager = ServerManager.GetInstance();
            _handlerManager = HandlerManager.GetInstance();
            //Config init
            
            //Events binding
            _eventBus.sendDecline.AddListener(ServerResponse.SendDecline);
            _eventBus.sendAccept.AddListener(ServerResponse.SendAccept);
            _eventBus.addClient.AddListener(AddNewClient);
            _eventBus.updateWorldState.AddListener(SendWorldState);
        }

        // Use this for initialization
        private void Start()
        {
            _udpSocket = new UdpSocket( IPAddress.Parse(Constants.MulticastAddress), Constants.ServerSendingPort,
                    IPAddress.Any, Constants.ServerReceivingPort);
            Debug.Log($"waiting connection from anyone: {_udpSocket.GetReceivingAddress()}:{Constants.ServerReceivingPort}");
            _udpSocket.BeginReceivingPackagesAsync();
        }

        private void Update()
        {
            try
            {
                // _servManager.ConnectedClients.ForEach(client => client.Update());
                // Debug.unityLogger.Log($"Clients count: {_servManager.ConnectedClients.Count}");
                //Send WorldState to every client
                _eventBus.updateWorldState.Invoke(GetWorldStatePackage().Result);
            }
            catch (Exception ex)
            {
                Debug.unityLogger.LogException(ex);
            }
        }
        
        private void FixedUpdate()
        {
            Dispatcher.Instance.InvokePending();
        }

        private async Task<StatePackage> GetWorldStatePackage()
        {
            Debug.unityLogger.Log("MainServerLoop.GetWorldStatePackage");
            var gameObjects = GameObject.FindGameObjectsWithTag(Constants.DynamicTag);
            var worldData = new StateData()
            {
                worldState = gameObjects.Select(go => new WorldObject(go.name,go.transform)).ToArray()
            };
            return new StatePackage(worldData);
        }

        private void AddNewClient(IPAddress address)
        {
            _udpSocket.AddClient(address);
            _connectedClients.Add(address);
        }

        private async void SendWorldState(StatePackage pack)
        {
            await _udpSocket.SendPackageAsync(pack);
        }
        
        private void OnDestroy()
        {
            // _servManager.Dispose();
            _eventBus.Dispose();
        }
    }

}

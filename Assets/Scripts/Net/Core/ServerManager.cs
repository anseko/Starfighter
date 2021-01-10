﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Client;
using Net.Interfaces;
using Net.PackageData;
using Net.Packages;
using UnityEngine;

/// <summary>
/// ClientManager занимается управлением Client'ов.
/// Создает, удаляет, хранит список. 
/// Также отвечает за инициацию бродкаст отправки.
/// </summary>
namespace Net.Core
{
    public class ClientManager : IDisposable
    {
        private static ClientManager _instance = new ClientManager();
        private int _lastGivenPort = 8000;
        
        public List<Client> ConnectedClients;

        private ClientManager()
        {
            ConnectedClients = new List<Client>();
        }

        public static ClientManager GetInstance()
        {
            return _instance;
        }

        public void AddClient(ConnectPackage info)
        {
            ConnectedClients.Add(new Client(
                info.ipAddress, info.data.portToReceive, info.data.portToSend));
        }
        
        public int GetNewPort()
        {
            return ++_lastGivenPort;
        }

        public void Dispose()
        {
            ConnectedClients.ForEach(client => client.Dispose());
        }
        
    }
}
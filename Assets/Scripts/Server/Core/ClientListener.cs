﻿using UnityEngine;
using System.Net;
using Server.Interfaces;
using Server.Packages;
using System.Threading.Tasks;

namespace Server.Core
{
    /// <summary>
    /// ClientListener слушает определенный адрес и порт (клиента). Принимает от него пакеты и отправляет пакеты ему.
    /// Подписан на событие updateWorldState - отправляет состояние мира своему клиенту
    /// При получении пакета от клиента - передает его на обработку вызовом события newPackageRecieved
    /// </summary>
    public class ClientListener : ScriptableObject
    {
        [SerializeField]
        private UdpSocket _udpSocket;

        [SerializeField]
        private Task _listening;

        public ClientListener(IPEndPoint endpoint, IPackage pack)
        {
            _udpSocket = new UdpSocket(endpoint);
            _listening = new Task(() => ListenClient());

            EventBus.Instance.updateWorldState.AddListener(SendWorldState);
        }

        public IPEndPoint GetEndPoint()
        {
            return _udpSocket.GetEndPoint();
        }

        private async void SendWorldState(StatePackage worldState)
        {
            await _udpSocket.SendPackageAsync(worldState);
        }

        private async void ListenClient()
        {
            var package = await _udpSocket.RecievePackageAsync();
            EventBus.Instance.newPackageRecieved.Invoke(package);
        }

        void FixedUpdate()
        {
            Debug.Log("ClientListener fixedUpdate");
            if (_listening == null) return;

            if(_listening.IsCompleted)
            {
                _listening.Start();
            }
        }
    }
}

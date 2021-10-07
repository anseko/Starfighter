using System;
using System.Threading.Tasks;
using Core;
using Net.Core;
using Net.PackageData;
using Net.PackageHandlers.ClientHandlers;
using Net.Packages;
using UnityEngine;

namespace Net.Utils
{
    public static class SenderHelper
    {
        public static async Task<bool> SendEventPackage(this StarfighterUdpClient client, object data, EventType type)
        {
            // Debug.unityLogger.Log($"Gonna send event to Server {type}");
            var eventData = new EventData()
            {
                data = data,
                eventType = type,
                eventId = Guid.NewGuid(),
                timeStamp = DateTime.Now
            };
            var pack = new EventPackage(eventData);

            switch (type)
            {
                case EventType.GrappleEvent:
                case EventType.DockEvent:
                    ClientHandlerManager.instance.AddToPendingList(pack);
                    break;
                case EventType.MoveEvent:
                case EventType.FireEvent:
                case EventType.HitEvent:
                case EventType.InitEvent:
                case EventType.WayPointEvent:
                case EventType.OtherEvent:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return await client.SendPackageAsync(pack);
        }
        
        public static async void SendConnectionResponse(AbstractPackage pack)
        {
            try
            {
                var socket = new StarfighterUdpClient(pack.ipAddress, Constants.ServerSendingPort, 0);
                var result = await socket.SendPackageAsync(pack);
            }
            catch (Exception ex)
            {
                Debug.unityLogger.LogException(ex);
            }

            Debug.unityLogger.Log($"Connection response sent to {pack.ipAddress}:{Constants.ServerSendingPort}");
        }
    }
}
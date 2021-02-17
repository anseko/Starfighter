﻿using System;
using Net.Core;
using Net.Packages;
using UnityEngine;

namespace Net.Utils
{
    public static class ServerHelper
    {
        public static async void SendConnectionResponse(AbstractPackage pack)
        {
            Debug.unityLogger.Log($"Sending Connection Response to {pack.ipAddress}:{Constants.ServerSendingPort}");
            try
            {
                var socket = new StarfighterUdpClient(pack.ipAddress, Constants.ServerSendingPort, 0);
                var result = await socket.SendPackageAsync(pack);
            }
            catch (Exception ex)
            {
                Debug.unityLogger.LogError("SendConnectionResponse error:", ex.Message);
            }

            Debug.unityLogger.Log($"Connection response sent to {pack.ipAddress}:{Constants.ServerSendingPort}");
        }
    }
}
﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core;
using Net.Core;
using Net.PackageData;
using Net.PackageHandlers.ServerHandlers;
using Net.Packages;
using Net.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Net
{
    [RequireComponent(typeof(ClientManager))]
    [RequireComponent(typeof(HandlerManager))]
    public class MainServerLoop : Singleton<MainServerLoop>
    {
        private StarfighterUdpClient _multicastUdpClient;
        
        
        private new void Awake()
        {
            base.Awake();
            ConfigInit();
            NetEventStorage.GetInstance().updateWorldState.AddListener(SendWorldState);
        }

        private void ConfigInit()
        {
            
        }

        private void ConfigSave()
        {
            
        }
        
        private void Start()
        {
            _multicastUdpClient = new StarfighterUdpClient(IPAddress.Parse(Constants.MulticastAddress),
                Constants.ServerSendingPort, Constants.ServerReceivingPort);
            Debug.Log($"start waiting connection packs from anyone: {Constants.ServerReceivingPort}");
            _multicastUdpClient.BeginReceivingPackage();
        }

        private void Update()
        {
            try
            {
                GetWorldStatePackage();
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

        private void GetWorldStatePackage()
        {
            // Debug.unityLogger.Log("MainServerLoop.GetWorldStatePackage");

            var collection = CollectWorldObjects();
            var worldStateCoroutine = StartCoroutine(collection);
        }
        
        private IEnumerator CollectWorldObjects()
        {
            var worldObjects = new List<WorldObject>();
            var allGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var go in allGameObjects.Where(obj => obj.CompareTag(Constants.DynamicTag)))
            {
                worldObjects.Add(new WorldObject(go.name, go.transform));
                yield return null;
            }

            var statePackage = new StatePackage(new StateData()
            {
                worldState = worldObjects.ToArray()
            });
            
            NetEventStorage.GetInstance().updateWorldState.Invoke(statePackage);
        }
        
        private async void SendWorldState(StatePackage pack)
        {
            await _multicastUdpClient.SendPackageAsync(pack);
        }
        
        private void OnDestroy()
        {
            ClientManager.instance.Dispose();
            HandlerManager.instance.Dispose();
            ConfigSave();
        }

        private void OnApplicationQuit()
        {
            ClientManager.instance.Dispose();
            HandlerManager.instance.Dispose();
            _multicastUdpClient.Dispose();
        }
    }

}

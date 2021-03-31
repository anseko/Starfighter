﻿using System;
using Core;
using Net.Core;
using Net.PackageHandlers.ClientHandlers;
using UnityEngine;

namespace Client.Core
{
    public class ClientInitManager: Singleton<ClientInitManager>
    {
        protected new void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            ClientEventStorage.GetInstance().InitNavigator.AddListener(InitNavigator);
            ClientEventStorage.GetInstance().InitPilot.AddListener(InitPilot);
        }

        public void InitPilot(PlayerScript ps)
        {
            //TODO: normal pilot init
            ps.movementAdapter = MovementAdapter.PlayerControl;
            var followComp = Camera.main.gameObject.GetComponent<Follow>();
            Camera.main.orthographicSize = 25;
            followComp.Player = ps.gameObject;
            followComp.enabled = true;
        }

        public void InitNavigator(PlayerScript ps)
        {
            //TODO: normal nav init
            ps.movementAdapter = MovementAdapter.BlankControl;
            var followComp = Camera.main.gameObject.GetComponent<Follow>();
            Camera.main.orthographicSize = 50;
            followComp.Player = ps.gameObject;
            followComp.enabled = true;
        }
    }
}
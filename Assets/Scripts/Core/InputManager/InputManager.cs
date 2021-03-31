﻿using System;
using System.Linq;
using UnityEngine.Serialization;

namespace Core.InputManager
{
    public class InputManager: Singleton<InputManager>, IDisposable
    {
        public SmartAxis[] axes;

        private void Start()
        {
            
        }

        private void Update()
        {
            foreach (var axis in axes)
            {
                axis.Update();
            }
        }

        public void Dispose()
        {
            
        }
    }
}
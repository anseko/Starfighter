﻿using System;
using UnityEngine.Serialization;

namespace Net.PackageData.EventsData
{
    [Serializable]
    public class MovementEventData
    {
        public float thrustValue;
        public float rotationValue;
        public float sideManeurValue;
        public float straightManeurValue;
    }
}
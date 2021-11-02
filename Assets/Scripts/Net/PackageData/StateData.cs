using System;
using Core;
using Core.Models;
using UnityEngine;

namespace Net.PackageData
{
    [Serializable]
    public class StateData
    {
        [SerializeField]
        public WorldObject[] worldState;
    }

    [Serializable]
    public class WorldObject
    {
        public WorldObject(string name, Transform transform, bool toDestroy = false)
        {
            this.name = name;
            position = transform.position;
            rotation = transform.rotation;
            this.toDestroy = toDestroy;
        }
        
        [SerializeField]
        public string name;
        [SerializeField]
        public Vector3 position;
        [SerializeField]
        public Quaternion rotation;
        [SerializeField]
        public bool toDestroy;
    }

    [Serializable]
    public class Asteroid : WorldObject
    {
        public Asteroid(string name, Transform transform, bool toDestroy = false) : base(name, transform, toDestroy)
        {}
    }
    
    [Serializable]
    public class WayPoint : WorldObject
    {
        public WayPoint(string name, Transform transform, bool toDestroy = false) : base(name, transform, toDestroy)
        {}
    }

    [Serializable]
    public class SpaceShip : WorldObject
    {
        [SerializeField]
        public Vector3 velocity;
        [SerializeField]
        public Vector3 angularVelocity;
        [SerializeField]
        public SpaceShipDto dto;
        [SerializeField]
        public UnitState shipState;

        public SpaceShip(string name, Transform transform, Vector3 velocity, Vector3 angularVelocity, SpaceShipDto config, UnitState shipState = UnitState.InFlight, bool toDestroy = false) : base(name,
            transform, toDestroy)
        {
            this.velocity = velocity;
            this.angularVelocity = angularVelocity;
            dto = config;
            this.shipState = shipState;
        }
    }

    
}

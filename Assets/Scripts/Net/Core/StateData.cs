using System;
using UnityEngine;

namespace Net.Core
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
}

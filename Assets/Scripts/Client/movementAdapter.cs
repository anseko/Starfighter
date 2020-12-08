using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Control
{
    public abstract class MovementAdapter
    {
        public abstract EngineState getMovement();
    }

    public struct EngineState
    {
        public bool topLeft;
        public bool topRight;
        public bool botLeft;
        public bool botRight;
        public bool Thrust;   
    }
}
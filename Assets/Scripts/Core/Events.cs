using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    public class IntEvent : UnityEvent<int> { }
    public class FloatEvent : UnityEvent<float> { }
    public class UnitStateEvent : UnityEvent<UnitState, UnitState> { }
    public class Vector3Event : UnityEvent<Vector3> { }
    public class CoreEvent: UnityEvent { }
    public class AxisValueEvent: UnityEvent<string, float> { }
    public class KeyCodeEvent: UnityEvent<KeyCode> { }
}
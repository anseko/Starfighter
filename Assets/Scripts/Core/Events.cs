using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    public class IntEvent : UnityEvent<int> { }
    public class CoreEvent: UnityEvent { }
    public class AxisValueEvent: UnityEvent<string, float> { }
    public class KeyCodeEvent: UnityEvent<KeyCode> { }
}
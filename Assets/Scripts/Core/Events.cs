using System.Net;
using Client;
using Net.Core;
using Net.PackageData;
using Net.PackageData.EventsData;
using Net.Packages;
using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    #region NetEvents
    
    public class PackageEvent : UnityEvent<AbstractPackage> { }
    public class StatePackageEvent : UnityEvent<StatePackage> { }

    public class PlayerMovementEvent: UnityEvent<IPAddress, MovementData> { }
    public class WayPointEvent : UnityEvent<IPAddress, WayPoint> { }
    
    #endregion
    
    public class IntEvent : UnityEvent<int> { }
    public class CoreEvent: UnityEvent { }
    public class AxisValueEvent: UnityEvent<string, float> { }
    public class KeyCodeEvent: UnityEvent<KeyCode> { }
    public class PlayerScriptEvent: UnityEvent<PlayerScript> { }
    public class UnitStateEvent: UnityEvent<UnitState> { }
}
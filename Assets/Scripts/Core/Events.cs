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
    public class ConnectPackageEvent : UnityEvent<ConnectPackage> { }
    public class DisconnectPackageEvent : UnityEvent<DisconnectPackage> { }
    public class EventPackageEvent : UnityEvent<EventPackage> { }
    public class StatePackageEvent : UnityEvent<StatePackage> { }
    public class AcceptPackageEvent : UnityEvent<AcceptPackage> { }
    public class DeclinePackageEvent : UnityEvent<DeclinePackage> { }
    
    public class StateDataEvent : UnityEvent<StateData> { }
    public class EventDataEvent : UnityEvent<EventData> { }
    public class ConnectDataEvent : UnityEvent<ConnectData> { }
    
    public class IpAddressEvent : UnityEvent<IPAddress> { }
    public class ClientEvent: UnityEvent<Net.Core.Client> { }
    public class PlayerMovementEvent: UnityEvent<IPAddress, MovementData> { }
    public class WayPointEvent : UnityEvent<IPAddress, WayPoint> { }
    public class StarfighterUdpClientEvent : UnityEvent<StarfighterUdpClient> { }
    
    #endregion
    
    public class IntEvent : UnityEvent<int> { }
    public class CoreEvent: UnityEvent { }
    public class AxisValueEvent: UnityEvent<string, float> { }
    public class KeyCodeEvent: UnityEvent<KeyCode> { }
    public class PlayerScriptEvent: UnityEvent<PlayerScript> { }
    public class UnitStateEvent: UnityEvent<UnitState> { }
}
using System;
using System.Threading.Tasks;
using Net.Core;
using Net.Interfaces;
using Net.PackageData;
using Net.PackageData.EventsData;
using Net.Packages;
using UnityEngine;
using EventType = Net.Utils.EventType;

namespace Net.PackageHandlers.ServerHandlers
{
    public class EventPackageHandler : IPackageHandler
    {
        public async Task Handle(AbstractPackage pack)
        {
            try
            {
                var eventPack = pack as EventPackage;
                Debug.unityLogger.Log($"Got the event: {eventPack.data.eventType.ToString()}");
                switch (eventPack.data.eventType)
                {
                    case EventType.MoveEvent:
                        var movement = (MovementData) eventPack.data.data;
                        NetEventStorage.GetInstance().serverMovedPlayer.Invoke(pack.ipAddress, movement);
                        break;
                    case EventType.DockEvent:
                        NetEventStorage.GetInstance().dockEvent.Invoke(pack);
                        break;
                    case EventType.FireEvent:
                        var fireData = eventPack.data.data.ToString();
                        break;
                    case EventType.OtherEvent:
                        var data = eventPack.data.data.ToString();
                        break;
                    case EventType.HitEvent:
                        var hitData = eventPack.data.data.ToString();
                        break;
                    case EventType.InitEvent:
                        var initData = eventPack.data.data.ToString();
                        break;
                    case EventType.WayPointEvent:
                        var wayPoint = (WayPoint) eventPack.data.data;
                        NetEventStorage.GetInstance().wayPointSetted.Invoke(pack.ipAddress, wayPoint);
                        break;
                    case EventType.GrappleEvent:
                        var grapple = eventPack.data.data;
                        //TODO: Запустить grappler с заданым Guid. 
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                Debug.unityLogger.LogException(ex);
            }
        }
    }
}
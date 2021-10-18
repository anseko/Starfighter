using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Client;
using Core;
using Net.PackageData;
using Net.PackageData.EventsData;
using Net.Packages;
using Net.Utils;
using ScriptableObjects;
using UnityEngine;
using Utils;
using EventType = Net.Utils.EventType;

namespace Net.Core
{
    /// <summary>
    /// Client слушает определенный адрес и порт (клиента). Принимает от него пакеты и отправляет пакеты ему.
    /// </summary>
    public class Client
    {
        //
        // private void TryToDock(AbstractPackage package)
        // {
        //     try
        //     {
        //         if (!Equals(GetIpAddress(), package.ipAddress)) return;
        //         Debug.unityLogger.Log($"TryToDock called: {_playerScript.GetState()}");
        //         Dispatcher.Instance.Invoke(async () =>
        //         {
        //             switch (_playerScript.GetState())
        //             {
        //                 case UnitState.InFlight:
        //                 {
        //                     if (!_playerScript.readyToDock)
        //                     {
        //                         await SendDecline(new DeclineData() {eventId = (package as EventPackage).data.eventId});
        //                         return;
        //                     }
        //
        //                     var clientToDock = ClientManager.instance.ConnectedClients.FirstOrDefault(x =>
        //                         x._playerScript.gameObject == _playerScript.lastThingToDock.gameObject);
        //                     if (clientToDock != null)
        //                     {
        //                         await clientToDock.SendEvent(new EventData()
        //                             {data = clientToDock._myGameObjectName, eventType = EventType.DockEvent});
        //                     }
        //
        //                     if (_playerScript.lastThingToDock is PlayerScript script)
        //                     {
        //                         script.unitStateMachine.ChangeState(UnitState.IsDocked);
        //                     }
        //
        //                     _playerScript.unitStateMachine.ChangeState(UnitState.IsDocked);
        //                     await SendAccept(new AcceptData() {eventId = (package as EventPackage).data.eventId});
        //
        //                     break;
        //                 }
        //                 case UnitState.IsDocked:
        //                 {
        //                     //It's always possible to undock
        //                     var clientToUnDock = ClientManager.instance.ConnectedClients.FirstOrDefault(x =>
        //                         x._playerScript.gameObject == _playerScript.lastThingToDock.gameObject);
        //                     if (clientToUnDock != null)
        //                     {
        //                         await clientToUnDock.SendEvent(new EventData()
        //                             {data = clientToUnDock._myGameObjectName, eventType = EventType.DockEvent});
        //                     }
        //
        //                     if (_playerScript.lastThingToDock is PlayerScript script)
        //                     {
        //                         script.unitStateMachine.ChangeState(UnitState.InFlight);
        //                     }
        //
        //                     _playerScript.unitStateMachine.ChangeState(UnitState.InFlight);
        //                     await SendAccept(new AcceptData() {eventId = (package as EventPackage).data.eventId});
        //                     break;
        //                 }
        //                 case UnitState.IsDead:
        //                 {
        //                     //It's always impossible to dock while being dead
        //                     await SendDecline(new DeclineData() {eventId = (package as EventPackage).data.eventId});
        //                     break;
        //                 }
        //                 default:
        //                     throw new ArgumentOutOfRangeException();
        //             }
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         Debug.unityLogger.LogException(ex);
        //     }
        // }
        //
    }
}

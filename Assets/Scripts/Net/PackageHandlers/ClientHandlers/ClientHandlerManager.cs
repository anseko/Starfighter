using System;
using System.Collections.Generic;
using System.Linq;
using Client;
using Core;
using Net.Core;
using Net.Packages;
using Net.Utils;
using UnityEngine;
using Utils;
using EventType = Net.Utils.EventType;

namespace Net.PackageHandlers.ClientHandlers
{
    public class ClientHandlerManager: AbstractHandlerManager
    {
        private List<AbstractPackage> _responsePendingList;
        
        public ClientHandlerManager()
        {
            NetEventStorage.GetInstance().newPackageRecieved.AddListener(HandlePackage);
            NetEventStorage.GetInstance().acceptPackageRecieved.AddListener(AcceptEvent);
            NetEventStorage.GetInstance().declinePackageRecieved.AddListener(DeclineEvent);
            AcceptHandler = new AcceptPackageHandler();
            DeclineHandler = new DeclinePackageHandler();
            EventHandler = new EventPackageHandler();
            StateHandler = new StatePackageHandler();
            _responsePendingList = new List<AbstractPackage>();
        }

        public override async void HandlePackage(AbstractPackage pack)
        {
            // Debug.unityLogger.Log($"Client Gonna handle some packs! {pack.packageType}");
            switch (pack.packageType)
            {
                case PackageType.AcceptPackage:
                    await AcceptHandler.Handle(pack);
                    break;
                case PackageType.DeclinePackage:
                    await DeclineHandler.Handle(pack);
                    break;
                case PackageType.EventPackage:
                    await EventHandler.Handle(pack);
                    break;
                case PackageType.StatePackage:
                    await StateHandler.Handle(pack);
                    break;
                case PackageType.ConnectPackage:
                case PackageType.DisconnectPackage:
                    //Предполагается, что этих пакетов не будет прилетать на клиент.
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void AddToPendingList(AbstractPackage package)
        {
            _responsePendingList.Add(package);
        }
        
        public void AcceptEvent(AbstractPackage acceptPackage)
        {
            Debug.unityLogger.Log($"Accept handling {acceptPackage.id}");
            var id = (acceptPackage as AcceptPackage).data.eventId;
            var package = _responsePendingList.FirstOrDefault(x => (x as EventPackage).data.eventId == id) as EventPackage;
            switch (package.data.eventType)
            {
                case EventType.MoveEvent:
                    break;
                case EventType.DockEvent:
                {
                    Debug.unityLogger.Log("Dock event accepted");
                    
                    var name = package.data.data.ToString();
                    Dispatcher.Instance.Invoke(() =>
                    {
                        var go = GameObject.Find(name).GetComponent<PlayerScript>(); 

                        if (go.GetState() == UnitState.InFlight)
                        {
                            go.unitStateMachine.ChangeState(UnitState.IsDocked);
                            
                        }
                        else if (go.GetState() == UnitState.IsDocked)
                        {
                            go.unitStateMachine.ChangeState(UnitState.InFlight);
                        }
                    });
                    break;
                }
                case EventType.GrappleEvent:
                    break;
                case EventType.FireEvent:
                    break;
                case EventType.HitEvent:
                    break;
                case EventType.InitEvent:
                    break;
                case EventType.WayPointEvent:
                    break;
                case EventType.OtherEvent:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            _responsePendingList.Remove(package);
        }
        
        public void DeclineEvent(AbstractPackage declinePackage)
        {
            Debug.unityLogger.Log($"Decline handling {declinePackage.id}");
            var id = (declinePackage as DeclinePackage).data.eventId;
            var package = _responsePendingList.FirstOrDefault(x => (x as EventPackage).data.eventId == id) as EventPackage;
            
            switch (package.data.eventType)
            {
                case EventType.GrappleEvent:
                {
                    //TODO: remove GO of grapple.
                }
                    break;
                case EventType.MoveEvent:
                case EventType.DockEvent:
                case EventType.FireEvent:
                case EventType.HitEvent:
                case EventType.InitEvent:
                case EventType.WayPointEvent:
                case EventType.OtherEvent:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            _responsePendingList.Remove(package);
        }
    }
}
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
    public class Client: IDisposable
    {
        private StarfighterUdpClient _udpSocket;
        private PlayerScript _playerScript;
        private string _myGameObjectName;
        private string _login;
        private UserType _accountType;

        public Client(IPAddress address, int sendingPort,  int listeningPort, ClientAccountObject account)
        {
            try
            {
                NetEventStorage.GetInstance().serverMovedPlayer.AddListener(UpdateMovement);
                NetEventStorage.GetInstance().dockEvent.AddListener(TryToDock);

                _accountType = account.type;
                _login = account.login;

                _playerScript = InstantiateHelper.InstantiateServerShip(account.ship);

                _myGameObjectName = _playerScript.gameObject.name;

                _udpSocket = new StarfighterUdpClient(address, sendingPort, listeningPort);

                
                
                StartListenClient();
            }
            catch (Exception ex)
            {
                Debug.unityLogger.LogException(ex);
            }
        }

        public IPAddress GetIpAddress() => _udpSocket.GetSendingAddress();

        public int GetListeningPort() => _udpSocket.GetListeningPort();

        public string GetShipGOName() => _myGameObjectName;

        public string GetLogin() => _login; 

        private void StartListenClient()
        {
            _udpSocket.BeginReceivingPackage();
        }

        private void UpdateMovement(IPAddress address, MovementData data)
        {
            if (!Equals(GetIpAddress(), address) || _playerScript.GetState() != UnitState.InFlight) return;
            ClientManager.instance.SendToAll(new EventPackage(new EventData()
            {
                data = (_myGameObjectName, data),
                eventType = EventType.MoveEvent
            }), GetIpAddress());
        }

        private void TryToDock(AbstractPackage package)
        {
            try
            {
                if (!Equals(GetIpAddress(), package.ipAddress)) return;
                Debug.unityLogger.Log($"TryToDock called: {_playerScript.GetState()}");
                Dispatcher.Instance.Invoke(async () =>
                {
                    switch (_playerScript.GetState())
                    {
                        case UnitState.InFlight:
                        {
                            if (!_playerScript.readyToDock)
                            {
                                await SendDecline(new DeclineData() {eventId = (package as EventPackage).data.eventId});
                                return;
                            }

                            var clientToDock = ClientManager.instance.ConnectedClients.FirstOrDefault(x =>
                                x._playerScript.gameObject == _playerScript.lastThingToDock.gameObject);
                            if (clientToDock != null)
                            {
                                await clientToDock.SendEvent(new EventData()
                                    {data = clientToDock._myGameObjectName, eventType = EventType.DockEvent});
                            }

                            if (_playerScript.lastThingToDock is PlayerScript script)
                            {
                                script.unitStateMachine.ChangeState(UnitState.IsDocked);
                            }

                            _playerScript.unitStateMachine.ChangeState(UnitState.IsDocked);
                            await SendAccept(new AcceptData() {eventId = (package as EventPackage).data.eventId});

                            break;
                        }
                        case UnitState.IsDocked:
                        {
                            //It's always possible to undock
                            var clientToUnDock = ClientManager.instance.ConnectedClients.FirstOrDefault(x =>
                                x._playerScript.gameObject == _playerScript.lastThingToDock.gameObject);
                            if (clientToUnDock != null)
                            {
                                await clientToUnDock.SendEvent(new EventData()
                                    {data = clientToUnDock._myGameObjectName, eventType = EventType.DockEvent});
                            }

                            if (_playerScript.lastThingToDock is PlayerScript script)
                            {
                                script.unitStateMachine.ChangeState(UnitState.InFlight);
                            }

                            _playerScript.unitStateMachine.ChangeState(UnitState.InFlight);
                            await SendAccept(new AcceptData() {eventId = (package as EventPackage).data.eventId});
                            break;
                        }
                        case UnitState.IsDead:
                        {
                            //It's always impossible to dock while being dead
                            await SendDecline(new DeclineData() {eventId = (package as EventPackage).data.eventId});
                            break;
                        }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.unityLogger.LogException(ex);
            }
        }
        
        private async Task SendDecline(DeclineData data)
        {
            // Debug.unityLogger.Log($"Gonna send decline to: {_udpSocket.GetSendingAddress()}");
            
            var result = await _udpSocket.SendPackageAsync(new DeclinePackage(data));
        }

        private async Task SendAccept(AcceptData data)
        {
            // Debug.unityLogger.Log($"Gonna send accept to: {_udpSocket.GetSendingAddress()}:{Constants.ServerSendingPort}");
            var result = await _udpSocket.SendPackageAsync(new AcceptPackage(data));
        }

        private async Task SendWorldState(StateData data)
        {
            var result = await _udpSocket.SendPackageAsync(new StatePackage(data));
        }

        private async Task SendEvent(EventData data)
        {
            Debug.unityLogger.Log($"Gonna send event to: {_udpSocket.GetSendingAddress()}:{Constants.ServerSendingPort}");
            var result = await _udpSocket.SendEventPackage(data.data, data.eventType);
        }

        private async Task SendDisconnect(DisconnectData data)
        {
            var result = await _udpSocket.SendPackageAsync(new DisconnectPackage(data));
        }
        
        public async Task SendPackage(AbstractPackage pack)
        {
            switch (pack.packageType)
            {
                case PackageType.DisconnectPackage:
                    await SendDisconnect((pack as DisconnectPackage).data);
                    break;
                case PackageType.AcceptPackage:
                    await SendAccept((pack as AcceptPackage).data);
                    break;
                case PackageType.DeclinePackage:
                    await SendDecline((pack as DeclinePackage).data);
                    break;
                case PackageType.EventPackage:
                    await SendEvent((pack as EventPackage).data);
                    break;
                case PackageType.StatePackage:
                    await SendWorldState((pack as StatePackage).data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        

        private IEnumerator WorldInitCoroutine(List<GameObject> asteroids, int rangeSize = 10)
        {
            yield return new WaitForSeconds(0.5f);
            while (asteroids.Count > 0)
            {
                var range = asteroids.GetRange(0, Mathf.Min(rangeSize, asteroids.Count));
                asteroids.RemoveRange(0, Mathf.Min(rangeSize, asteroids.Count));
                Debug.unityLogger.Log($"sended {range.Count} asteroids, {asteroids.Count} remains. Coords {range[0].transform.position}");
                SendWorldState(new StateData()
                {
                    worldState = range.Select(x => new Asteroid(x.name, x.transform)).ToArray()
                });
                
                yield return new WaitForSeconds(0.5f);
            }

            yield return new WaitForSeconds(0.5f);
            NetEventStorage.GetInstance().worldInitDone.Invoke(this);
        }
        
        public void Dispose()
        {
            NetEventStorage.GetInstance().serverMovedPlayer.RemoveListener(UpdateMovement);
            NetEventStorage.GetInstance().dockEvent.RemoveListener(TryToDock);
            _udpSocket.Dispose();
        }
    }
}

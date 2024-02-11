using System;
using UnityEngine;
using Client.UI;
using Client.UI.Admin;
using Client.UI.Mechanic;
using Client.Utils;
using Core;
using Core.Models;
using Mirror;
using Net;
using Net.Components;

namespace Client.Core
{
    public class ClientInitManager: MonoBehaviour
    {
        [SerializeField] private Canvas _pilotUi;
        [SerializeField] private Canvas _navigatorUi;
        [SerializeField] private Canvas _adminUi;
        [SerializeField] private Canvas _spectatorUi;
        [SerializeField] private Canvas _mainMenuUi;
        [SerializeField] private Canvas _stationUi;
        [SerializeField] private Canvas _mechanicUi;
        
        public void InitPilot(PlayerScript ps)
        {
            _mainMenuUi.gameObject.SetActive(false);
            _pilotUi.gameObject.SetActive(true);
            var cam = FindObjectOfType<Camera>();
            var followComp = cam.gameObject.GetComponent<CameraMotion>() ?? cam.gameObject.AddComponent<CameraMotion>();
            cam.orthographicSize = 25;
            followComp.Player = ps.gameObject;
            followComp.enabled = true;
            ps.GetComponent<WayPointComponent>()?.Init(false);
            FindObjectOfType<DataOutput>()?.Init(ps);
            FindObjectOfType<RotationWheelScript>()?.Init(ps);
            FindObjectOfType<RotationPanelScript>()?.Init(ps);
            FindObjectOfType<SpeedPanelScript>()?.Init(ps);
            FindObjectOfType<CoordinatesUI>()?.Init(ps);
            FindObjectOfType<CourseView>()?.Init(ps);
            FindObjectOfType<DockingState>()?.Init(ps);
            FindObjectOfType<GPSView>(true)?.Init(ps);
            FindObjectOfType<DeathStateEffects>()?.Init(ps);
            FindObjectOfType<HpMarker>()?.Init(ps);
            Destroy(FindObjectOfType<GridFiller>().gameObject);
            //не отображать зоны опасности на пилоте
            foreach (var dangerZone in FindObjectsOfType<DangerZone>())
            {
                dangerZone.gameObject.SetActive(false);
            }

            NetworkClient.Ready();
        }
        
        public void InitNavigator(PlayerScript ps)
        {
            _mainMenuUi.gameObject.SetActive(false);
            _navigatorUi.gameObject.SetActive(true);
            ps.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            var cam = FindObjectOfType<Camera>();
            var followComp = cam.gameObject.GetComponent<CameraMotion>()??cam.gameObject.AddComponent<CameraMotion>();
            cam.orthographicSize = 50;
            followComp.Player = ps.gameObject;
            followComp.enabled = true;
            var zoomComp = cam.gameObject.GetComponent<Zoom>()??cam.gameObject.AddComponent<Zoom>();
            zoomComp.navigatorCamera = cam;
            zoomComp.enabled = true;
            ps.GetComponent<WayPointComponent>()?.Init(true);
            ps.GetComponent<OrderComponent>()?.Init();
            ps.GetComponent<FieldOfViewComponent>()?.Init(ps);
            cam.cullingMask &= ~(1 << 10); //Disable docking marks render
            FindObjectOfType<NavigatorCourseView>()?.Init(ps);
            FindObjectOfType<Stressbar>(true)?.Init(ps);
            FindObjectOfType<Hpbar>(true)?.Init(ps);

            RescaleGrid();
            
            NetworkClient.Ready();
        }
        
        public void InitSpectator()
        {
            _spectatorUi.gameObject.SetActive(true);
            _mainMenuUi.gameObject.SetActive(false);
            var cam = FindObjectOfType<Camera>();
            var followComp = cam.gameObject.GetComponent<CameraMotion>()??cam.gameObject.AddComponent<CameraMotion>();
            cam.orthographicSize = 50;
            followComp.enabled = true;
            var zoomComp = cam.gameObject.GetComponent<Zoom>()??cam.gameObject.AddComponent<Zoom>();
            zoomComp.navigatorCamera = cam;
            zoomComp.enabled = true;
            cam.cullingMask &= ~(1 << 10); //Disable docking marks render

            RescaleGrid();
            
            NetworkClient.Ready();
        }
        
        public void InitStation(PlayerScript ps)
        {
            _stationUi.gameObject.SetActive(true);
            _mainMenuUi.gameObject.SetActive(false);
            FindObjectOfType<OrdersScript>(true).gameObject.SetActive(true);
            var cam = FindObjectOfType<Camera>();
            var followComp = cam.gameObject.GetComponent<CameraMotion>()??cam.gameObject.AddComponent<CameraMotion>();
            cam.orthographicSize = 50;
            followComp.enabled = true;
            var zoomComp = cam.gameObject.GetComponent<Zoom>()??cam.gameObject.AddComponent<Zoom>();
            zoomComp.navigatorCamera = cam;
            zoomComp.enabled = true;
            cam.cullingMask &= ~(1 << 10); //Disable docking marks render
            FindObjectOfType<OrdersScript>().GetShipList();
            ps.GetComponent<FieldOfViewComponent>()?.Init(ps);

            RescaleGrid();
            
            NetworkClient.Ready();
        }

        public void InitAdmin()
        {
            _mainMenuUi.gameObject.SetActive(false);
            _adminUi.gameObject.SetActive(true);
            FindObjectOfType<ShipInfoCollector>()?.Init();
            FindObjectOfType<UnitInfoCollector>()?.Init();
            FindObjectOfType<PrefabCollector>()?.Init();
            FindObjectOfType<DangerZoneInfoCollector>()?.Init();
            FindObjectOfType<Spawner>()?.Init();
            
            var cam = FindObjectOfType<Camera>(false);
            var followComp = cam.gameObject.GetComponent<CameraMotion>()??cam.gameObject.AddComponent<CameraMotion>();
            cam.orthographicSize = 50;
            followComp.enabled = true;
            var zoomComp = cam.gameObject.GetComponent<Zoom>()??cam.gameObject.AddComponent<Zoom>();
            zoomComp.navigatorCamera = cam;
            zoomComp.enabled = true;

            RescaleGrid();
            
            NetworkClient.Ready();
        }
        
        public void InitMechanic()
        {
            _mechanicUi.gameObject.SetActive(true);
            _mainMenuUi.gameObject.SetActive(false);
            var cam = FindObjectOfType<Camera>();
            var followComp = cam.gameObject.GetComponent<CameraMotion>()??cam.gameObject.AddComponent<CameraMotion>();
            Destroy(FindObjectOfType<GridFiller>().gameObject);
            cam.orthographicSize = 100;
            followComp.enabled = false;
            var zoomComp = cam.gameObject.GetComponent<Zoom>()??cam.gameObject.AddComponent<Zoom>();
            zoomComp.enabled = false;
            FindObjectOfType<MechanicPlayerSelectorFill>()?.Init();
            
            NetworkClient.Ready();
        }

        private void RescaleGrid()
        {
            var spacefield = FindObjectOfType<SpaceFieldTypeDto>()?.Type;
            switch (spacefield)
            {
                case SpaceFieldType.SpaceField_Test:
                    break;
                case SpaceFieldType.SpaceField_1:
                case SpaceFieldType.SpaceField_2:
                    FindObjectOfType<GridFiller>().transform.root.localScale *= 3;
                    break;
                case SpaceFieldType.SpaceField_3:
                    FindObjectOfType<GridFiller>().transform.root.localScale *= 1.3f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
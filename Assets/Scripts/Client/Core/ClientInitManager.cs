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
            var cam = FindFirstObjectByType<Camera>();
            var followComp = cam.gameObject.GetComponent<CameraMotion>() ?? cam.gameObject.AddComponent<CameraMotion>();
            cam.orthographicSize = 25;
            followComp.Player = ps.gameObject;
            followComp.enabled = true;
            ps.GetComponent<WayPointComponent>()?.Init(false);
            FindFirstObjectByType<DataOutput>()?.Init(ps);
            FindFirstObjectByType<RotationWheelScript>()?.Init(ps);
            FindFirstObjectByType<RotationPanelScript>()?.Init(ps);
            FindFirstObjectByType<SpeedPanelScript>()?.Init(ps);
            FindFirstObjectByType<CoordinatesUI>()?.Init(ps);
            FindFirstObjectByType<CourseView>()?.Init(ps);
            FindFirstObjectByType<DockingState>()?.Init(ps);
            FindFirstObjectByType<GPSView>(FindObjectsInactive.Include)?.Init(ps);
            FindFirstObjectByType<DeathStateEffects>()?.Init(ps);
            FindFirstObjectByType<HpMarker>()?.Init(ps);
            Destroy(FindFirstObjectByType<GridFiller>().gameObject);
            //не отображать зоны опасности на пилоте
            foreach (var dangerZone in FindObjectsByType<DangerZone>(FindObjectsSortMode.None))
            {
                dangerZone.gameObject.SetActive(false);
            }


        }
        
        public void InitNavigator(PlayerScript ps)
        {
            _mainMenuUi.gameObject.SetActive(false);
            _navigatorUi.gameObject.SetActive(true);
            ps.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            var cam = FindFirstObjectByType<Camera>();
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
            FindFirstObjectByType<NavigatorCourseView>()?.Init(ps);
            FindFirstObjectByType<Stressbar>(FindObjectsInactive.Include)?.Init(ps);
            FindFirstObjectByType<Hpbar>(FindObjectsInactive.Include)?.Init(ps);

            RescaleGrid();
            

        }
        
        public void InitSpectator()
        {
            _spectatorUi.gameObject.SetActive(true);
            _mainMenuUi.gameObject.SetActive(false);
            var cam = FindFirstObjectByType<Camera>();
            var followComp = cam.gameObject.GetComponent<CameraMotion>()??cam.gameObject.AddComponent<CameraMotion>();
            cam.orthographicSize = 50;
            followComp.enabled = true;
            var zoomComp = cam.gameObject.GetComponent<Zoom>()??cam.gameObject.AddComponent<Zoom>();
            zoomComp.navigatorCamera = cam;
            zoomComp.enabled = true;
            cam.cullingMask &= ~(1 << 10); //Disable docking marks render

            RescaleGrid();
            

        }
        
        public void InitStation(PlayerScript ps)
        {
            _stationUi.gameObject.SetActive(true);
            _mainMenuUi.gameObject.SetActive(false);
            FindFirstObjectByType<OrdersScript>(FindObjectsInactive.Include).gameObject.SetActive(true);
            var cam = FindFirstObjectByType<Camera>();
            var followComp = cam.gameObject.GetComponent<CameraMotion>()??cam.gameObject.AddComponent<CameraMotion>();
            cam.orthographicSize = 50;
            followComp.enabled = true;
            var zoomComp = cam.gameObject.GetComponent<Zoom>()??cam.gameObject.AddComponent<Zoom>();
            zoomComp.navigatorCamera = cam;
            zoomComp.enabled = true;
            cam.cullingMask &= ~(1 << 10); //Disable docking marks render
            FindFirstObjectByType<OrdersScript>().GetShipList();
            ps.GetComponent<FieldOfViewComponent>()?.Init(ps);

            RescaleGrid();
            

        }

        public void InitAdmin()
        {
            _mainMenuUi.gameObject.SetActive(false);
            _adminUi.gameObject.SetActive(true);
            FindFirstObjectByType<ShipInfoCollector>()?.Init();
            FindFirstObjectByType<UnitInfoCollector>()?.Init();
            FindFirstObjectByType<PrefabCollector>()?.Init();
            FindFirstObjectByType<DangerZoneInfoCollector>()?.Init();
            FindFirstObjectByType<Spawner>()?.Init();
            
            var cam = FindFirstObjectByType<Camera>(FindObjectsInactive.Exclude);
            var followComp = cam.gameObject.GetComponent<CameraMotion>()??cam.gameObject.AddComponent<CameraMotion>();
            cam.orthographicSize = 50;
            followComp.enabled = true;
            var zoomComp = cam.gameObject.GetComponent<Zoom>()??cam.gameObject.AddComponent<Zoom>();
            zoomComp.navigatorCamera = cam;
            zoomComp.enabled = true;

            RescaleGrid();
            

        }
        
        public void InitMechanic()
        {
            _mechanicUi.gameObject.SetActive(true);
            _mainMenuUi.gameObject.SetActive(false);
            var cam = FindFirstObjectByType<Camera>();
            var followComp = cam.gameObject.GetComponent<CameraMotion>()??cam.gameObject.AddComponent<CameraMotion>();
            Destroy(FindFirstObjectByType<GridFiller>().gameObject);
            cam.orthographicSize = 100;
            followComp.enabled = false;
            var zoomComp = cam.gameObject.GetComponent<Zoom>()??cam.gameObject.AddComponent<Zoom>();
            zoomComp.enabled = false;
            FindFirstObjectByType<MechanicPlayerSelectorFill>()?.Init();
            

        }

        private void RescaleGrid()
        {
            var spacefield = FindFirstObjectByType<SpaceFieldTypeDto>()?.Type;
            switch (spacefield)
            {
                case SpaceFieldType.SpaceField_Test:
                    break;
                case SpaceFieldType.SpaceField_1:
                case SpaceFieldType.SpaceField_2:
                    FindFirstObjectByType<GridFiller>().transform.root.localScale *= 3;
                    break;
                case SpaceFieldType.SpaceField_3:
                    FindFirstObjectByType<GridFiller>().transform.root.localScale *= 1.3f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
﻿using UnityEngine;
using Client.UI;
using Net;
using Net.Components;
using UnityEngine.SceneManagement;

namespace Client.Core
{
    public class ClientInitManager: MonoBehaviour
    {
        [SerializeField] private Canvas _pilotUi;
        [SerializeField] private Canvas _navigatorUi;
        [SerializeField] private Canvas _adminUi;
        [SerializeField] private Canvas _spectatorUi;
        [SerializeField] private Canvas _moderatorUi;
        [SerializeField] private Canvas _mainMenuUi;
        
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

            //не отображать зоны опасности на пилоте
            foreach (var dangerZone in FindObjectsOfType<DangerZone>())
            {
                dangerZone.gameObject.SetActive(false);
            }
        }
        
        public void InitNavigator(PlayerScript ps)
        {
            _mainMenuUi.gameObject.SetActive(false);
            _navigatorUi.gameObject.SetActive(true);
            ps.gameObject.GetComponent<Collider>().enabled = false;
            ps.gameObject.GetComponent<Rigidbody>().detectCollisions = false;
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
            cam.cullingMask &= ~(1 << 10); //Disable docking marks render
            FindObjectOfType<NavigatorCourseView>()?.Init(ps);
            FindObjectOfType<Stressbar>(true)?.Init(ps);
        }
    }
}
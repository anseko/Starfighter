using System.Linq;
using Core;
using UnityEngine;
using Client.UI;

namespace Client.Core
{
    public class ClientInitManager: Singleton<ClientInitManager>
    {
        protected new void Awake()
        {
            base.Awake();
            
            ClientEventStorage.GetInstance().InitNavigator.AddListener(InitNavigator);
            ClientEventStorage.GetInstance().InitPilot.AddListener(InitPilot);
        }


        private static void InitPilot(PlayerScript ps)
        {
            ps.movementAdapter = MovementAdapter.PlayerControl;
            ps.gameObject.GetComponent<Collider>().enabled = false;
            ps.gameObject.GetComponent<Rigidbody>().detectCollisions = false;
            ps.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            var cam = FindObjectOfType<Camera>();
            var followComp = cam.gameObject.GetComponent<CameraMotion>() ?? cam.gameObject.AddComponent<CameraMotion>();
            cam.orthographicSize = 25;
            followComp.Player = ps.gameObject;
            followComp.enabled = true;
            FindObjectOfType<DataOutput>()?.Init(ps);
            FindObjectOfType<RotationWheelScript>()?.Init(ps);
            FindObjectOfType<RotationPanelScript>()?.Init(ps);
            FindObjectOfType<SpeedPanelScript>()?.Init(ps);
            FindObjectOfType<CoordinatesUI>()?.Init(ps);
            FindObjectOfType<RotationScript>()?.Init(ps);
            FindObjectOfType<CourseView>()?.Init(ps);
            // FindObjectOfType<DockingTrigger>()?.Init(ps);
            FindObjectOfType<DockingState>()?.Init(ps);
            Resources.FindObjectsOfTypeAll<GPSView>().First().Init(ps);
            FindObjectOfType<MenuButton>().PauseMenuUI = Resources.FindObjectsOfTypeAll<PauseMenu>().First().gameObject;
            
            // Init collision behavior
            ps.gameObject.GetComponent<CollisionHpReductionScript>().Init(ps);
        }

        private static void InitNavigator(PlayerScript ps)
        {
            ps.movementAdapter = MovementAdapter.RemoteNetworkControl;
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
            
            FindObjectOfType<CourseView>()?.Init(ps);
            FindObjectOfType<RotationScript>()?.Init(ps);
            FindObjectOfType<MenuButton>().PauseMenuUI = FindObjectOfType<PauseMenu>().gameObject;
        }
    }
}
﻿using System;
using Client.Core;
using Core;
using Net.PackageData;
using UnityEngine;
using Utils;
using EventType = Net.Utils.EventType;

namespace Client
{
    public class NaviPointController : MonoBehaviour
    {
        public GameObject mainPoint;
        private GameObject _point;
        private Vector3 _position;
        private Camera _camera;

        private void Start()
        {
            _camera = FindObjectOfType<Camera>();
        }
        
        private void OnGUI()
        {
            if (Event.current.button == 1 && Event.current.isMouse)
                SetPoint(_camera.ScreenToWorldPoint(Input.mousePosition));
        }

        void SetPoint(Vector3 clickCoords)
        {
            _position = new Vector3(clickCoords.x, 0, clickCoords.z);
            if (_point is null)
            {
                _point = Instantiate(mainPoint, _position, mainPoint.transform.rotation);
                _point.name = _point.name.Replace("(Clone)","") + Constants.Separator + Guid.NewGuid();
                _point.tag = Constants.WayPointTag;
            }

            _point.transform.position = _position;

            ClientEventStorage.GetInstance().SetPointEvent.Invoke(new EventData()
            {
                data = new WorldObject(_point.name, _point.transform),
                eventType = EventType.WayPointEvent
            });
        }
    
    }
}

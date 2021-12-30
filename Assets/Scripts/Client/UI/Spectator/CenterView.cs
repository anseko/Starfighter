using System.Collections;
using System.Collections.Generic;
using Client;
using Client.Core;
using TMPro;
using UnityEngine;

public class CenterView : MonoBehaviour

{
    private GameObject _ship;
    [SerializeField] private Camera _camera;
    
    private void Start()
    {
        _camera = transform.root.gameObject.GetComponentInChildren<Camera>();
        _ship = GameObject.Find(GetComponentInChildren<TextMeshProUGUI>().text);
    }
    
    public void Spectate()
    {
        var motion = _camera.GetComponent<CameraMotion>();
        motion.Player = _ship;
        if (!motion.GetFollowMode())
        {
            motion.SwitchFollowMode();
        }

        motion.enabled = true;
    }
}

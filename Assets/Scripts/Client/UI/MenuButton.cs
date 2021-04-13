﻿using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public GameObject PauseMenuUI;
    
    private void Start() 
    {
        PauseMenuUI.SetActive(false);    
    }

    public void Pause() 
    {
        PauseMenuUI.SetActive(true);
    }

}
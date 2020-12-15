using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject PauseMenuUI;

    public void Resume() 
    {
        PauseMenuUI.SetActive(false);
    }

    public void Quit() 
    {
        Application.Quit();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    public Toggle PilotToggle; 
    public Toggle NaviToggle;
    
    public void PlayGame()
    {
        if (PilotToggle.isOn)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
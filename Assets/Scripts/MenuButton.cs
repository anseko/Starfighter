using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
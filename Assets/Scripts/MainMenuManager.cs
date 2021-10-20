using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    public void GoToMain(){
        SceneManager.LoadScene("main");
    }
    public void GoToCC(){
        SceneManager.LoadScene("changecollection");
    }
    public void QuitGame(){
        Application.Quit();
    }
    

}

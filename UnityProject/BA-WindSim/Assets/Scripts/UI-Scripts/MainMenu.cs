using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public bool isTUI = false;
    

    public void GroupA(){
        int boolInt = isTUI ? 1 : 0;
        SceneManager.LoadSceneAsync(1 + boolInt);
    }

    public void GroupB(){
        int boolInt = isTUI ? 1 : 0;
        SceneManager.LoadSceneAsync(3 + boolInt);
    }

    public void tutorialGUI(){
        SceneManager.LoadSceneAsync(5);
    }

    public void tutorialTUI(){
        SceneManager.LoadSceneAsync(6);
    }

    public void Exit(){
        Application.Quit();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject menuCanvas;
    private bool menuActive = false;

    public void BackToMain(){
        SceneManager.LoadSceneAsync(0);
    }

    public void Reset(){
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public void Resume(){
        menuActive = false;
        menuCanvas.SetActive(false);
    }

    public void Exit(){
        Application.Quit();
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape) && !menuActive){
            menuActive = true;
            menuCanvas.SetActive(true);
        }else if (Input.GetKeyDown(KeyCode.Escape) && menuActive){
            menuActive = false;
            menuCanvas.SetActive(false);
        }
    }
}

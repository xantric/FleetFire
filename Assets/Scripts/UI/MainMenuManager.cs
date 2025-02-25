using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject instructionsMenu;
    void Start() {
        mainMenuCanvas.SetActive(true);
        instructionsMenu.SetActive(false);
    }

    void Update() {

    }

    public void StartGame() {
        SceneManager.LoadScene(1);
    }
    public void Instructions() {
        mainMenuCanvas.SetActive(false);
        instructionsMenu.SetActive(true);
    }

    public void QuitGame() {
        //"QUIT");
        Application.Quit();
    }
    public void Back() { 
        mainMenuCanvas.SetActive(true);
        instructionsMenu.SetActive(false);
    }
}

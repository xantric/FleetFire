using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour {

    public GameObject pauseMenuCanvas;
    public GameObject healthBar;
    public bool isPaused;

    void Start() {
        pauseMenuCanvas.SetActive(false);
        healthBar.SetActive(true);
        Time.timeScale = 1.0f;
        isPaused = false;
    }

    void Update() {
        /*if (Input.GetKeyDown(KeyCode.Escape)) {
            if (isPaused) {
                Resume();
                isPaused = false;
            }
            else {
                Pause();
                isPaused=true;
            }
        }*/
    }

    public void Resume() {
        pauseMenuCanvas.SetActive(false);
        healthBar.SetActive(true);
        Time.timeScale = 1.0f;
    }
    public void Pause() { 
        pauseMenuCanvas.SetActive(true);
        healthBar.SetActive(false);
        Time.timeScale = 0.0f;
    }

    public void Restart() {
        //"GameScene");
        SceneManager.LoadScene("GameScene");
        Time.timeScale = 1.0f;
    }
    public void MainMenu(){
        //"MainMenu");
        SceneManager.LoadScene("MainMenu");
    }
}

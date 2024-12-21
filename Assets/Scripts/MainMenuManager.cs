using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainmenuCanvas;
    public GameObject InstructionsMenu;
    // Start is called before the first frame update
    void Start()
    {
        mainmenuCanvas.SetActive(true);
        InstructionsMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void Instructions()
    {
        mainmenuCanvas.SetActive(false);
        InstructionsMenu.SetActive(true);
    }
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
    public void back()
    {
        mainmenuCanvas.SetActive(true);
        InstructionsMenu.SetActive(false);

    }
}

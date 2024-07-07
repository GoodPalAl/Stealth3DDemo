using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    GameObject menuMain = null;
    [SerializeField]
    GameObject menuControls = null;
    [SerializeField]
    GameObject menuOptions = null;

    AudioSource AS;

    private void Start()
    {
        if (menuMain != null && menuControls != null && menuOptions != null)
        {
            menuMain.SetActive(true);
            menuControls.SetActive(false);
            menuOptions.SetActive(false);
            AS = GetComponent<AudioSource>();
            AS.Play();
        }
        else
        {
            Debug.Log("ERROR: menuMain, menuControls, or menuOptions are missing.");
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void QuitGame()
    {
        Debug.Log("Game Quit");
        Application.Quit();
    }
}

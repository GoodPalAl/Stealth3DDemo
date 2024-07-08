using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    GameObject menuPause = null;

    // Music
    /*
    AudioSource AS;
    [SerializeField]
    AudioSource _otherAS;
    //*/

    private void Start()
    {
        if (menuPause != null)
        {
            menuPause.SetActive(false);
            //AS = GetComponent<AudioSource>();
        }
        else
        {
            Debug.Log("ERROR: menuPause is missing.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Space key is pause button
        //if (Input.GetKeyDown(KeyCode.Space) 
        //    && GameManager.instance.getStartStatus())
        // NEW: see KeybindManager
        if (KeybindManager.instance.KeyDown("Pause") 
            && GameManager.instance.GetStartStatus())
        {
            if (!GameManager.instance.GetPauseStatus())
                PauseGame();
        }
    }

    public void PauseGame()
    {
        menuPause.SetActive(true);
        /*
        AS.Play();
        if (_otherAS != null)
            _otherAS.Pause();
        //*/
        Time.timeScale = 0f;
        Debug.Log("Pausing Game..." + Time.timeScale);
        GameManager.instance.SetPauseStatus(true);
    }

    public void ContinueGame()
    {
        menuPause.SetActive(false);
        /*
        AS.Stop();
        if (_otherAS != null)
            _otherAS.UnPause();
        //*/
        Time.timeScale = 1f;
        Debug.Log("Resuming Game..." + Time.timeScale);
        GameManager.instance.SetPauseStatus(false);
    }

    public void RestartGame()
    {
        Debug.Log("Restarting Level...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMainMenu()
    {
        Debug.Log("Quitting to Main Menu..." + (SceneManager.GetActiveScene().buildIndex - 1));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}

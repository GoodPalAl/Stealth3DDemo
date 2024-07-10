using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMenu : MonoBehaviour
{
    [SerializeField]
    GameObject menuEnd = null;
    [SerializeField]
    GameObject gameWinText = null;
    [SerializeField]
    GameObject gameLostText = null;

    AudioSource AS;
    // FIXME: Trigger animation how?
    //Animator Ani;

    private void Start()
    {
        menuEnd.SetActive(false);
        AS = GetComponent<AudioSource>();
        //Ani = GetComponent<Animator>();
    }

    private void Update()
    {
        // If the player moves, change the text
        if (GameManager.instance.GetStartStatus())
        {
            if (GameManager.instance.GetWinStatus())
            {
                //Ani.ResetTrigger("Level Complete");
                menuEnd.SetActive(true);
                gameWinText.SetActive(true);
                gameLostText.SetActive(false);
                //Ani.SetTrigger("Level Complete");
                // FIXME: play win music
                //AS.Play();
            }
            else if (GameManager.instance.GetLossStatus())
            {
                Time.timeScale = 0f;
                menuEnd.SetActive(true);
                gameWinText.SetActive(false);
                gameLostText.SetActive(true);
            }
        }
    }

    public void RestartGame()
    {
        ResetGame();
        Debug.Log("Restarting Level...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        ResetGame();
        Debug.Log("Quitting Game...");
        SceneManager.LoadScene(0);
    }

    private void ResetGame()
    {
        GameManager.instance.SetStartStatus(false);
        GameManager.instance.SetPauseStatus(false);
        GameManager.instance.SetWinStatus(false);
        GameManager.instance.SetLossStatus(false);
        //Ani.ResetTrigger("Level Complete");
    }
}

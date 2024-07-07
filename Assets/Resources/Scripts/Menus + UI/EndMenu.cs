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

    private void Start()
    {
        menuEnd.SetActive(false);
        AS = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // If the player moves, change the text
        if (GameManager.instance.getStartStatus())
        {
            if (GameManager.instance.getWinStatus())
            {
                menuEnd.SetActive(true);
                gameWinText.SetActive(true);
                gameLostText.SetActive(false);
                AS.Play();

            }
            else if (GameManager.instance.getLossStatus())
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
        GameManager.instance.setStartStatus(false);
        GameManager.instance.setPauseStatus(false);
        GameManager.instance.setWinStatus(false);
        GameManager.instance.setLossStatus(false);
        Debug.Log("Restarting Level...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        GameManager.instance.setStartStatus(false);
        GameManager.instance.setPauseStatus(false);
        GameManager.instance.setWinStatus(false);
        GameManager.instance.setLossStatus(false);
        Debug.Log("Restarting Level...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}

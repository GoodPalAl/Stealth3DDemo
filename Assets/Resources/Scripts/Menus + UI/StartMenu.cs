using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StartMenu : MonoBehaviour
{
    [SerializeField]
    GameObject menuStart = null;
    AudioSource AS;

    [SerializeField]
    TextMeshProUGUI keys = null;
    Keybinding keybinds;

    private void Start()
    {
        if (menuStart != null)
        {
            menuStart.SetActive(true);
            GameManager.instance.SetPauseStatus(false);
            GameManager.instance.SetStartStatus(false);
            AS = GetComponent<AudioSource>();
            AS.Play();
            Time.timeScale = 0f;

            // NEW
            keybinds = KeybindManager.instance.keybinding;

            keys.text =
                keybinds.forward.ToString() + " " +
                keybinds.left.ToString() + " " +
                keybinds.back.ToString() + " " +
                keybinds.right.ToString() + "\n" +
                "Mouse Movement" + "\n" + 
                keybinds.pause.ToString();
        }
        else
        {
            Debug.Log("ERROR: menuStart are missing.");
        }
    }

    public void StartGame()
    {
        menuStart.SetActive(false);
        Time.timeScale = 1f;
        Debug.Log("Starting Game...");
        GameManager.instance.SetStartStatus(true);
    }
}

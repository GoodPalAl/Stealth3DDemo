using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager instance;

    void Awake()
    {
        instance = this;
        /*
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(this);
        DontDestroyOnLoad(this);
        //*/
    }

    #endregion

    public GameObject mainCamera;

    static bool hasStarted = false;
    static bool isPaused = false;
    static bool hasWon = false;
    static bool hasLost = false;

    public bool getStartStatus() { return hasStarted; }
    public void setStartStatus(bool newStatus) { hasStarted = newStatus; }

    public bool getPauseStatus() { return isPaused; }
    public void setPauseStatus(bool newStatus) { isPaused = newStatus; }

    public bool getWinStatus() { return hasWon; }
    public void setWinStatus(bool newStatus) { hasWon = newStatus; }
    public bool getLossStatus() { return hasLost; }
    public void setLossStatus(bool newStatus) { hasLost = newStatus; }
}

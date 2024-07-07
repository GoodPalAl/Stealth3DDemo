/*
 * Al A.
 * Summer 2020 / Summer 2024 (c)
 * This script is the game manager. There should only be ONE of these in an instance.
 * In charge of keeping track of the game's current state.
 * This is useful for other scripts that need to reference this sort of data.
 */
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

    // VARIABLES
    // Keeps track of if the game has started or not.
    static bool hasStarted = false;
    // Keeps track of if the game is paused or not.
    static bool isPaused = false;
    // Keeps track of if the player has won or not.
    static bool hasWon = false;
    // Keeps track of if the player has lost or not.
    static bool hasLost = false;

    public bool getStartStatus() => hasStarted; 
    public void setStartStatus(bool newStatus) => hasStarted = newStatus;

    public bool getPauseStatus() => isPaused; 
    public void setPauseStatus(bool newStatus) => isPaused = newStatus;

    public bool getWinStatus() => hasWon; 
    public void setWinStatus(bool newStatus) => hasWon = newStatus; 
    public bool getLossStatus() => hasLost;
    public void setLossStatus(bool newStatus) => hasLost = newStatus;
}

/* 
 * Al A.
 * Summer 2020 / Summer 2024 (c)
 */
using UnityEngine;

/// <summary>
/// This script is the game manager. There should only be ONE of these in an instance.
/// In charge of keeping track of the game's current state.
/// This is useful for other scripts that need to reference this sort of data.
/// </summary>
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

    // COMPONENTS
    /// <summary>
    /// References Main Camera of the game.
    /// </summary>
    public GameObject mainCamera;

    // VARIABLES
    /// <summary>
    /// Keeps track of if the game has started or not.
    /// </summary>
    static bool hasStarted = false;
    /// <summary>
    /// Keeps track of if the game is paused or not.
    /// </summary>
    static bool isPaused = false;
    /// <summary>
    /// Keeps track of if the player has won or not.
    /// </summary>
    static bool hasWon = false;
    /// <summary>
    /// Keeps track of if the player has lost or not.
    /// </summary>
    static bool hasLost = false;

    // FUNCTIONS
    public bool getStartStatus() => hasStarted; 
    public void setStartStatus(bool newStatus) => hasStarted = newStatus;

    public bool getPauseStatus() => isPaused; 
    public void setPauseStatus(bool newStatus) => isPaused = newStatus;

    public bool getWinStatus() => hasWon; 
    public void setWinStatus(bool newStatus) => hasWon = newStatus; 
    public bool getLossStatus() => hasLost;
    public void setLossStatus(bool newStatus) => hasLost = newStatus;
}

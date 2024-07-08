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
    // VARIABLES
    /// <summary>
    /// Keeps track of if the game has started or not.
    /// </summary>
    static bool HasStarted = false;
    /// <summary>
    /// Keeps track of if the game is paused or not.
    /// </summary>
    static bool IsPaused = false;
    /// <summary>
    /// Keeps track of if the player has won or not.
    /// </summary>
    static bool HasWon = false;
    /// <summary>
    /// Keeps track of if the player has lost or not.
    /// </summary>
    static bool HasLost = false;

    // FUNCTIONS
    /// <summary>
    /// Fetches if level has started.
    /// </summary>
    /// <returns>True if level has started, false if it has not.</returns>
    public bool GetStartStatus() => HasStarted; 
    /// <summary>
    /// Sets if level has started or not. True typically after the "start menu" button is pressed.
    /// </summary>
    /// <param name="newStatus"></param>
    public void SetStartStatus(bool newStatus) => HasStarted = newStatus;

    public bool GetPauseStatus() => IsPaused; 
    public void SetPauseStatus(bool newStatus) => IsPaused = newStatus;

    public bool GetWinStatus() => HasWon; 
    public void SetWinStatus(bool newStatus) => HasWon = newStatus; 
    public bool GetLossStatus() => HasLost;
    public void SetLossStatus(bool newStatus) => HasLost = newStatus;
}

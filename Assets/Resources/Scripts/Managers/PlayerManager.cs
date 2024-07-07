using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Dynamically keeps track of the player for enemy detection
public class PlayerManager : MonoBehaviour
{
    #region Singleton

    public static PlayerManager instance;

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

    public GameObject player;
    bool isHidden = true;
    List<GameObject> enemiesSpottedPlayer = new List<GameObject>();

    public bool getHideStatus() { return isHidden; }
    public void setHideStatus(bool newStatus) { isHidden = newStatus; }

    public void enemySpotsPlayer(GameObject enemyToAdd) { enemiesSpottedPlayer.Add(enemyToAdd); }
    public void enemyLosesPlayer(GameObject enemyToRemove) { enemiesSpottedPlayer.Remove(enemyToRemove); }
    public bool isEnemyFollowingPlayer(GameObject enemyToFind) { return enemiesSpottedPlayer.Contains(enemyToFind); }
    public int enemyFollowingCount()
    {
        List<GameObject> temp = enemiesSpottedPlayer;
        int count = 0;
        foreach (GameObject g in temp)
            ++count;

        return count;
    }

    private void FixedUpdate()
    {
        if (enemiesSpottedPlayer.Count >= 1)
            setHideStatus(false);
        else
            setHideStatus(true);
    }
}

using UnityEngine;

/// <summary>
/// Triggers when player reaches the goal.
/// </summary>
public class GoalTrigger : MonoBehaviour
{
    // COMPONENTS
    /// <summary>
    /// Transform of player's character.
    /// </summary>
    Transform player;
    /// <summary>
    /// Collider of the goal barrier.
    /// </summary>
    public Collider barrier;

    private void Start()
    {
        player = PlayerManager.instance.player.transform;
    }

    private void OnTriggerEnter()
    {
        // If player is hidden from enemies and player has not lost.
        if (PlayerManager.instance.getHideStatus() 
            && !GameManager.instance.getLossStatus())
        {
            // Goal Barrier will ignore collision with player collider.
            Physics.IgnoreCollision(player.GetComponent<Collider>(), barrier);
            // Tell the game manager that the player has won!
            GameManager.instance.setWinStatus(true);
        }
        else
        {
            // Disallow player from winning if they are currently being hunted.
            Debug.Log("You cannot complete level while detected.");
        }
    }
}

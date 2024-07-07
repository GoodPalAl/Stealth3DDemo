using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    Transform player;
    public Collider barrier;

    private void Start()
    {
        player = PlayerManager.instance.player.transform;
    }

    private void OnTriggerEnter()
    {
        if (PlayerManager.instance.getHideStatus() 
            && !GameManager.instance.getLossStatus())
        {
            Physics.IgnoreCollision(player.GetComponent<Collider>(), barrier);
            GameManager.instance.setWinStatus(true);
        }
        else
        {
            Debug.Log("You cannot complete level while detected.");
        }
    }
}

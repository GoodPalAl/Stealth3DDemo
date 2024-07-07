using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    // Unity Objects
    Transform player;

    [SerializeField]
    bool isHidingArea = true;

    private void Start()
    {
        player = PlayerManager.instance.player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        // If an environment object is considered a "hiding area", allow the player to ignore it's collision.
        if (isHidingArea)
            Physics.IgnoreCollision(player.GetComponent<Collider>(), GetComponent<Collider>());
    }
}

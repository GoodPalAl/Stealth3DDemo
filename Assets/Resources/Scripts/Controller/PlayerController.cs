/*
 * Al A.
 * Summer 2020 / Summer 2024 (c)
 */
using UnityEngine;

/// <summary>
/// In charge of controlling the player's character. 
/// Movement, speed, etc.
/// </summary>
public class PlayerController : MonoBehaviour
{
    // COMPONENTS
    /// <summary>
    /// Character Controller game object attached to player model.
    /// </summary>
    public CharacterController controller;
    /// <summary>
    /// Transform component of the main camera.
    /// </summary>
    Transform cam;

    /// <summary>
    /// Player's speed as a float.
    /// </summary>
    [SerializeField]
    float moveSpeed = 10f;
    /// <summary>
    /// Speed in which the player's model turns.
    /// </summary>
    float turnSmoothTime = 0.1f;
    /// <summary>
    /// Current speed of the player's model turning. 
    /// </summary>
    float turnSmoothVelocity;
    
    // Start is called once when the game starts.
    private void Start()
    {
        // Fetch camera from GameManager.
        cam = GameManager.instance.mainCamera.transform;
    }

    // Update is called once per frame
    void Update()
    {
        // Only allow player movement if the player hasn't lost.
        if (!GameManager.instance.getLossStatus())
        {
            PlayerMovement();
        }
    }

    /// <summary>
    /// Gives control to player to move their character model.
    /// </summary>
    void PlayerMovement()
    {
        // Gets input using the current keybinding for left and right movement.
        float horizontal = KeybindManager.instance.GetAxisRaw("Horizontal");
        // Gets input using the current keybinding for forward and backward movement.
        float vertical = KeybindManager.instance.GetAxisRaw("Vertical");

        // Store input from user in a vector.
        // y = 0 because there should be only movement along the xz-plane.
        // Normalize vector to "fix" object's speed when moving diagonally.
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;   
        
        // If the player moves in any direction
        if (direction.magnitude > 0f)
        {
            // Point player towards direction of movement.
            //  atan2(z,x) returns a counterclockwise angle, 
            //   in radians, on the xz-plane between 
            //   the +x axis and a ray that points 
            //   from where the player currently is (0,0)
            //   to where the player is moving to (x,z).
            //  Unity's starting angle is +y axis and rotates clockwise. 
            //   Passing the arguments into atan2 as (x,z) fixes this issue.
            float directionAngle = Mathf.Atan2(direction.x, direction.z) 
                * Mathf.Rad2Deg      // Converts radians to degrees.
                + cam.eulerAngles.y; // Direction in reference to camera's position for input consistency.

            // Slows/Smooths rotation.
            float directionAngleSmooth = Mathf.SmoothDampAngle(transform.eulerAngles.y, directionAngle, ref turnSmoothVelocity, turnSmoothTime);

            // Rotation applied to player.
            //  Quaternion.Euler(x,y,z) returns a vector3 rotation 
            //   that rotates x,y,z degrees around the 
            //   x-, y-, z-axes respectively. 
            //  Use the "smooth" angle so the player doesn't "snap" to the direction.
            Vector3 turnDirection = new Vector3(0f, directionAngleSmooth, 0f);
            transform.rotation = Quaternion.Euler(turnDirection);

            // Movement applied to player.
            Vector3 moveDirection = Quaternion.Euler(0f, directionAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
        }
    }
}

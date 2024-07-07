using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Reference objects
    public CharacterController controller;
    Transform cam;

    [SerializeField]
    float moveSpeed = 10f;          // Player speed
    float turnSmoothTime = 0.1f;    // Player's turning speed
    float turnSmoothVelocity;
    
    private void Start()
    {
        cam = GameManager.instance.mainCamera.transform;
    }
    // Update is called once per frame
    void Update()
    {
        // Disable player movement if player loses
        if (!GameManager.instance.getLossStatus())
            PlayerMovement();
    }

    // Moves player
    void PlayerMovement()
    {
        // Gets input using WASD 
        // This also works with arrow keys
        //float horizontal = Input.GetAxisRaw("Horizontal");  // A and D 
        //float vertical = Input.GetAxisRaw("Vertical");      // W and S
        // NEW: see KeybindManagers
        float horizontal = KeybindManager.instance.GetAxisRaw("Horizontal");  // forward and back
        float vertical = KeybindManager.instance.GetAxisRaw("Vertical");      // left and right

        //Debug.Log(horizontal + ", " + vertical);

        // Store input from user in a vector
        // y = 0 because there should be only movement along the XZ plane
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;   // ".normalized" fixes object's speed when moving diagonally
        
        // If the player moves in any direction
        if (direction.magnitude > 0f)
        {
            // Point player towards direction of movement
            //  atan2(z,x) returns a counterclockwise angle, 
            //   in radians, on the xz plane between 
            //   the +x axis and a ray that points 
            //   from where the player currently is (0,0)
            //   to where the player is moving to (x,z).
            //  Unity's starting angle is +y axis and rotates clockwise. 
            //   Passing the arguments into atan2 as (x,z) fixes this issue.
            float directionAngle = Mathf.Atan2(direction.x, direction.z) 
                * Mathf.Rad2Deg      // Converts radians to degrees
                + cam.eulerAngles.y; // Direction in reference to camera's position for input consistency

            // Slows/Smooths rotation
            float directionAngleSmooth = Mathf.SmoothDampAngle(transform.eulerAngles.y, directionAngle, ref turnSmoothVelocity, turnSmoothTime);

            // Rotation applied to player.
            //  Quaternion.Euler(x,y,z) returns a vector3 rotation 
            //   that rotates x,y,z degrees around the 
            //   x-, y-, z-axes respectively. 
            //  Use the "smooth" angle so the player doesn't "snap" to the direction.
            Vector3 turnDirection = new Vector3(0f, directionAngleSmooth, 0f);
            transform.rotation = Quaternion.Euler(turnDirection);

            // Movement applied to player
            Vector3 moveDirection = Quaternion.Euler(0f, directionAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
        }
    }
}

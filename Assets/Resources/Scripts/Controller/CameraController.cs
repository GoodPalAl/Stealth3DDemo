using UnityEngine;
using System.Collections;

// This class tells the camera to follow the player, third-person style
public class CameraController : MonoBehaviour
{
    // Daylight Sky: 7F99BC
    // Nighttime Sky: 4B4665

    // Update is called once per frame
    void Update()
    {
        // If the right mouse button is pressed, 
        //  activate camera movement with mouse
        if (Input.GetKey(KeyCode.Mouse1))
        {
            // Enable camera control?
        }
    }
}

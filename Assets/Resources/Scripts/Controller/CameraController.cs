/*
 * Al A.
 * Summer 2020 (c)
 */

using Cinemachine;
using UnityEngine;

/// <summary>
/// Enables camera control once the game starts.
/// </summary>
[RequireComponent(typeof(CinemachineBrain))]
public class CameraController : MonoBehaviour
{
    // Daylight Sky: 7F99BC
    // Nighttime Sky: 4B4665
    CinemachineBrain camBrain;
    bool gameStartStatusLastFrame = false;
    bool gameStartStatusThisFrame;

    private void Start()
    {
        camBrain = CameraManager.instance.MainCam.GetComponent<CinemachineBrain>();
        // Disable camera before game starts.
        camBrain.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        CameraControl();
    }

    void CameraControl()
    {
        // Fetch game start status.
        gameStartStatusThisFrame = GameManager.instance.GetStartStatus();

        // Update camera control only if the game start status has changed from the previous frame.
        // Prevents code from reassigning the camera enabling every frame.
        if (gameStartStatusLastFrame != gameStartStatusThisFrame)
        {
            camBrain.enabled = gameStartStatusThisFrame;
            Debug.Log("Camera status updated: " + camBrain.enabled);
        }
        gameStartStatusLastFrame = gameStartStatusThisFrame;
    }
}

using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    #region Singleton

    public static CameraManager instance;

    void Awake()
    {
        instance = this;
        MainCam = FindAnyObjectByType<Camera>();
        if (MainCam != null)
        {
            if (!CameraManager.instance.MainCam.TryGetComponent<CinemachineBrain>(out CinemachineBrain component))
            {
                Debug.Log("ERROR: MainCam failed to have brain.");
            }
            else
            {
                Debug.Log("Camera with brain has been assigned to Game Manager! " + MainCam.name);
            }
        }
        else
        {
            Debug.Log("ERROR camera not found.");
        }

        FreeLookCam = FindAnyObjectByType<CinemachineFreeLook>();
        if (FreeLookCam != null)
        {
            Debug.Log("FreeLook Camera assigned to Game Manager! " + FreeLookCam.name);
        }
        else
        {
            Debug.Log("ERROR freelook camera not found.");
        }
    }


    #endregion

    /// <summary>
    /// References Main Camera of the game.
    /// </summary>
    public CinemachineFreeLook FreeLookCam;
    public Camera MainCam;

}


using UnityEngine;

public class KeybindManager : MonoBehaviour
{
    #region Singleton

    public static KeybindManager instance;
    public Keybinding keybinding;

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
    
    public bool KeyDown(string key)
    {
        if (Input.GetKeyDown(keybinding.GetKey(key)))
            return true;
        else
            return false;
    }
    public bool Key(string key)
    {
        if (Input.GetKey(keybinding.GetKey(key)))
            return true;
        else
            return false;
    }

    // See PlayerController
    public float GetAxisRaw(string axisName)
    {
        switch (axisName)
        {
            case "Vertical":
                if (Key("Forward"))
                    return 1;
                else if (Key("Back"))
                    return -1;
                else
                    return 0;
            case "Horizontal":
                if (Key("Right"))
                    return 1;
                else if (Key("Left"))
                    return -1;
                else
                    return 0;

            default:
                return 0;
        }
    }
}

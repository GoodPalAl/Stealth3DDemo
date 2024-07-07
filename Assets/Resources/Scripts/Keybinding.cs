using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NEW
// Scriptable Object
[CreateAssetMenu(fileName ="Keybinding", menuName ="Keybinding")]
public class Keybinding : ScriptableObject
{
    public KeyCode forward, left, back, right, pause;

    [ContextMenu("Reset to Default")]
    public void Default()
    {
        forward = KeyCode.W;
        left = KeyCode.A;
        back = KeyCode.S;
        right = KeyCode.D;
        pause = KeyCode.Space;
    }
    public KeyCode GetKey(string key)
    {
        switch (key)
        {
            case "Pause":
                return pause;

            case "Forward":
                return forward;
            case "Left":
                return left;
            case "Right":
                return right;
            case "Back":
                return back;

            default:
                return KeyCode.None;
        }
    }
    public void SetKey(string command, KeyCode newkey)
    {
        switch (command)
        {
            case "Pause":
                pause = newkey;
                break;

            case "Forward":
                forward = newkey;
                break;
            case "Left":
                left = newkey;
                break;
            case "Right":
                right = newkey;
                break;
            case "Back":
                back = newkey;
                break;

            default:
                Debug.Log("???");
                break;
        }
    }
}

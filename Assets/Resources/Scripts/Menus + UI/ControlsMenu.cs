using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// NEW
public class ControlsMenu : MonoBehaviour
{
    [SerializeField]
    Transform buttonPanel;
    Event keyEvent;
    TextMeshProUGUI buttonText;
    KeyCode newKey;

    bool waitingForKey;

    private void Start()
    {
        waitingForKey = false;

        if (buttonPanel == null)
            Debug.Log("ERROR: buttonPanel missing.");
    }

    private void Update()
    {
        if (buttonPanel != null)
        {
            for (int i = 0; i < buttonPanel.childCount; ++i)
            {
                if (buttonPanel.GetChild(i).name == "forward_button")
                    buttonPanel.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text
                        = KeybindManager.instance.keybinding.forward.ToString();
                else if (buttonPanel.GetChild(i).name == "back_button")
                    buttonPanel.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text
                        = KeybindManager.instance.keybinding.back.ToString();
                else if (buttonPanel.GetChild(i).name == "left_button")
                    buttonPanel.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text
                        = KeybindManager.instance.keybinding.left.ToString();
                else if (buttonPanel.GetChild(i).name == "right_button")
                    buttonPanel.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text
                        = KeybindManager.instance.keybinding.right.ToString();
                else if (buttonPanel.GetChild(i).name == "pause_button")
                    buttonPanel.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text
                        = KeybindManager.instance.keybinding.pause.ToString();
            }
        }
    }

    private void OnGUI()
    {
        keyEvent = Event.current;

        if (keyEvent.isKey && waitingForKey)
        {
            newKey = keyEvent.keyCode;
            waitingForKey = false;
        }
    }

    public void StartAssignment(string keyname)
    {
        if (!waitingForKey)
            StartCoroutine(AssignKey(keyname));
    }

    public void SentText(TextMeshProUGUI text)
    {
        buttonText = text;
    }

    IEnumerator WaitforKey()
    {
        while (!keyEvent.isKey)
            yield return null;
    }

    public IEnumerator AssignKey(string keyName)
    {
        waitingForKey = true;

        yield return WaitforKey();

        KeybindManager.instance.keybinding.
            SetKey(keyName, newKey);
        buttonText.text = KeybindManager.instance.
            keybinding.GetKey(keyName).ToString();

        yield return null;
    }
}

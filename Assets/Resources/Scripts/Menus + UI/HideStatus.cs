using UnityEngine;
using UnityEngine.UI;

// Makes the text that appears when the game starts disappear
// At the moment it just disappears, but I eventually want to make it fade out
public class HideStatus : MonoBehaviour
{
    // Define objects in Unity
    Transform player;
    //[SerializeField]
    //GameObject titleText = null;
    [SerializeField]
    GameObject hideStatus = null;
    [SerializeField]
    GameObject hiddenText = null;
    [SerializeField]
    GameObject foundText = null;

    // Start is called before the first frame update
    private void Start()
    {
        player = PlayerManager.instance.player.transform;
        hideStatus.SetActive(false);
        hiddenText.SetActive(false);
        foundText.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        // If the player moves, change the text
        if (GameManager.instance.getStartStatus())
        {
            if (!GameManager.instance.getWinStatus())
            {
                if (PlayerManager.instance.getHideStatus())
                {
                    hiddenText.SetActive(true);
                    foundText.SetActive(false);
                }
                if (!PlayerManager.instance.getHideStatus())
                {
                    hiddenText.SetActive(false);
                    foundText.SetActive(true);
                }
                // If game is paused.
                if (Time.timeScale == 0f)
                {
                    hideStatus.SetActive(false);
                }
                else
                {
                    hideStatus.SetActive(true);
                }
            }
            if (GameManager.instance.getWinStatus() 
                || GameManager.instance.getLossStatus())
            {
                hideStatus.SetActive(false);
            }
        }
    }

    /*
    void PlayerFoundText()
    {
        statusText.GetComponent<Text>().color = Color.red;
        statusText.GetComponent<Text>().fontStyle = FontStyle.Bold;
        statusText.GetComponent<Text>().text = "Detected";
    }
    void PlayerHidText()
    {
        statusText.GetComponent<Text>().color = Color.white;
        statusText.GetComponent<Text>().fontStyle = FontStyle.Normal;
        statusText.GetComponent<Text>().text = "Hidden";
    }
    //*/
}

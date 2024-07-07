/*
 * Al A.
 * Summer 2020 (c)
 */
using UnityEngine;

public class BushNoise : MonoBehaviour
{
    //[SerializeField]
    //AudioMixer audioMixer;
    [SerializeField]
    AudioClip triggerSound;
    AudioSource AS;

    private void Start()
    {
        AS = GetComponent<AudioSource>();
    }

    public void OnTriggerEnter(Collider other)
    {
        // If something touches the bush, make a noise.
        if (triggerSound != null)
            AS.PlayOneShot(triggerSound, 5f);
        Debug.Log("bush noises");
    }
}

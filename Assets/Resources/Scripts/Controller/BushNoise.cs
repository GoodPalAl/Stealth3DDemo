using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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
        if (triggerSound != null)
            AS.PlayOneShot(triggerSound, 5f);
        Debug.Log("bush noises");
    }
}

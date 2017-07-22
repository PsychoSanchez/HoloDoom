using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour {

    public AudioClip GameMusic;
    AudioSource audioSource;
    // Use this for initialization
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(GameMusic, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

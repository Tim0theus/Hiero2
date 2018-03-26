using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {

    [Serializable]
    public struct Sounds
    {
        public string name;
        public AudioClip source;
        public float volume;
    }

    public Sounds[]  tempS;
    Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>();

    public static SoundController instance;

    public AudioSource aSource;

	// Use this for initialization
	void Start () {
		if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        for (int i = 0; i < tempS.Length; i++)
        {
            sounds[tempS[i].name] = tempS[i].source;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Play(string audio)
    {
        aSource.clip = sounds[audio];
        aSource.Play();
    }

}

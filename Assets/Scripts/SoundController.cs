using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {

	static SoundController instance;
	AudioSource source;

	// Use this for initialization
	void Start () {
		source = GetComponent<AudioSource> ();
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public static void PlayOneShot (AudioClip clip) {
		instance.source.PlayOneShot (clip);
	}

	public static void PlayOneShot (AudioClip clip, float volume) {
		Debug.Log (volume);
		instance.source.PlayOneShot (clip, volume);
	}
}

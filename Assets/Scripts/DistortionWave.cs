﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistortionWave : MonoBehaviour {

	public float maxSize = 170f;
	public float expandSpeed = 140f;
	float curScale = 0f;
	Transform trans;
	Renderer rend;
	static DistortionWave instance;

	// Initialise stuff
	void Start () {
		trans = transform;
		instance = this;
		rend = GetComponent<Renderer> ();
		rend.enabled = false;
	}

	// Reset and play the wave animation
	public static void Play (Vector2 position) {
		instance.StopAllCoroutines ();
		instance.trans.position = new Vector3 (position.x, position.y, instance.trans.position.z);
		instance.StartCoroutine (instance.WaveRoutine());
	}

	// Get wave bigger until max size, then disable
	IEnumerator WaveRoutine () {
		rend.enabled = true;
		curScale = 0;
		while (curScale < maxSize) {
			yield return new WaitForEndOfFrame ();
			curScale += Time.unscaledDeltaTime * expandSpeed;
			trans.localScale = curScale * Vector3.one;
		}
		rend.enabled = false;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

	public float intensity = 0.5f;
	float curShake = 0f;
	public float speed = 3.5f;
	public float decay = 1.5f;
	Vector3 startPos;
	Transform trans;
	static CameraShake instance;
	public AudioSource shakeNoise;

	// Use this for initialization
	void Start () {
		trans = transform;
		startPos = trans.position;
		instance = this;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		curShake = Mathf.Clamp01 (curShake - (Time.unscaledDeltaTime * decay));
		shakeNoise.volume = curShake * curShake;
		trans.position = startPos + (new Vector3 (Mathf.PerlinNoise (Time.unscaledTime * speed, 0) - 0.5f, Mathf.PerlinNoise (0, Time.unscaledTime * speed) - 0.5f) * intensity * curShake * curShake);
	}

	public static void Shake (float shakeAmount) {
		instance.curShake = shakeAmount;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceEffect : MonoBehaviour {

	public Vector3 sliceStartScale;
	public Vector3 sliceFinalScale;
	public AnimationCurve sliceAnimCurve;
	public float sliceAnimLength = 0.5f;

	static SliceEffect[] instances;
	static int curInstance = 0;
	Transform trans;
	Renderer rend;
	float startTime;

	void Start () {
		AddThisToInstances();
		trans = transform;
		rend = GetComponentInChildren<Renderer>();
		rend.enabled = false;
	}
	
	public static void Slice (Vector3 pos) {
		instances[curInstance]._Slice(pos);
		curInstance = (curInstance + 1) % instances.Length;
	}

	void AddThisToInstances () {
		if (instances != null) {
			SliceEffect[] newInstances = new SliceEffect[instances.Length + 1];
			for (int i = 0; i < instances.Length; ++i) {
				newInstances[i] = instances[i];
			}
			newInstances[instances.Length] = this;
			instances = newInstances;
		} else {
			instances = new SliceEffect[1];
			instances[0] = this;
		}
	}

	void _Slice (Vector3 pos) {
		StopAllCoroutines();
		startTime = Time.time;
		rend.enabled = true;
		trans.position = pos;
		StartCoroutine(SliceAnim());
	}

	IEnumerator SliceAnim () {

		while (Time.time < startTime + sliceAnimLength) {

			trans.localScale = Vector3.LerpUnclamped(sliceStartScale, sliceFinalScale, sliceAnimCurve.Evaluate((Time.time - startTime) / sliceAnimLength));

			yield return new WaitForFixedUpdate();
		}

		rend.enabled = false;
	}
}

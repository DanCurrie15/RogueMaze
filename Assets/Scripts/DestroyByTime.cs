using UnityEngine;
using System.Collections;

public class DestroyByTime : MonoBehaviour {

	public float lifetime;

	private float startTime;

	void Start () {
		startTime = Time.time;
		//Debug.Log("start particle effect");
	}

	void Update () {

		if (Time.time > (lifetime + startTime)) {
			//Debug.Log("end particle effect");
			Destroy(this.gameObject);
		}
	}
}

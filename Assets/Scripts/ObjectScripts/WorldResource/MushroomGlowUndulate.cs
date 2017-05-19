using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomGlowUndulate : MonoBehaviour {

	Light light;
	// Use this for initialization
	void Start () {
		light = GetComponent<Light> ();
	}
	
	// Update is called once per frame
	void Update () {
		light.intensity = 2f + Mathf.Sin (Time.timeSinceLevelLoad);
	}
}

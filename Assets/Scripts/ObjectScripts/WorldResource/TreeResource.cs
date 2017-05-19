using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeResource : MonoBehaviour {

	public int current = 40;
	public int max = 40;
	private float collapseDistance = 8f;
	private float collapseStep = 0.05f;
	private float collapseProgress = 0f;

	private GameObject tree;
	private GameObject trunk;
	private int collapseMax;

	void Start () {
		tree = transform.Find ("Tree").gameObject;
		//trunk = transform.Find ("Trunk").gameObject;
		//collapseMax = Mathf.RoundToInt (collapseDistance / collapseStep);
	}
	
	// Update is called once per frame
	void Update () {
		if (current <= 0) {
			if (collapseProgress < collapseDistance) {
				// resource is exhausted and has not fully collapsed yet.
				tree.transform.Translate (new Vector3 (0, -1 * collapseStep * Time.deltaTime, 0));
				collapseProgress += collapseStep * Time.deltaTime;
				collapseStep += collapseStep/10;
			}
		}
	}
}

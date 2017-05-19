using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceUIHandler : MonoBehaviour {

	public int currentWood;
	public int maxWood;
	public Text woodText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		woodText.text = ""+currentWood;// + "/" + maxWood;
	}
}

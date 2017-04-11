using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerHandler : MonoBehaviour {

	public GameObject target; // Target for the worker.
	public float timeBetweenActions = 0.5f; // Time between worker actions (each chop of wood, each swing etc.)
	public bool targetInRange; // Boolean recording whether a target is in range.
	public int gatherAmount;

	Animator anim;
	GameObject player;
	CharacterMovement charMovement;
	ResourceUIHandler resourceUI;

	float timer;

	void Awake() {
		anim = GetComponent<Animator> ();
		player = GameObject.FindGameObjectWithTag ("Player");
		charMovement = GetComponent<CharacterMovement> ();	
		resourceUI = player.GetComponent<ResourceUIHandler> ();
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject == target) {
			Debug.Log ("entered target");
			targetInRange = true;
			Debug.Log (transform.forward * 0.1f);
			charMovement.destination = transform.position + (transform.forward * 0.1f);			// Stop trying to reach the center of the target.
			charMovement.commandedRecently = true;
		}
	}

	void OnTriggerExit(Collider other){
		if (other.gameObject == target) {
			Debug.Log ("Walked away!");
			targetInRange = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (target != null) {
			timer += Time.deltaTime;
			if (targetInRange) {
				if (timer >= timeBetweenActions) {
					TakeAction ();
				}
			} else if (!targetInRange && charMovement.destination != target.transform.position) { // If we have a target and we're not in range yet, and we're not heading that way already, start heading that direction.
				charMovement.destination = target.transform.position; 
				charMovement.commandedRecently = true;
			}
		} else {
			// Reset all animations.
			anim.SetBool ("IsLumbering", false);
			targetInRange = false;
		}
	}

	void TakeAction(){
		timer = 0f;
		if (target.CompareTag ("WoodResource")) {
			ExhaustibleResource resource = target.GetComponent<ExhaustibleResource> ();
			if (resource != null) {
				if (resource.current > 0) {
					anim.SetBool ("IsLumbering", true);
					//transform.LookAt (target.transform.position);
					int amntRemoved = resource.current - gatherAmount >= 0 ? gatherAmount : resource.current;
					resource.current -= amntRemoved;
					resourceUI.currentWood += amntRemoved; 

				} else {
					// Resource exhausted
					target = null;
					targetInRange = false;
				}
			} else {
				Debug.Log ("Error can't harvest resource! No script to interact with.");
			}
		}
	}
}

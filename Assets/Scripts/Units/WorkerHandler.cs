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
		this.anim = GetComponent<Animator> ();
		this.player = GameObject.FindGameObjectWithTag ("Player");
		this.charMovement = GetComponent<CharacterMovement> ();	
		this.resourceUI = player.GetComponent<ResourceUIHandler> ();
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject == this.target) {
			if (this.targetInRange == false) {
				this.targetInRange = true;
				this.charMovement.destination = transform.position + (transform.forward * 0.15f); // Stop trying to reach the center of the target.
				this.charMovement.commandedRecently = true;
			}

		}
	}

	void OnTriggerExit(Collider other){
		if (other.gameObject == this.target) {
			this.targetInRange = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (this.target != null) {
			this.timer += Time.deltaTime;
			if (this.targetInRange) {
				if (this.timer >= timeBetweenActions) {
					TakeAction ();
				}
			} else if (!this.targetInRange && this.charMovement.destination != this.target.transform.position) { // If we have a target and we're not in range yet, and we're not heading that way already, start heading that direction.
				this.charMovement.destination = this.target.transform.position; 
				this.charMovement.commandedRecently = true;
			}
		} else {
			// Reset all animations.
			this.anim.SetBool ("IsLumbering", false);
			this.targetInRange = false;
		}
	}

	void TakeAction(){
		this.timer = 0f;
		if (this.target.CompareTag ("WoodResource")) {
			ExhaustibleResource resource = this.target.GetComponent<ExhaustibleResource> ();
			if (resource != null) {
				if (resource.current > 0) {
					this.anim.SetBool ("IsLumbering", true);
					int amntRemoved = resource.current - gatherAmount >= 0 ? gatherAmount : resource.current;
					resource.current -= amntRemoved;
					resourceUI.currentWood += amntRemoved; 

				} else {
					// Resource exhausted
					this.target = null;
					this.targetInRange = false;
				}
			} else {
				Debug.Log ("Error can't harvest resource! No script to interact with.");
			}
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorkerHandler : MonoBehaviour {

	public GameObject target; // Target for the worker.
	public float timeBetweenActions = 0.5f; // Time between worker actions (each chop of wood, each swing etc.)
	public bool targetInRange; // Boolean recording whether a target is in range.
	public int gatherAmount;
	public int atExit = 0; // This is an integer rather than a boolean because this fixes a problem with moving between sections of multi-tile exit zones.

	Animator anim;
	AudioSource audioSource;
	CharacterMovement charMovement;
	GameObject player;
	ResourceUIHandler resourceUI;

	AudioClip woodChop_clip;


	float timer;

	void Awake() {
		this.anim = GetComponent<Animator> ();
		this.audioSource = GetComponent<AudioSource> ();
		this.charMovement = GetComponent<CharacterMovement> ();	
		this.player = GameObject.FindGameObjectWithTag ("Player");
		this.resourceUI = player.GetComponent<ResourceUIHandler> ();

		woodChop_clip = Resources.Load ("Audio/SoundEffects/WoodChop") as AudioClip;

	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject == this.target) {
			if (this.targetInRange == false) {
				this.targetInRange = true;
				this.charMovement.destination = transform.position; //+ (transform.forward * 0.15f); // Stop trying to reach the center of the target.
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
			} else { // If we have a target and we're not in range yet, and we're not heading that way already, start heading that direction.
				this.anim.SetBool ("IsLumbering", false);
				NavMeshHit navHit;
				if (NavMesh.SamplePosition (this.target.transform.position, out navHit, 15, NavMesh.AllAreas)) { // This is incredibly janky. I really need a way of determining these positions less often.
					this.charMovement.destination = navHit.position;
				}
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
			TreeResource resource = this.target.transform.parent.GetComponent<TreeResource> ();
			if (resource != null) {
				if (resource.current > 0) {
					this.anim.SetBool ("IsLumbering", true);
					this.audioSource.clip = woodChop_clip;
					this.audioSource.pitch = Random.Range (0.7f, 1.1f);
					this.audioSource.volume = Random.Range (0.7f, 1f);
					this.audioSource.PlayDelayed (0.52f);

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

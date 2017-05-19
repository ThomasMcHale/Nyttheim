using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterMovement : MonoBehaviour {

	public AudioClip attackSound;
	private float timeBetweenAttacks = 2.8f;


	private GameObject[] villagers;
	private NavMeshAgent nav;
	private Animator anim;
	private AudioSource audioSource;
	bool targetInRange;
	VillagerHealth targetHealth;
	GameObject target;
	float attackTimer;
	float targetReacquisitionTimer = 1f; // So our monster acquires a target immediately upon spawning. 
	bool isAttacking;
	float attackAnimationTimer;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		villagers = GameObject.FindGameObjectsWithTag ("Norse");
		this.nav = GetComponent<NavMeshAgent> ();
		audioSource = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		attackTimer += Time.deltaTime;
		targetReacquisitionTimer += Time.deltaTime;

		if (targetReacquisitionTimer >= 1f) {
			targetReacquisitionTimer = 0f;
			target = GetClosestTarget (villagers);
			if (target == null) {
				// Nothing else to target, ideally this would actually be something helpful to when there is nothing left to attack, but for now this will do.
				anim.SetBool ("idle", true);
				nav.Stop ();
				this.enabled = false;
				return;
			} 
			targetHealth = target.GetComponent<VillagerHealth> ();
		}
		if (attackTimer >= timeBetweenAttacks && targetInRange) {
			Attack ();
		}

		if (isAttacking) {
			// Monsters stop moving while they attack.
			attackAnimationTimer += Time.deltaTime;
			if (attackAnimationTimer >= timeBetweenAttacks - 1.8) { // a little room for the animation to finish before we start attacking again.
				if (targetInRange) {
					if (targetHealth.currentHealth > 0) {
						targetHealth.TakeDamage (1);
					}
					if (targetHealth.currentHealth <= 0) {
						targetInRange = false;
					}
				}
				isAttacking = false;
			}
		} else {
			nav.SetDestination (target.transform.position);
		}
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject == target){
			targetInRange = true;
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject == target){
			targetInRange = false;
		}
	}

	GameObject GetClosestTarget(GameObject[] targets){
		GameObject closest = null;
		float minDist = Mathf.Infinity;
		Vector3 pos = transform.position;
		foreach (GameObject t in targets) {
			if (t != null) {
				VillagerHealth health = t.GetComponent<VillagerHealth> ();
				if (health != null && health.currentHealth > 0) {
					float dist = Vector3.SqrMagnitude (t.transform.position - pos);

					if (dist < minDist) {
						closest = t;
						minDist = dist;
					}
				}
			}
		}
		// Update targetInRange if target is already is in range, to get around the fact that on trigger enter won't fire if we're already touching our next target.
		if (closest != null) {
			float dist = Vector3.SqrMagnitude (closest.transform.position - pos);
			if (dist < 2) {
				targetInRange = true;
			}
		}
		return closest;
	}

	void Attack(){
		isAttacking = true;
		nav.SetDestination (transform.position);
		attackAnimationTimer = 0f;
		attackTimer = 0f;
		anim.SetTrigger ("attack");
		audioSource.clip = attackSound;
		audioSource.volume = 0.3f + Random.Range (-0.1f, 0.1f);
		audioSource.pitch = 1f + Random.Range (-0.2f, 0.2f);
		audioSource.Play ();
	}
}

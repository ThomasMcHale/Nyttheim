using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class VillagerHealth : MonoBehaviour {

	public int maxHealth = 2;
	public int currentHealth;
	public AudioClip villagerHurt;
	public AudioClip villagerDeath;
	public Image healthCircle;

	bool isDead;

	private Color fullHealth = new Color(94,255,45);
	private Color criticalHealth = new Color(255,0,0);
	private AudioSource aud;
	private Rigidbody rb;
	private	WorkerHandler vilWorker;
	private CharacterMovement vilMovement;
	private NavMeshAgent nav;

	// Use this for initialization
	void Start () {
		aud = GetComponent<AudioSource> ();
		rb = GetComponent<Rigidbody> ();
		nav = GetComponent<NavMeshAgent> ();
		vilWorker = GetComponent<WorkerHandler> ();
		vilMovement = GetComponent<CharacterMovement> ();
		currentHealth = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		if (isDead) {
			transform.Translate (new Vector3 (0f, -0.5f * Time.deltaTime, 0f));
			if (transform.position.y < -10) {
				Object.Destroy (this.gameObject);
			}
		}
	}

	public void TakeDamage(int amount)
	{

		currentHealth -= amount;
		if (currentHealth <= 0 && !isDead) {
			Death ();
		} else if (!isDead) {
			aud.clip = villagerHurt;
			aud.volume = 1f + Random.Range (-0.1f, 0.1f);
			aud.pitch = 1f + Random.Range (-0.2f, 0.2f);
			aud.Play ();
			if (currentHealth == 2) {
				healthCircle.color = fullHealth;
			} else if (currentHealth == 1) {
				healthCircle.color = criticalHealth;
			}
		}

	}

	public void Death() {
		isDead = true;
		healthCircle.canvas.enabled = false;
		aud.clip = villagerDeath;
		aud.volume = 1f + Random.Range (-0.1f, 0.1f);
		aud.pitch = 1f + Random.Range (-0.2f, 0.2f);
		aud.Play ();
		rb.isKinematic = false;
		rb.detectCollisions = false;
		nav.enabled = false;

		vilWorker.enabled = false;
		vilMovement.enabled = false;
	}
}

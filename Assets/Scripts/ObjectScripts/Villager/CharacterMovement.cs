using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterMovement : MonoBehaviour {

	public Vector3 destination;
	public bool commandedRecently;

	private NavMeshAgent nav;
	private Animator anim;
	private NavMeshPath navMeshPath;
	private WorkerHandler worker;


	void Awake()
	{
		worker = GetComponent<WorkerHandler> ();
		nav = GetComponent<NavMeshAgent> ();
		commandedRecently = false;
		anim = GetComponent<Animator> ();
	}

	void Update()
	{
		if (commandedRecently) {
			// Calculate path and if a full path to the destination can be found, set destination and march away.
			navMeshPath = new NavMeshPath ();
			nav.CalculatePath(destination, navMeshPath);
			if (navMeshPath.status == NavMeshPathStatus.PathComplete) {
				nav.SetPath (navMeshPath);
			}
			commandedRecently = false; // We've handled the recent command, so wait for future notifications.
		}
	
		nav.updateRotation = nav.hasPath; // Stop the navAgent from rotating randomly.
		anim.SetBool ("IsWalking", nav.hasPath);
	
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitController : MonoBehaviour {

	bool collisionHandled = false; // Used to prevent multiple onEnter triggers from firing.

	void OnTriggerEnter(Collider other){
		if (collisionHandled)
			return;
		WorkerHandler worker = other.transform.GetComponent<WorkerHandler> ();
		if (worker != null){
			worker.atExit += 1;
		}
		collisionHandled = true;
	}

	void OnTriggerExit(Collider other){
		if (collisionHandled)
			return;
		WorkerHandler worker = other.transform.GetComponent<WorkerHandler> ();
		if (worker != null){
			worker.atExit -= 1;
		}
		collisionHandled = true;
	}

	// Update is called once per frame
	void Update () {
		collisionHandled = false;	
	}
}

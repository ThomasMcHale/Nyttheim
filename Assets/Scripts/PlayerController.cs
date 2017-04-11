using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	// Camera Scroll variables
	public float speed = 5f; // speed of scroll
	public float scrollBoundaryWidth = 15f; // Distance the mouse must be from an edge to scroll the camera
	private Vector2 mousePos;
	private Vector3 movement;
	private bool mouseScrollEnabled = false; // Disable scrolling with the mouse.

	// Mouse Click Variables
	private int terrainMask;
	private int charactersMask;
	private int resourcesMask;
	private float camRayLength = 400f;
	private List<GameObject> selectedCharacters;
	private bool commandIssuedRecently = false;

	// Key bindings
	private static KeyCode KEY_SELECT_MULTIPLE = KeyCode.LeftShift;

	void Awake(){
		terrainMask = LayerMask.GetMask ("Terrain");
		charactersMask = LayerMask.GetMask ("PlayerCharacters");
		resourcesMask = LayerMask.GetMask ("Resources");
		selectedCharacters = new List<GameObject>();
	}

	void Update()
	{
		mousePos = Input.mousePosition;
		bool lmb = Input.GetMouseButton (0);
		bool rmb = Input.GetMouseButton (1);
		float h = Input.GetAxis ("Horizontal");
		float v = Input.GetAxis ("Vertical");

		HandleClick (lmb, rmb);
		MoveCamera (h, v);
	}

	void HandleClick(bool lmb, bool rmb){
		if (rmb == true) {
			RaycastHit worldHit;
			Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);

			if (Physics.Raycast (camRay, out worldHit, camRayLength, resourcesMask)) {
				// First check if we clicked on a resource. If we did, command all selected workers to set that as their target.
				for (int i = 0; i < selectedCharacters.Count; i++) {
					WorkerHandler worker = selectedCharacters[i].GetComponent<WorkerHandler> ();
					if (worker != null) {
						worker.target = worldHit.transform.gameObject;
					}
				}

			} else if (Physics.Raycast (camRay, out worldHit, camRayLength, terrainMask)) {
				// If we didn't click on a resource, but if we clicked on the world, then tell our selected units to travel to that location, spreading out in formation.
				for (int i = 0; i < selectedCharacters.Count; i++){
					GameObject obj = selectedCharacters [i];
					CharacterMovement charController = obj.GetComponent<CharacterMovement> ();
					if (charController != null) {
						Vector3 dest = worldHit.point;
						if (i > 0) {
							dest = dest + new Vector3 (Mathf.Cos (2*Mathf.PI * (float) i / (float) (selectedCharacters.Count - 1)) * 1.2f, 0f, Mathf.Sin (2*Mathf.PI * (float) i / (float) (selectedCharacters.Count - 1)) * 1.2f);
						}
						charController.destination = dest;
						WorkerHandler worker = obj.GetComponent<WorkerHandler> ();
						if (worker!= null){
							worker.target = null; // clear worker targets.
						}
						charController.commandedRecently = true;
						//commandIssuedRecently = true;
					} else {
						Debug.Log ("Unmoveable character selected! " + obj.ToString ());
					}
				}
			} else {
				Debug.Log ("Invalid location clicked.");
			}
		} else if (lmb == true) {
			// Check if LMB clicked on character or not
			RaycastHit characterHit;
			Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (camRay, out characterHit, camRayLength, charactersMask)) {
				if (!Input.GetKey (KEY_SELECT_MULTIPLE)) {
					selectedCharacters.Clear ();
				}
				if (!selectedCharacters.Contains (characterHit.transform.gameObject)) {
					selectedCharacters.Add (characterHit.transform.gameObject);
				}
			} else {
				selectedCharacters.Clear ();
			}
		} /*else if (commandIssuedRecently) {
			// Entirely because NavAgents are being finicky.
			commandIssuedRecently = false;
			foreach (GameObject obj in selectedCharacters) {
				CharacterMovement charController = obj.GetComponent<CharacterMovement> ();
				if (charController != null) {
					charController.commandedRecently = false;
				} else {
					Debug.Log ("Unmoveable character selected! " + obj.ToString ());
				}
			}
		}*/
			

	}

	void MoveCamera (float h, float v){
		float xMove = 0;
		float yMove = 0;

		// Handle movement in the horizontal
		if ((mouseScrollEnabled && mousePos.x < scrollBoundaryWidth) || h < 0){
			xMove = -1;
		} else if ((mouseScrollEnabled && mousePos.x >= Screen.width - scrollBoundaryWidth) || h > 0){
			xMove = 1;
		} else {
			xMove = 0;
		}

		// Handle movement in the vertical
		if ((mouseScrollEnabled && mousePos.y < scrollBoundaryWidth) || v > 0) {
			yMove = 1;
		} else if ((mouseScrollEnabled && mousePos.y > Screen.height - scrollBoundaryWidth) || v < 0) {
			yMove = -1;
		} else {
			yMove = 0;
		}

		if (xMove != 0 || yMove != 0) {
			Vector3 movement = new Vector3(v + h, 0f, v - h) * speed * Time.deltaTime;
			transform.Translate (movement, Space.World);
		}
	}
}

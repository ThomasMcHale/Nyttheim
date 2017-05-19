using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public int villagerCount;

	public Light lighting;
	public Color dayColor;
	public Color nightColor;
	public GameObject victoryOverlay;
	public GameObject loseOverlay;
	public Text resultsText;

	// options
	private int hourLength = 4; // How long between "hour" shifts
	//private int startAngle = 80; // angle the lighting starts at
	//private int hours = 10; // number of time shifts to go through
	private int angleChange = -4; // lighting angle change per 'hour'.
	private int hours = 10;
	private float monsterSpawnFrequency = 6f; // How often monsters spawn at night.

	private float gameTime = 0f;
	private float currentTime = 0f;
	private float monsterSpawnTimer = 0f;
	private int currentHour = 0;
	private bool night = false;
	private float maxShadowStrength;
	private float currentShadowStrength = 0f;
	private GameObject[] spawners;
	private GameObject characters;
	private List<GameObject> villagers;
	private List<GameObject> monsters;
	private bool gameOver = false;


	// Use this for initialization
	void Start () {
		spawners = GameObject.FindGameObjectsWithTag ("Spawn");
		characters = GameObject.FindGameObjectWithTag ("Characters");
		maxShadowStrength = 255-lighting.color.b;
		villagers = new List<GameObject> ();
		monsters = new List<GameObject> ();

	
		SpawnVillagers ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) { // Close game if ESC key is hit
			Application.Quit ();
		}
		if (!gameOver) {
			// Advance time
			currentTime += Time.deltaTime;
			gameTime += Time.deltaTime;
			AdvanceLighting ();
			if (currentTime > hourLength) {
				AdvanceHour ();
				currentTime = 0f;
			}
			// If night, spawn monsters
			if (night) {
				monsterSpawnTimer -= Time.deltaTime;
				if (monsterSpawnTimer < 0) { 
					SpawnSkeleton ();
					monsterSpawnTimer = monsterSpawnFrequency + Random.Range (-2f, 2f); 
				}
			}
			// Check if villagers are dead.
			CheckLoss ();
		} else {
			if (Input.GetKeyDown (KeyCode.R)) {
				SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
			}
		}
	}
		
	// Time functions
	private void AdvanceLighting(){
		if (currentHour < hours){
			lighting.transform.Rotate ((angleChange * Time.deltaTime)/hourLength, 0, 0);
			lighting.intensity = lighting.intensity - ((0.5f / hours * Time.deltaTime) / hourLength);
		} else if (!night){
			if (currentShadowStrength < maxShadowStrength) {
				lighting.transform.Rotate ((angleChange * Time.deltaTime)/hourLength, 0, 0);
				lighting.color = Color.Lerp (dayColor, nightColor, (currentShadowStrength / maxShadowStrength));
				lighting.shadowStrength = 1 - (currentShadowStrength / maxShadowStrength);
				currentShadowStrength++;
			} else {
				night = true;
			}
			// Night fall animation.
		}
	}

	private void AdvanceHour(){
		currentHour++;
	}

	// Spawn Functions
	private void SpawnVillagers(){
		for (int i = 0; i < villagerCount; i++) {
			GameObject villager = Instantiate(Resources.Load("Prefabs/Characters/Villagers/Villager"),
											spawners [0].transform.position + new Vector3(Random.Range(-1f,1f), 0, Random.Range(-1f,1f)),
											Quaternion.identity,
										   	characters.transform) as GameObject;
			villagers.Add(villager);
		}
	}

	private void SpawnSkeleton(){
		GameObject monster = Instantiate(Resources.Load("Prefabs/Characters/Monsters/Skeleton"),
										spawners [Random.Range (0, spawners.Length)].transform.position + new Vector3(Random.Range(-1f,1f), 0, Random.Range(-1f,1f)),
										Quaternion.identity,
										characters.transform) as GameObject;
		monsters.Add (monster);
	}

	// Check if all villagers have died or all villagers are in the exit.
	private void CheckLoss(){
		bool lost = true;
		bool won = true;
		int livingCount = 0;
		foreach (GameObject v in villagers) {
			if (v != null) {
				lost = false;
				livingCount++;
				WorkerHandler w = v.GetComponent<WorkerHandler> ();
				if (w != null && w.atExit <= 0) {
					won = false;
				}
			}
		}
		if (lost) {
			loseOverlay.SetActive (true);
		} else if (won) {
			victoryOverlay.SetActive (true);
			resultsText.text = "You finished in "+(int) gameTime+" seconds with "+livingCount+" villagers surviving!";
		}
		if (lost || won) {
			foreach (GameObject m in monsters) {
				MonsterMovement controller = m.GetComponent<MonsterMovement> ();
				if (controller != null) {
					controller.enabled = false;
				}
				Animator anim = m.GetComponent<Animator> ();
				if (anim != null) {
					anim.SetBool ("idle", true);
				}
				NavMeshAgent nav = m.GetComponent<NavMeshAgent> ();
				if (nav != null) {
					nav.Stop ();
				}
			}

			gameOver = true;
		}
	}
}

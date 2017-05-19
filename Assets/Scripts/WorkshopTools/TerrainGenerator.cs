using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {

	private static int TILE_SIZE = 3;
	private static float ELEVATION_STEP = 0f; // = 2.5f;

	private static string[] terMap = {
		"pppppppppppprppp",
		"pppppppppppprppp",
		"pppppppppppprrpp",
		"pppppppppppprppp",
		"rrrrpppppppprppp",
		"ppprpppppprrrppp",
		"ppprpppppprppppp",
		"ppprrrrppppppppp",
		"pppppprppppppppp",
		"pppppprrrppppppp",
		"pppppprprrpppppp",
		"ppppprrpprrrpppp",
		"ppppprppppprrbrr",
		"pppppbpppppppppp",
		"ppppprpppppppppp",
		"ppppprpppppppppp",
		"ppppprpppppppppp",
		"ppppprpppppppppp",
		"ppppprpppppppppp",
		"ppppprpppprrrrrr",
		"ppppprpppprppppp",
		"rrrrrrppprrppppp",
		"ppppppppprpppppp",
		"ppppppppprpppppp",
		"pppppppprrrrrrrr",
		"ppppppprrppppppp",
		"pppppprrpppppppp",
		"rrrrrrrppppppppp",
		"pppppppppppppppp",
		"pppppppppppppppp",
	};

	private static string[] elevMap = {
		"0000000222221111",
		"0000000011211110",
		"0000000000111000",
		"0000000000000000",
		"0000000000000000",
		"0000000000000000",
		"0000000000001101",
		"0000000000001000",
		"0000000000000000",
		"0000000000000000",
		"0000000000000000",
		"0000000000000000",
		"0001000000000000",
		"0000000000000000",
		"0000000000000000",
		"0000000000000000",
		"0000000000000000",
		"0000000000000000",
		"0000000000000000",
		"0000000000000000",
		"0000000000000000",
		"0000000000000000",
		"0000000000000000",
		"0000000000000000",
		"0000000000000000",
		"0000000000000000",
		"0000000000000000",
		"0000000000000000",
		"0000000000000000",
		"0000000000000000",
	};

	private static string[] resMap = {
		"--tt----------t-",
		"--tst-E--t-E-s--",
		"ts--t----ttt----",
		"-t---s----t---t-",
		"------t------tt-",
		"------tt--------",
		"----t-----tt----",
		"--t---------t---",
		"-ttt-----s----t-",
		"---t---------tt-",
		"---t---------ttt",
		"-s-----t------st",
		"---t------------",
		"-------st-------",
		"tts----t----t--t",
		"-t-------ttt--s-",
		"t--st-tt---t----",
		"ttttt-ttttt--tt-",
		"ttttt-ttttt-sttt",
		"t-ttt-tttt------",
		"t-st--tttt-tstst",
		"------ttt---tttt",
		"-----ssts-tttt--",
		"-stt-tttt-ttttst",
		"--t----t--------",
		"---t-ts---------",
		"-s-tt----tt-----",
		"---------t-t--t-",
		"-ts---t-s-s-XXt-",
		"-t--t---t--t-tt-",
	};

	private string[] plainsDoodads = {
		"Prefabs/World/Terrain/Plains/Doodads/BigBlueFlower",
		"Prefabs/World/Terrain/Plains/Doodads/BigGrassTuft",
		"Prefabs/World/Terrain/Plains/Doodads/BigRedFlower",
		"Prefabs/World/Terrain/Plains/Doodads/BigYellowFlower",
		"Prefabs/World/Terrain/Plains/Doodads/BrownMushroom",
		"Prefabs/World/Terrain/Plains/Doodads/GrassTuft",
		"Prefabs/World/Terrain/Plains/Doodads/RedMushroom",
		"Prefabs/World/Terrain/Plains/Doodads/Shrub",
		"Prefabs/World/Terrain/Plains/Doodads/SmallBlueFlower",
		"Prefabs/World/Terrain/Plains/Doodads/SmallGrassTuft",
		"Prefabs/World/Terrain/Plains/Doodads/SmallRedFlower",
		"Prefabs/World/Terrain/Plains/Doodads/SmallYellowFlower",
		"Prefabs/World/Terrain/Plains/Doodads/SpikeyGrass",
		"Prefabs/World/Terrain/Plains/Doodads/StarGrass"
	};

	private string[,] terrainMap;
	private string[,] elevationStrMap;
	private float[,] elevationMap;
	private string[,] resourceMap;


    private GameObject world; // Reference to map
    private GameObject terrain;
    private GameObject resources;
    private GameObject characters;
	private GameObject doodads;

	private int terrainMask;
	private int resourcesMask;
	private int riverBuildableMask;

	void Awake(){
		terrainMask = LayerMask.NameToLayer ("Terrain");
		resourcesMask = LayerMask.NameToLayer ("Resources");
		riverBuildableMask = LayerMask.NameToLayer ("RiverBuildable");
	}


	void Start () {
		// Initialize Variables
		world = transform.Find("World").gameObject;
		terrain = world.transform.Find("Terrain").gameObject;
		resources = world.transform.Find("Resources").gameObject;
		characters = world.transform.Find("Characters").gameObject;
		doodads = world.transform.Find ("Doodads").gameObject;

		// Break maps into 2D array.
		terrainMap = extract2DArray(terMap);
		elevationStrMap = extract2DArray (elevMap);
		elevationMap = convert2DStringToFloat(elevationStrMap);


		resourceMap = extract2DArray (resMap);

		renderMap ();
	}



	private void renderMap(){
		renderTerrain ();
		//renderCliffs (); // Todo resolve interal corner Z-Fighting cliff problems...
		renderResources ();
	}

	// === TERRAIN RENDERER ===
	private void renderTerrain(){
		// Generate map
		for (int x = 0; x < terrainMap.GetLength (0); x++) {
			for (int y = 0; y < terrainMap.GetLength (1); y++) {
				switch (terrainMap [x, y]) {
				case "p":
					placePlains (x, y);
					break;
				case "r":
					placeRiver (x,y);
					break;
				case "b":
					placeBridge (x, y);
					break;
				default:
					Debug.Log ("ERROR! Unkown Tile: " + terrainMap [x, y]);
					break;
				}
			}
		}
	}

	// Place a plains tile
	private void placePlains(int x, int y){
		GameObject tile = (GameObject)Instantiate (Resources.Load ("Prefabs/World/Terrain/Plains/Full"));
		tile.name = "PlainsFull";
		tile.transform.SetParent (terrain.transform);
		tile.transform.position = new Vector3 (x * TILE_SIZE, elevationMap[x,y] * ELEVATION_STEP, y * TILE_SIZE);
		tile.layer = terrainMask;
	}

	private void placeBridge(int x, int y){
		string up;
		string down;
		string left;
		string right;
		GameObject tile;
		Quaternion rotation = Quaternion.Euler(0,0,0);

		// Determine adjacent tiles
		up = (x > 0) ? terrainMap[x-1,y] : "r";
		down = x < terrainMap.GetLength(0)-1 ? terrainMap[x+1,y] : "r";
		left = y > 0 ? terrainMap[x,y-1] : "r";
		right = y < terrainMap.GetLength(1)-1 ? terrainMap[x,y+1] : "r";

		// count up rivers
		int riverCount = 0;
		riverCount = (up == "r") ? riverCount + 1 : riverCount;
		riverCount = (down == "r") ? riverCount + 1 : riverCount;
		riverCount = (left == "r") ? riverCount + 1 : riverCount;
		riverCount = (right == "r") ? riverCount + 1 : riverCount;
		if (riverCount == 2 && (up == "r" && down == "r") || (left == "r" && right == "r")) {
			tile = (GameObject)Instantiate (Resources.Load ("Prefabs/World/Terrain/River/DirtBridge"));
			if (up == "r" && down == "r") {
				rotation = Quaternion.Euler (0, 90, 0);
			}
		} else {
			Debug.Log ("ERROR! Invalid bridge location.");
			return;
		}
		tile.transform.SetParent (terrain.transform);
		tile.transform.position = new Vector3 (x * TILE_SIZE, elevationMap[x,y] * ELEVATION_STEP, y * TILE_SIZE);
		tile.transform.rotation = rotation;
		tile.layer = terrainMask;
	}

	// Place a river tile based on surrounding tiles.
	// Passing the terrainMap by reference to avoid copying the array unnecessarily. 
	private void placeRiver (int x, int y){
		string up;
		string down;
		string left;
		string right;
		GameObject tile;
		Quaternion rotation = Quaternion.Euler(0,0,0);
		string[] riverTypes = { "r", "b" }; 

		// Determine adjacent tiles
		up = (x > 0) ? terrainMap[x-1,y] : "r";
		down = x < terrainMap.GetLength(0)-1 ? terrainMap[x+1,y] : "r";
		left = y > 0 ? terrainMap[x,y-1] : "r";
		right = y < terrainMap.GetLength(1)-1 ? terrainMap[x,y+1] : "r";

		// count up rivers
		int riverCount = 0;
		riverCount = Contains(riverTypes,up) ? riverCount + 1 : riverCount;
		riverCount = Contains(riverTypes,down) ? riverCount + 1 : riverCount;
		riverCount = Contains(riverTypes,left) ? riverCount + 1 : riverCount;
		riverCount = Contains(riverTypes,right) ? riverCount + 1 : riverCount;

		int nextTileMask = terrainMask;

		if (riverCount == 4) {
			tile = (GameObject)Instantiate (Resources.Load ("Prefabs/World/Terrain/River/DirtQuad"));
		} else if (riverCount == 3) {
			tile = (GameObject)Instantiate (Resources.Load ("Prefabs/World/Terrain/River/DirtTri"));
			if (! Contains(riverTypes,right)) {
				rotation = Quaternion.Euler (0, 90, 0);
			} else if (!Contains(riverTypes,down)) {
				rotation = Quaternion.Euler (0, 180, 0);
			} else if (!Contains(riverTypes,left)) {
				rotation = Quaternion.Euler (0, 270, 0);
			}
		} else if (riverCount == 2) {
			if ((Contains(riverTypes,up) && Contains(riverTypes,down) || Contains(riverTypes,left) && Contains(riverTypes,right))) {
				tile = (GameObject)Instantiate (Resources.Load ("Prefabs/World/Terrain/River/DirtDuo"));
				if (Contains(riverTypes,up) && Contains(riverTypes,down)) {
					rotation = Quaternion.Euler (0, 90, 0);
				}
				nextTileMask = riverBuildableMask;
			} else {
				tile = (GameObject)Instantiate (Resources.Load ("Prefabs/World/Terrain/River/DirtTurn"));
				if (Contains(riverTypes,left) && Contains(riverTypes,up)) {
					rotation = Quaternion.Euler (0, 90, 0);
				} else if (Contains(riverTypes,up) && Contains(riverTypes,right)) {
					rotation = Quaternion.Euler (0, 180, 0);
				} else if (Contains(riverTypes,right) && Contains(riverTypes,down)) {
					rotation = Quaternion.Euler (0, 270, 0);
				}
			}
		} else if (riverCount == 1) {
			tile = (GameObject)Instantiate (Resources.Load ("Prefabs/World/Terrain/River/DirtEnd"));
			if (Contains(riverTypes,down)) {
				rotation = Quaternion.Euler (0, 90, 0);
			} else if (Contains(riverTypes,left)) {
				rotation = Quaternion.Euler (0, 180, 0);
			} else if (Contains(riverTypes,up)) {
				rotation = Quaternion.Euler (0, 270, 0);
			}
		} else {
			Debug.Log ("ERROR isolated river tile");
			return;
		}
		tile.transform.SetParent (terrain.transform);
		tile.transform.position = new Vector3 (x * TILE_SIZE, elevationMap[x,y] * ELEVATION_STEP, y * TILE_SIZE);
		tile.transform.rotation = rotation;
		tile.layer = nextTileMask;
	}

	// === CLIFF RENDERER ===

	private void renderCliffs(){
		// Generate map
		for (int x = 0; x < elevationMap.GetLength (0); x++) {
			for (int y = 0; y < elevationMap.GetLength (1); y++) {
				float[,] neighbors = convert2DStringToFloat(getAdjacentTiles (elevationStrMap, x, y));


				for (int i = 0; i < 3; i++) {
					for (int j = 0; j < 3; j++) {
						// Check elevation comparison to figure out if we need to handle the cliff tile.
						if (neighbors [i, j] != null) {
							float difference = neighbors[i,j] - elevationMap[x,y];
							if (difference > 0) {
								// We need to build some cliffs in.

								if ((i == j && i != 1) || (i == 0 && j == 2) || (i == 2 && j == 0) ) {


									//Debug.Log("Neighbors: "+neighbors+  " COMPARED TO: "+ neighbors[i,j]);
									// Check if this "corner" neighbor is actually a corner
									bool topLeftCorner = (i == 0 && j == 0 && neighbors[i+1,j] != neighbors[i,j] && neighbors[i,j+1] != neighbors[i,j]);
									bool botLeftCorner = (i == 0 && j == 2 && neighbors[i+1,j] != neighbors[i,j] && neighbors[i,j-1] != neighbors[i,j]);
									bool topRightCorner = (i == 2 && j == 0 && neighbors[i-1,j] != neighbors[i,j] && neighbors[i,j+1] != neighbors[i,j]);
									bool botRightCorner = (i == 2 && j == 2 && neighbors[i-1,j] != neighbors[i,j] && neighbors[i,j-1] != neighbors[i,j]);


									if (topLeftCorner || botLeftCorner || topRightCorner || botRightCorner){ // HOW TO CHECK TRUTHFUL CORNERS
										Debug.Log ("Position (" + x + "," + y + ") has TLC: " + topLeftCorner + ", BLC: " + botLeftCorner + ", TRC: " + topRightCorner + " BRC: " + botRightCorner);

										for (int stack = 0; stack < Mathf.RoundToInt(difference); stack++){ // sometimes we need double (or more) cliffs)
											GameObject cliff = Instantiate (Resources.Load("Prefabs/World/Terrain/Cliff/Corner")) as GameObject;
											cliff.transform.SetParent (terrain.transform);
											cliff.transform.position = new Vector3 (x * TILE_SIZE + (1.5f * (i-1)), 0.3f + ((elevationMap [x, y] + stack) * ELEVATION_STEP), y * TILE_SIZE + (1.5f * (j - 1)));
											if (topLeftCorner) {
												cliff.transform.Rotate (0, 270, 0);
											} else if (topRightCorner) {
												cliff.transform.Rotate (0, 180, 0);
											} else if (botRightCorner) {
												cliff.transform.Rotate (0, 90, 0);
											}
											cliff.layer = terrainMask;
										}
										// Top corner with edge
										int stackHeight = Mathf.RoundToInt(difference);
										GameObject edge = Instantiate (Resources.Load("Prefabs/World/Terrain/Cliff/CornerEdge")) as GameObject;
										edge.transform.SetParent (terrain.transform);
										edge.transform.position = new Vector3 (x * TILE_SIZE + (1.5f * (i-1)), 0.0015f + ((elevationMap [x, y] + stackHeight) * ELEVATION_STEP), y * TILE_SIZE  + (1.5f * (j-1)));
										if (topLeftCorner) {
											edge.transform.Rotate (0, 270, 0);
										} else if (topRightCorner) {
											edge.transform.Rotate (0, 180, 0);
										} else if (botRightCorner) {
											edge.transform.Rotate (0, 90, 0);
										}
										edge.layer = terrainMask;
									}
								} else {
									// Side
									bool leftWall = (i == 0);
									bool rightWall = (i == 2);
									bool topWall = (j == 0);
									bool botWall = (j == 2);
									for (int stack = 0; stack < Mathf.RoundToInt(difference); stack++){ // sometimes we need double (or more) cliffs)
										GameObject cliff = Instantiate (Resources.Load("Prefabs/World/Terrain/Cliff/Straight")) as GameObject;
										cliff.transform.SetParent (terrain.transform);
										cliff.transform.position = new Vector3 (x * TILE_SIZE + (i-1), 0.3f + ((elevationMap [x, y] + stack) * ELEVATION_STEP), y * TILE_SIZE  + (j-1));
										if (botWall) {
											cliff.transform.Rotate (0, 90, 0);
										} else if (topWall) {
											cliff.transform.Rotate (0, 270, 0);
										} else if (rightWall) {
											cliff.transform.Rotate (0, 180, 0);
										}
										cliff.layer = terrainMask;
									}
									// Top cliff with edge
									int stackHeight = Mathf.RoundToInt(difference);
									GameObject edge = Instantiate (Resources.Load("Prefabs/World/Terrain/Cliff/StraightEdge")) as GameObject;
									edge.transform.SetParent (terrain.transform);
									edge.transform.position = new Vector3 (x * TILE_SIZE + (i-1),  0.0015f + ((elevationMap [x, y] + stackHeight) * ELEVATION_STEP), y * TILE_SIZE  + (j-1));
									if (botWall) {
										edge.transform.Rotate (0, 90, 0);
									} else if (topWall) {
										edge.transform.Rotate (0, 270, 0);
									} else if (rightWall) {
										edge.transform.Rotate (0, 180, 0);
									}
									edge.layer = terrainMask;
								}
							}
						}
					}
				}
				/*
				switch (terrainMap [x, y]) {
				case "r":
					break;
				case "b":
					break;
				default:
					break;
				}*/
			}
		}
	}

	// === RESOURCE RENDERER ===
	private void renderResources(){
		// Place Resources
		for (int x = 0; x < resourceMap.GetLength (0); x++) {
			for (int y = 0; y < resourceMap.GetLength (1); y++) {
				switch (resourceMap [x, y]) {
				case "t":
					placeTree (x, y);
					break;
				case "s":
					placeStones (x, y);
					break;
				case "E":
					placeSpawn (x, y);
					break;
				case "X":
					placeExit (x, y);
					break;
				case "-":
					placeDoodad (x, y);
					break;
				default:
					Debug.Log ("ERROR! Unkown resource: " + resourceMap [x, y]);
					break;
				}
			}
		}
	}

	private void placeTree(int x, int y){
		GameObject res = (GameObject)Instantiate (Resources.Load ("Prefabs/World/Resources/Trees/Pine"));
		res.transform.SetParent (resources.transform);
		res.transform.position = new Vector3 (x * TILE_SIZE, elevationMap[x,y] * ELEVATION_STEP, y * TILE_SIZE) + new Vector3(Random.Range(-1f,1f), 0, Random.Range(-1f,1f)) ;
		res.transform.rotation = Quaternion.Euler (0, Random.Range (0, 359), 0);
		res.layer = resourcesMask;
	}

	private void placeStones(int x, int y){
		GameObject res = (GameObject)Instantiate (Resources.Load ("Prefabs/World/Resources/StoneMine"));
		res.transform.SetParent (resources.transform);
		res.transform.position = new Vector3 (x * TILE_SIZE, elevationMap[x,y] * ELEVATION_STEP, y * TILE_SIZE);
		res.transform.rotation = Quaternion.Euler (0, Random.Range (0, 359), 0);
		res.layer = resourcesMask;
	}

	private void placeSpawn(int x, int y){
		GameObject spawn = Instantiate(Resources.Load("Prefabs/World/Resources/Spawn"),
			new Vector3 (x * TILE_SIZE, 1.8f + elevationMap[x,y] * ELEVATION_STEP, y * TILE_SIZE),
			Quaternion.identity,
			resources.transform) as GameObject;
		spawn.layer = resourcesMask;
	}

	private void placeExit(int x, int y){
		GameObject exit = Instantiate(Resources.Load("Prefabs/World/Resources/Exit"),
			new Vector3 (x * TILE_SIZE, 1.8f + elevationMap[x,y] * ELEVATION_STEP, y * TILE_SIZE),
			Quaternion.identity,
			resources.transform) as GameObject;
		exit.layer = resourcesMask;
	}

	private void placeDoodad(int x, int y){
		if (Random.Range (0, 3) != 0) {
			if (terrainMap [x, y] == "p") {
				string doodad = plainsDoodads [Random.Range (0, plainsDoodads.Length)]; 
				Instantiate (Resources.Load (doodad),
					new Vector3 (x * TILE_SIZE + Random.Range (-1.4f, 1.4f), 0.3f + elevationMap [x, y] * ELEVATION_STEP, y * TILE_SIZE + Random.Range (-1.4f, 1.4f)),
					Quaternion.Euler (0, Random.Range (0f, 360f), 0),
					doodads.transform);
			}
		}
	}

	// === UTILITY ===

	/*
	 * extract2DArray takes an array of strings and splits each string in the original array into individual characters. It then returns a 2D array of those characters.
	*/
	private string[,] extract2DArray(string[] arr){
		string[,] retArr;
		retArr = new string[arr.Length, arr [0].Length];
		for (int i = 0; i < arr.Length; i++) {
			char[] row = arr [i].ToCharArray ();//Split (new string[] {''});
			for (int j = 0; j < row.Length; j++) {
				retArr [i, j] = row [j].ToString ();
			}
		}
		return retArr;
	}

	private float[,] convert2DStringToFloat(string[,] arr){
		float[,] retArr;
		retArr = new float[arr.GetLength(0), arr.GetLength(1)];
		for (int i = 0; i < arr.GetLength(0); i++) {
			for (int j = 0; j < arr.GetLength(1); j++){
				float.TryParse(arr[i,j],out retArr [i, j]);
			}
		}
		return retArr;
	}

	// Checks whether a string array contains a value v.
	private bool Contains(string[] arr, string v){
		foreach (string str in arr) {
			if (str == v) {
				return true;
			}
		}
		return false;
	}

	// Returns a matrix of the 9 tiles surrounding an x/y position (including the main one), null if out of bounds
	private string[,] getAdjacentTiles(string[,] baseArr,int x, int y){
		string[,] retArr = new string[3, 3];
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 3; j++) {
				int xVal = x + (i - 1);
				int yVal = y + (j - 1);
				if (xVal >= 0 && xVal < baseArr.GetLength (0) && yVal >= 0 && yVal < baseArr.GetLength (1)) {
					retArr [i, j] = baseArr [xVal, yVal];
				} else {
					retArr [i, j] = null;
				}
			}
		}
		return retArr;
	}
}

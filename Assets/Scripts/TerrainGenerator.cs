using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {

	private static int TILE_SIZE = 3;

	private static string[] terrainMap = {
		"pppppppppppppppp",
		"pppppppppppppppp",
		"pppppppppppppppp",
		"pppppppppppppppp",
		"ppprpppppppppppp",
		"ppprpppppppppppp",
		"ppprpppppppppppp",
		"ppprrrrppppppppp",
		"pppppprppppppppp",
		"pppppprrrppppppp",
		"pppppprprrpppppp",
		"ppppprrpprrrpppp",
		"ppppprppppprrbrr",
		"pppppbpppppppppp",
		"ppppprpppppppppp",
		"ppppprpppppppppp",
	};
	private static string[] resourceMap = {
		"--tt----------t-",
		"--tst----t---s--",
		"ts--t----ttt----",
		"-t---s----t---t-",
		"------t------tt-",
		"------tt--------",
		"----t-----tt----",
		"--t---------t---",
		"-ttt-----s----t-",
		"---t---------tt-",
		"---t---------ttt",
		"-s------------st",
		"---t------------",
		"-------st-------",
		"tts----t--------",
		"-t-------ttt--s-",
	};

    private GameObject world; // Reference to map
    private GameObject terrain;
    private GameObject resources;
    private GameObject characters;

	private int terrainMask;
	private int resourcesMask;

	void Awake(){
		terrainMask = LayerMask.NameToLayer ("Terrain");
		resourcesMask = LayerMask.NameToLayer ("Resources");
	}


	void Start () {
		// Initialize Variables
		world = transform.Find("World").gameObject;
		terrain = world.transform.Find("Terrain").gameObject;
		resources = world.transform.Find("Resources").gameObject;
		characters = world.transform.Find("Characters").gameObject;

		renderMap (terrainMap,resourceMap);
        
        // Add a basic character to the game
        GameObject character = (GameObject)Instantiate(Resources.Load("Prefabs/Characters/Villagers/Villager"));
        character.name = "Villager";
        character.transform.SetParent(characters.transform);
        character.transform.position = new Vector3(15, 3, 15);

	}



	private void renderMap(string[] terMap, string[] resMap){
		renderTerrain (terMap);
		renderResources (resMap);
	}

	// === TERRAIN RENDERER ===
	private void renderTerrain(string[] terMap){
		string[,] terrainMap;

		// Break terrain map into 2D array.
		terrainMap = extract2DArray(terMap);
	
		// Generate map
		for (int x = 0; x < terrainMap.GetLength (0); x++) {
			for (int y = 0; y < terrainMap.GetLength (1); y++) {
				switch (terrainMap [x, y]) {
				case "p":
					placePlains (x, y);
					break;
				case "r":
					placeRiver (x,y, ref terrainMap);
					break;
				case "b":
					placeBridge (x, y, ref terrainMap);
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
		tile.transform.position = new Vector3 (x * TILE_SIZE, 0, y * TILE_SIZE);
		tile.layer = terrainMask;
	}

	private void placeBridge(int x, int y, ref string[,] terrainMap){
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
		tile.transform.position = new Vector3 (x * TILE_SIZE, 0, y * TILE_SIZE);
		tile.transform.rotation = rotation;
		tile.layer = terrainMask;
	}

	// Place a river tile based on surrounding tiles.
	// Passing the terrainMap by reference to avoid copying the array unnecessarily. 
	private void placeRiver (int x, int y, ref string[,] terrainMap){
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
		tile.transform.position = new Vector3 (x * TILE_SIZE, 0, y * TILE_SIZE);
		tile.transform.rotation = rotation;
		tile.layer = terrainMask;
	}

	// === RESOURCE RENDERER ===
	private void renderResources(string[] resMap){
		string[,] resourceMap;

		// Break resource map into 2D array.
		resourceMap = extract2DArray(resMap);

		// Place Resources
		for (int x = 0; x < resourceMap.GetLength (0); x++) {
			for (int y = 0; y < resourceMap.GetLength (1); y++) {
				switch (resourceMap [x, y]) {
				case "t":
					placeTree (x, y);
					break;
				case "s":
					placeStones (x,y);
					break;
				case "-":
					break;
				default:
					Debug.Log ("ERROR! Unkown resource: " + resourceMap [x, y]);
					break;
				}
			}
		}
	}

	private void placeTree(int x, int y){
		GameObject res = (GameObject)Instantiate (Resources.Load ("Prefabs/World/Resources/Trees/Pine1Tree"));
		res.transform.SetParent (resources.transform);
		res.transform.position = new Vector3 (x * TILE_SIZE, 0, y * TILE_SIZE) + new Vector3(Random.Range(-1f,1f), 0, Random.Range(-1f,1f)) ;
		res.transform.rotation = Quaternion.Euler (0, Random.Range (0, 359), 0);
		res.layer = resourcesMask;
	}

	private void placeStones(int x, int y){
		GameObject res = (GameObject)Instantiate (Resources.Load ("Prefabs/World/Resources/StoneMine"));
		res.transform.SetParent (resources.transform);
		res.transform.position = new Vector3 (x * TILE_SIZE, 0, y * TILE_SIZE);
		res.transform.rotation = Quaternion.Euler (0, Random.Range (0, 359), 0);
		res.layer = resourcesMask;
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

	// Checks whether a string array contains a value v.
	private bool Contains(string[] arr, string v){
		foreach (string str in arr) {
			if (str == v) {
				return true;
			}
		}
		return false;
	}
}

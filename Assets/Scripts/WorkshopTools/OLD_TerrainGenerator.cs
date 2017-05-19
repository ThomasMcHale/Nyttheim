using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OLD_TerrainGenerator : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/*

		private static int TILE_SIZE = 3;

    public int worldSize = 10;

    private GameObject world; // Reference to map
    private GameObject terrain;
    private GameObject resources;
    private GameObject characters;
    private WorldMap map; // 2D array to store map data

	void Start () {
		// Initialize Variables
		world = transform.Find("World").gameObject;
		terrain = world.transform.Find("Terrain").gameObject;
		resources = world.transform.Find("Resources").gameObject;
		characters = world.transform.Find("Characters").gameObject;

		renderMap (terrainMap,resourceMap);



        // generate a world 
	map = generateBasicMap(worldSize,worldSize);

	// Build the world
	for (int x = 0; x < worldSize; x++)
	{
		for (int z = 0; z < worldSize; z++)
		{
			// Lay out tiles
			WorldObject mapData = map.terrain[x,z];
			GameObject tile = (GameObject) Instantiate(Resources.Load("Prefabs/World/"+mapData.prefab));
			tile.name = mapData.prefab;
			tile.transform.SetParent(terrain.transform);
			tile.transform.position = new Vector3(x * 3, 0, z * 3) + mapData.offset;
			tile.transform.rotation = mapData.rotation;

			// Lay out resources, if any
			WorldObject resData = map.resources[x, z];
			if (resData != null)
			{
				GameObject resource = (GameObject)Instantiate(Resources.Load("Prefabs/World/" + resData.prefab));
				resource.name = resData.prefab;
				resource.transform.SetParent(resources.transform);
				resource.transform.position = new Vector3(x * 3, 0, z * 3) + resData.offset;
				resource.transform.rotation = resData.rotation;
			}          
		}
	}

	// Add a basic character to the game
	GameObject character = (GameObject)Instantiate(Resources.Load("Prefabs/Characters/Villagers/Villager"));
	character.name = "Villager";
	character.transform.SetParent(characters.transform);
	character.transform.position = new Vector3(15, 3, 15);
	
}

// Function that generates a basic map with a river in it.
private WorldMap generateBasicMap(int sizeX, int sizeZ)
{
	map = new WorldMap(sizeX,sizeZ);

	// Start with just plain grass tiles
	for (int x = 0; x < sizeX; x++)
	{
		for (int z = 0; z < sizeZ; z++)
		{
			Quaternion rot = Quaternion.identity;
			map.terrain[x,z] = new WorldObject("Terrain/Plain/FullGrass", rot, new Vector3(0, 0, 0));
		}
	}

	// Carve out a river
	int posX = 0;
	int posZ = 0;
	int riverLength = Random.Range(25, 40);
	int prevDirection = -1;
	for (int i = 0; i < riverLength; i++)
	{
		int direction = Random.Range(0, 2); // the direction the river is moving 0 is +X, 1 is +Z
		// Select the correct tile and rotate it to reflect river geography.
		string tileType;
		Quaternion rot; 
		if (prevDirection == -1 || i == riverLength - 1)
		{
			tileType = "Terrain/River/DirtEnd";
			rot = Quaternion.Euler(0, 90 * direction, 0); 
			if (i == riverLength - 1)
			{
				rot = Quaternion.Euler(0, 90 * prevDirection + 180, 0); // invert the end piece
			} 
		} else if (prevDirection == direction)
		{
			tileType = "Terrain/River/DirtMiddle";
			rot = Quaternion.Euler(0, 90 * direction, 0);
		} else
		{
			tileType = "Terrain/River/DirtTurn";
			if (prevDirection < direction)
			{
				rot = Quaternion.Euler(0, 0, 0);
			}
			else
			{
				rot = Quaternion.Euler(0, 180, 0);
			}
		}
		map.terrain[posX, posZ] = new WorldObject(tileType, rot, new Vector3(0,0,0)); // TODO would it be more efficient to use one vector3(0,0,0) for all 0 offsets?
		switch (direction)
		{
		case 0:
			posZ++;
			break;
		case 1:
			posX++;
			break;
		}
		if (posX >= sizeX || posZ >= sizeZ) { // Don't draw river outside bounds.
			break;
		}
		prevDirection = direction;
	}

	// === Apply Resource layer ===
	// Generate forests
	for (int i = 0; i < Random.Range(20, 30); i++)
	{
		int seedX;
		int seedZ;
		seedX = Random.Range(0, worldSize);
		seedZ = Random.Range(0, worldSize);

		if (!map.terrain[seedX, seedZ].prefab.Contains("River"))
		{
			Quaternion rot = Quaternion.Euler(0, Random.Range(0, 360), 0);
			Vector3 offset = new Vector3(Random.Range(-1f, 1f), Random.Range(-0.2f, 0), Random.Range(-1f, 1f));
			map.resources[seedX, seedZ] = new WorldObject("Resources/Trees/Pine1Tree", rot, offset);
		}
	}
	// Generate stone mines
	for (int i = 0; i < Random.Range(8,10); i ++)
	{
		int seedX;
		int seedZ;
		seedX = Random.Range(0, worldSize);
		seedZ = Random.Range(0, worldSize);

		if (!map.terrain[seedX, seedZ].prefab.Contains("River"))
		{
			Quaternion rot = Quaternion.Euler(0, Random.Range(0, 360), 0);
			Vector3 offset = new Vector3(0, Random.Range(-1f, 0f), 0);
			map.resources[seedX, seedZ] = new WorldObject("Resources/StoneMine", rot, offset);
		}
	}
	return map;
}

// ======== PRIVATE CLASSES ==========

// A private class that wraps a prefab, a rotation and an offset together.
private class WorldObject
{
	public string prefab; // path to prefab under the /Assets/Resources/Prefabs/World/ directory TODO Why not point to the prefab in the first place? that seems more reasonable.
	public Quaternion rotation; // Any rotation to be applied to the object
	public Vector3 offset; // World Object offset from its tile location

	public WorldObject(string prefab, Quaternion rotation, Vector3 offset)
	{
		this.prefab = prefab;
		this.rotation = rotation;
		this.offset = offset;
	}
}

private class WorldMap
{
	public WorldObject[,] terrain;
	public WorldObject[,] resources;
	public WorldMap(int sizeX, int sizeZ)
	{
		terrain = new WorldObject[sizeX,sizeZ];
		resources = new WorldObject[sizeX, sizeZ];
	}
}
*/
}

using UnityEngine;
using System.Collections.Generic;

/*
 * 
 * This class will manage the creation of prefabs, including loading and saving them.
 * It will also store a list of all of the prefabs we've created.
 * 
 */
public class PrefabManager : MonoBehaviour 
{
	// The prefab we want to create.
	public GameObject prefab;
	// The name of the file we'll create to save and load our data from.
	public string filename = "SavedPrefabs.txt";
	// The horizontal position of the button for this prefab.
	public int buttonPositionX = 0;
	
	// A List which we'll add any created prefabs to.
	private List<GameObject> createdPrefabs = new List<GameObject>();
	
	
	/*
	 * This is where we initialize our prefabs.
	 */
	void Start () 
	{
		Debug.Log (gameObject.renderer.material.shader.name);
		
		// If there are saved prefabs to load, load them.
		if(ES2.Exists(filename))
			LoadAllPrefabs();
	}
	
	/*
	 * This method will load all of the saved prefabs when called.
	 */
	void LoadAllPrefabs()
	{
		// Load our prefab count so we know how many prefabs to load.
		int prefabCount = ES2.Load<int>(filename+"?tag=prefabCount");
		// Load each prefab using a for loop.
		for(int i=0; i < prefabCount; i++)
			LoadPrefab(i);
	}
	
	/*
	 * Loads the prefab specified by the tag number
	 * supplied as a parameter.
	 */
	void LoadPrefab(int tag)
	{
		// Create a new instance of the prefab.
		GameObject newPrefab = Instantiate(prefab) as GameObject;
		
		// Load the Transform using the auto-assigning method.
		// Note: this takes a Transform as the second parameter.
		ES2.Load<Transform>(filename+"?tag="+tag, newPrefab.transform);
		
		// Now add the newly created prefab to our createdPrefabs list.
		createdPrefabs.Add(newPrefab);
	}
	
	/*
	 * Instantiates the prefab at a random position and with a random
	 * rotation.
	 */
	void CreateRandomPrefab()
	{
		// Create a new prefab at a random position with random rotation.
		GameObject newPrefab = Instantiate (prefab, Random.insideUnitSphere*5, Random.rotation) as GameObject;
		
		// Now add the newly created prefab to our createdPrefabs list.
		createdPrefabs.Add(newPrefab);
	}
	
	/*
	 * This is called whenever the application is quit, and is where we'll
	 * save our prefabs.
	 * 
	 * Note: 
	 * We could also use OnDestroy(), which would run when the application
	 * is quit, *and* when the level is changed.
	 */
	void OnApplicationQuit()
	{
		// First, we save the length of the createdPrefabs list so we know how
		// many prefabs we need to load when we restart the application.
		ES2.Save(createdPrefabs.Count, filename+"?tag=prefabCount");
		
		// Now we iterate through our prefab list and save each one seperately,
		// using it's position in the array as the tag.
		for(int i=0; i < createdPrefabs.Count; i++)
			SavePrefab( createdPrefabs[i], i );
	}
	
	/*
	 * This is where we save the Variables/Components of our prefab.
	 */
	void SavePrefab(GameObject prefabToSave, int tag)
	{
		// Save the Transform of the prefab.
		ES2.Save (prefabToSave.transform, filename+"?tag="+tag);
	}
	
	/*
	 * Creates a button which allows us to delete data or spawn a random prefab.
	 */
	void OnGUI () 
	{
		// Button to create random prefab.
		if (GUI.Button (new Rect (buttonPositionX,0,250,100), "Create Random " + prefab.name)) 
			CreateRandomPrefab();
		
		// Button to delete all data.
		if (GUI.Button (new Rect (buttonPositionX,100,250,100), "Delete Saved "+ prefab.name))
		{
			// Delete entire file.
			ES2.Delete(filename);
			// Destroy each created prefab, then clear the List.
			for(int i=0; i<createdPrefabs.Count; i++)
				Destroy (createdPrefabs[i]);
			createdPrefabs.Clear();
		}
	}
}

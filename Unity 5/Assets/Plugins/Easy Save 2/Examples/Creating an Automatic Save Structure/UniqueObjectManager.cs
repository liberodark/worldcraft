using UnityEngine;
using System.Collections.Generic;

/*
 * This class will manage the creation, loading, saving and 
 * destruction of prefabs that we want to save/load.
 * 
 * Objects which are not created at runtime will be handled
 * in a different class.
 * 
 */
public class UniqueObjectManager : MonoBehaviour 
{
	
	// This is where we'll put all of the objects in our scene which
	// aren't dynamically created, and thus will always be there.
	public GameObject[] sceneObjects;
	
	// All of the prefabs we want to be able to create.
	public GameObject[] prefabs;
	
	// All of the prefabs we have instantiated.
	public static List<GameObject> createdObjects = new List<GameObject>();
	
	// A static reference to the instantiated UniquePrefabManager object.
	public static UniqueObjectManager mgr;
	
	
	/* 
	 * Creates a prefab by searching for one with the given name.
	 * If prefab with specified name doesn't exist, it throws an error.
	 */
	public static GameObject InstantiatePrefab(string prefabName, Vector3 position, Quaternion rotation)
	{	
		// Find the prefab. If the prefab doesn't exist, throw an error.
		GameObject prefab = FindPrefabWithName(prefabName);
		if(prefab == null)
			throw new System.Exception("Cannot instantiate prefab: No such prefab exists.");
		// If it doesn't have a UniqueID object, also throw error.
		if(prefab.GetComponent<UniqueID>() == null)
			throw new System.Exception("Can't instantiate a prefab which has no UniqueID attached.");
		
		// Instantiate our prefab and then add it to the created prefabs list.
		GameObject createdObject = Instantiate(prefab, position, rotation) as GameObject;
		CreatedObjects.Add (createdObject);
		
		return createdObject;
	}
	
	/*
	 * Same as above, but instantiates with default position and rotation.
	 */
	public static GameObject InstantiatePrefab(string prefabName)
	{
		return InstantiatePrefab(prefabName, Vector3.zero, Quaternion.identity);
	}
	
	/* 
	 * Destroys a given prefab.
	 */
	public static void DestroyObject(GameObject obj)
	{
		// Remove prefab from createdPrefabs list, or throw error if it's not in list.
		if(!CreatedObjects.Remove(obj))
			throw new System.Exception("Cannot destroy prefab: No such prefab exists.");
		
		// If destroying a parent object, we also need to destroy it's children.
		foreach(Transform child in obj.transform)
			DestroyObject(child.gameObject);
		// Destroy the object.
		Destroy(obj);
	}
	
	/* 
	 * Finds a prefab of a given name in the prefab array.
	 * If no such prefab exists, it returns null.
	 */
	public static GameObject FindPrefabWithName(string prefabName)
	{
		GameObject prefab = null;
		for(int i=0; i<Prefabs.Length; i++)
		{
			if(Prefabs[i].name == prefabName)
				prefab = Prefabs[i];
		}
		return prefab;
	}
	
	public void Awake()
	{		
		// Allows us to get a static reference to this instance.
		// (Like a singleton)
		mgr = this;
	}
	
	// Static accessor for our scene object array
	public static GameObject[] SceneObjects
	{
		get{return mgr.sceneObjects;}
	}
	// Static accessor for our prefab array
	public static GameObject[] Prefabs
	{
		get{return mgr.prefabs;}
	}
	// Static accessor for our created prefabs array
	public static List<GameObject> CreatedObjects
	{
		get{return createdObjects;}
	}
}

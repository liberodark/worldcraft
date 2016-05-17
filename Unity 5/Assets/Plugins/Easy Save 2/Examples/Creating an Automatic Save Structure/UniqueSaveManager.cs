using UnityEngine;
using System.Collections;

/*
 * This class will handle the saving and loading of data.
 */
public class UniqueSaveManager : MonoBehaviour 
{
	public string sceneObjectFile = "sceneObjectsFile.txt";
	public string createdObjectFile = "createdObjectsFile.txt";
	
	/*
	 * This is where we save certain aspects of our instantiated prefabs.
	 * This will be called when the application is quit, and when
	 * the level is changed. You could also use OnApplicationQuit.
	 */
	public void OnApplicationQuit()
	{		
		// Save how many scene objects we're saving so we know how many to load.
		ES2.Save(UniqueObjectManager.SceneObjects.Length, sceneObjectFile+"?tag=sceneObjectCount");
		
		// Iterate over each scene object
		for(int i=0; i<UniqueObjectManager.SceneObjects.Length; i++)
			SaveObject(UniqueObjectManager.SceneObjects[i], i, sceneObjectFile);
		
		// Save how many created objects we're saving so we know how many to load.
		ES2.Save(UniqueObjectManager.CreatedObjects.Count, createdObjectFile+"?tag=createdObjectCount");
		
		// Iterate over each created object
		for(int i=0; i<UniqueObjectManager.CreatedObjects.Count; i++)
			SaveObject(UniqueObjectManager.CreatedObjects[i], i, createdObjectFile);
	}
	
	/*
	 * This is where we'll load our data.
	 * 
	 */
	public void Start()
	{
		// If there's scene object data to load
		if(ES2.Exists(sceneObjectFile))
		{
			// Get how many scene objects we need to load, and then try to load each.
			// We load scene objects first as created objects may be children of them.
			int sceneObjectCount = ES2.Load<int>(sceneObjectFile+"?tag=sceneObjectCount");
			
			for(int i=0; i<sceneObjectCount; i++)
				LoadObject(i, sceneObjectFile);
		}
		
		// If there's created objects to load
		if(ES2.Exists(createdObjectFile))
		{
			// Get how many created objects we need to load, and then try to load each.
			int createdObjectCount = ES2.Load<int>(createdObjectFile+"?tag=createdObjectCount");
			
			for(int i=0; i<createdObjectCount; i++)
				LoadObject(i, createdObjectFile);
		}
	}
	
	/*
	 * Saves an Object
	 * 'i' is the number of the object we are saving.
	 */
	private void SaveObject(GameObject obj, int i, string file)
	{
		// Let's get the UniqueID object, as we'll need this.
		UniqueID uID = obj.GetComponent<UniqueID>();
		

		 //Note that we're appending the 'i' to the end of the path so that
		 //we know which object each piece of data belongs to.
		ES2.Save(uID.id, file+"?tag=uniqueID"+i);
		ES2.Save(uID.prefabName, file+"?tag=prefabName"+i);
		
		// You could add many more components here, inlcuding custom components.
		// For simplicity, we're only going to save the Transform component.
		Transform t = obj.GetComponent<Transform>();
		if(t != null)
		{
			ES2.Save(t, file+"?tag=transform"+i);
			// We'll also save the UniqueID of the parent object here, or -1
			// string if it doesn't have a parent.
			UniqueID parentuID = UniqueID.FindUniqueID(t.parent);
			if(parentuID == null)
				ES2.Save(-1, file+"?tag=parentID"+i);
			else
				ES2.Save(parentuID.id, file+"?tag=parentID"+i);
		}
	}
	
	/*
	 * Loads an Object
	 * 'i' is the number of the object we are loading.
	 */
	private void LoadObject(int i, string file)
	{
		int uniqueID = ES2.Load<int>(file+"?tag=uniqueID"+i);
		string prefabName = ES2.Load<string>(file+"?tag=prefabName"+i);
			
		// Create or get an object based on our loaded id and prefabName
		GameObject loadObject;
		// If our prefab name is blank, we're loading a scene object.
		if(prefabName == "")
			loadObject = UniqueID.FindTransform(uniqueID).gameObject;
		else 
			loadObject = UniqueObjectManager.InstantiatePrefab(prefabName);
		
		Transform t = loadObject.GetComponent<Transform>();
		if(t != null)
		{
			// Auto-assigning Load is the best way to load Components.
			ES2.Load<Transform>(file+"?tag=transform"+i, t);
			// Now we'll get the parent object, if any.
			int parentuID = ES2.Load<int>(file+"?tag=parentID"+i);
			Transform parent = UniqueID.FindTransform(parentuID);
			if(parent != null)
				t.parent = parent;
		}
	}
}

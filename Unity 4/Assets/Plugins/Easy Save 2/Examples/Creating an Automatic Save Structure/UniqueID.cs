using UnityEngine;
using System.Collections.Generic;

/*
 * A UniqueID component will be attached to every object we want to save.
 * We will also attach it to our prefabs so it's instantiated with one.
 * 
 * This class also contains static functions which keep track of all UniqueID
 * objects.
 * 
 * We also inherit from IComparable so we can easily sort them.
 */
public class UniqueID : MonoBehaviour
{
	// This will uniquely define our GameObject.
	// We'll hide it in the inspector as we'll automatically generate IDs.
	[HideInInspector]
	public int id;
	
	// This is the name of the prefab this object is based on.
	// If left blank, we will presume that this object is not based on a prefab
	// and therefore is not instantiated at runtime.
	public string prefabName = "";
	
	// Stores every UniqueID in the scene.
	private static List<UniqueID> uniqueIDList = new List<UniqueID>();
	
	/*
	 * This is called by Unity when the UniqueID is created.
	 * It'll create a unique number for it and then add it to the list.
	 */
	public void Awake()
	{
		this.id = GenerateUniqueID();
		uniqueIDList.Add(this);
	}
	
	/*
	 * This is called by Unity when the UniqueID is destroyed.
	 */
	public void OnDestroy()
	{
		// If we destroy this UniqueID, remove it from the list.
		uniqueIDList.Remove(this);
	}
	
	/*
	 * Creates a new uniqueID value which is guaranteed to be unique.
	 * This will only work if the uniqueIDList is in ascending order.
	 */
	private static int GenerateUniqueID()
	{
		// If no unique IDs have yet been set, use zero.
		if(uniqueIDList.Count == 0)
			return 0;
		// Get the last (and thus highest) number in the list
		// and then add 1 to it to get a new unique number.
		return uniqueIDList[uniqueIDList.Count-1].id+1;
	}
	
	/*
	 * Gets the UniqueID object relating to a Transform.
	 * Returns null if no Transforms match.
	 */
	public static UniqueID FindUniqueID(Transform t)
	{
		foreach(UniqueID uID in uniqueIDList)
			if(uID.transform == t)
				return uID;
		return null;
	}
	
	/*
	 * Gets the Transform relating to a UniqueID.id.
	 * Returns null if no Transform's match.
	 */
	public static Transform FindTransform(int id)
	{
		foreach(UniqueID uID in uniqueIDList)
			if(uID.id == id)
				return uID.transform;
		return null;
	}
}
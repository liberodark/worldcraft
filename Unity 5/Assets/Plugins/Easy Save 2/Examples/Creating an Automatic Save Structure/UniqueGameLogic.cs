using UnityEngine;
using System.Collections;

/*
 * This class handles our game logic. In this case we just randomly create objects,
 * destroy objects or make objects a child of another.
 * We'll also randomly change the scale of the scene objects.
 */
public class UniqueGameLogic : MonoBehaviour 
{
	public void Start()
	{
		// Start our Game Logic routines
		StartCoroutine(DestroyOrCreateRoutine(3f, 1f));
		StartCoroutine(ScaleRoutine(3f, 1f));
		StartCoroutine(MakeChildRoutine(3f, 1f));
	}
	
	public IEnumerator DestroyOrCreateRoutine(float delaySeconds, float runEverySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		while(true)
		{
			// If we have more than 20 created objects, randomly destroy one ...
			if(UniqueObjectManager.CreatedObjects.Count > 9)
				UniqueObjectManager.DestroyObject(UniqueObjectManager.CreatedObjects[Random.Range(0, UniqueObjectManager.CreatedObjects.Count)]);
			// ... Else, randomly create one.
			else
				UniqueObjectManager.InstantiatePrefab(	UniqueObjectManager.Prefabs[Random.Range(0, UniqueObjectManager.Prefabs.Length)].name, 
														Random.insideUnitSphere*10, 
														Random.rotation);
			yield return new WaitForSeconds(runEverySeconds);
		}
	}
	
	public IEnumerator MakeChildRoutine(float delaySeconds, float runEverySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		while(true)
		{
			 // If there are more than 10 objects in the scene, make first one a child of another.
			if(UniqueObjectManager.CreatedObjects.Count > 4)
			{
				UniqueObjectManager.CreatedObjects[0].transform.parent = 
					UniqueObjectManager.CreatedObjects[Random.Range(1, UniqueObjectManager.CreatedObjects.Count)].transform;
			}
			yield return new WaitForSeconds(runEverySeconds);
		}
	}
	
	public IEnumerator ScaleRoutine(float delaySeconds, float runEverySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		while(true)
		{
			UniqueObjectManager.SceneObjects[Random.Range(0,UniqueObjectManager.SceneObjects.Length)].transform.localScale =
				Random.insideUnitSphere;
			yield return new WaitForSeconds(runEverySeconds);
		}
	}
}

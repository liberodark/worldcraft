using UnityEngine;
using System.Collections;

public class			DontDestroyOnLoad : MonoBehaviour
{
	#region Input Data
	public string		_accepted_level = "Game";
	#endregion

	#region Unity
	void				Awake()
	{
		DontDestroyOnLoad(this.gameObject);
	}
 
	void				OnLevelWasLoaded(int level)
	{
		if (Application.loadedLevelName != this._accepted_level)
		   Destroy(this.gameObject);
	}
	#endregion
}

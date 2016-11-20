using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameSetup : MonoBehaviour {
	
	public static BlockSet blockSet;

    void Awake()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (blockSet != null) GetComponent<Map>().SetBlockSet(blockSet);
    }

 //   void OnLevelWasLoaded(int level) {
	//}
	
}

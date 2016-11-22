using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameSetup : MonoBehaviour {
	
	public static BlockSet blockSet;

    void Awake()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        GameStateManager.IsPaused = true;
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (blockSet != null)
        {
            Map map = GetComponent<Map>();
            if (map != null)
            {
                map.SetBlockSet(blockSet);
            }
        }
    }

 //   void OnLevelWasLoaded(int level) {
	//}
	
}

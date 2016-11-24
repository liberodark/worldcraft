using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameSetup : MonoBehaviour {
    public static bool isEnable = true;
	public static BlockSet blockSet;

    void Awake()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (blockSet != null && isEnable)
        {
            GetComponent<Map>().SetBlockSet(blockSet);
        }
    }

    //   void OnLevelWasLoaded(int level) {
    //}
    public void OnDestroy()
    {
        isEnable = false;
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoad : MonoBehaviour
{
    #region Input Data
    public string _accepted_level = "Game";
    #endregion

    #region Unity
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += LoadScene;
    }

    //void OnLevelWasLoaded(int level)
    //{
        
    //}

    void LoadScene(Scene scene, LoadSceneMode mode)
    {
        // SceneManager.LoadScene(this._accepted_level);
        if (this != null)
        {
            Destroy(this.gameObject);
        }
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public void LoadLevel(string levelName) {
        Debug.Log("Loading level: " + levelName);
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelName);
    }
}

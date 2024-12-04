using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneManager = Global.SceneManager;

public class ReloadSceneByName : MonoBehaviour
{
    public string sceneName;

    public void ReloadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}

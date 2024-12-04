using System;
using UnityEngine;

namespace Global
{
    public class SceneManager : MonoBehaviour
    {
        public void LoadScene(int sceneNumber)
        {
            if (sceneNumber < 0 || sceneNumber >= UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
            {
                Debug.LogError("Invalid scene index. Make sure it's within the build settings range.");
                return;
            }
            
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneNumber);
        }

        public void LoadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("Scene name cannot be null or empty.");
                return;
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }

        public void ReloadCurrentScene()
        {
            int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneIndex);
        }

        public void QuitApplication()
        {
            Debug.Log("Quitting application...");
            Application.Quit();
        }
    }
}
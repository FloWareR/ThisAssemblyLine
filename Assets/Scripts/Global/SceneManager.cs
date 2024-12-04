using System.Collections;
using UnityEngine;

namespace Global
{
    public class SceneManager : MonoBehaviour
    {
        public static SceneManager Instance { get; private set; }
        public Animator transition;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

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

            StartCoroutine(SceneTransition(sceneName));
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

        public IEnumerator SceneTransition(string sceneName)
        {
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(1f);
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}
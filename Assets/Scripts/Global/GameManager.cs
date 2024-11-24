using System.Collections.Generic;
using Environment;
using ScriptableObjects;
using UnityEngine;

namespace Global
{
    public class GameManager : MonoBehaviour
    {
        public List<LevelData> levels;
        public LevelData currentLevel;
        public int currentLevelIndex = 0;
        private ObjectSpawnerManager _objectSpawnerManager;
        
        public int currentScore;
        private bool _isLevelActive;

        public GameManager(bool isLevelActive)
        {
            _isLevelActive = isLevelActive;
        }

        private void Awake()
        {
            _objectSpawnerManager = GetComponent<ObjectSpawnerManager>();
        }

        private void Start()
        {
            if (levels.Count > 0)
                LoadLevel(currentLevelIndex);
            else
                Debug.LogError("No levels assigned to the GameManager!");
        }
        
        public void LoadLevel(int levelIndex)
        {
            if (levelIndex < 0 || levelIndex >= levels.Count)
            {
                Debug.LogError("Invalid level index!");
                return;
            }
            currentLevelIndex = levelIndex;
            currentLevel = levels[levelIndex];
            _isLevelActive = true;
            _objectSpawnerManager.InitializeParts(currentLevel);
            foreach (var objectDta in currentLevel.objectsToBuild)
            { 
                MonitorManager.Instance.SpawnMonitor(objectDta.objectData.objectImage, objectDta.quantity);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;
using Environment; 

namespace Global
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance { get; private set; }

        public List<LevelData> levels;
        public LevelData currentLevel;
        public int currentLevelIndex = 0;
        private ObjectSpawnerManager _objectSpawnerManager;
        private ScoreBoardController _scoreBoardController;
        public static event Action<LevelData> LoadNewLevel;

        public int currentScore;
        public int previousObjectScore;
        private bool _isLevelActive;
        

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject); 
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject); 

            _objectSpawnerManager = GetComponent<ObjectSpawnerManager>();
            _scoreBoardController = FindAnyObjectByType<ScoreBoardController>();

        }

        private void Start()
        {
            if (levels.Count > 0)
                LoadLevel(currentLevelIndex);
            else
                Debug.LogError("No levels assigned to the GameManager!");
        }

        private void OnEnable()
        {
            DeliveryBoxController.EvaluateObject += OnEvaluateObject;
        }
        
        private void OnDisable()
        {
            DeliveryBoxController.EvaluateObject -= OnEvaluateObject;
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
            LoadNewLevel?.Invoke(currentLevel);
        }

        private void OnEvaluateObject(GameObject playerObject)
        {
            foreach (var prefabObject in currentLevel.objectsToBuild)
            { 
                previousObjectScore = (int)ScoreManager.Instance.CompareObjects(prefabObject.objectData.objectPrefab, 
                    playerObject);
                currentScore += previousObjectScore;
            }
            _scoreBoardController.UpdateScores();
            Destroy(playerObject);
        }
    }
}

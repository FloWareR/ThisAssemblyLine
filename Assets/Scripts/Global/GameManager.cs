using System;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;
using Environment;
using UnityEngine.Serialization;

namespace Global
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private AudioClip[] backgroundMusic;
        public static GameManager instance { get; private set; }
        public List<LevelData> levels;
        public LevelData currentLevel;
        
        private ScoreBoardController _scoreBoardController;
        private TimerMonitorController _timerMonitorController;
        public static event Action<LevelData> LoadNewLevel;
        public static event Action LevelTimeUp;

        public int currentLevelIndex = 0;
        public int currentScore;
        public int previousObjectScore;
        private bool _isLevelActive;
        public float musicVolume = 1f;

        public float levelTimeLeft;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject); 
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject); 

            _scoreBoardController = FindAnyObjectByType<ScoreBoardController>();
            _timerMonitorController = FindAnyObjectByType<TimerMonitorController>();

        }

        private void Start()
        {
            if (levels.Count <= 0) return;
            LoadLevel(currentLevelIndex);
            MusicChannel.PlayerSoundTrack(backgroundMusic, musicVolume);
        }

        private void Update()
        {
            if(!_isLevelActive) return;
            LevelTimer();
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
            levelTimeLeft = currentLevel.timeLimit;
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

        private void LevelTimer()
        {
            levelTimeLeft -= Time.deltaTime;
            if (!(levelTimeLeft <= 0)) return;
            LevelTimeUp?.Invoke();
            _isLevelActive = false;
        }
        
    }
}

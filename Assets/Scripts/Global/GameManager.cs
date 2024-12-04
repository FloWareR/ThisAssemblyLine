using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects;
using UnityEngine;
using Environment;
using UnityEngine.Serialization;

namespace Global
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private AudioClip[] backgroundMusic;
        [SerializeField] private float timeBetweenRounds = 10f;
        public static GameManager instance { get; private set; }
        public List<LevelData> levels;
        public LevelData currentLevel;
        
        private ScoreBoardController _scoreBoardController;
        private TimerMonitorController _timerMonitorController;
        public static event Action<LevelData> LoadNewLevel;
        public static event Action LevelTimeUp;
        public static event Action LevelDone;

        public int allowedStrikes;
        private int _strikes;
        public int currentLevelIndex = 0;
        public int currentScore = 0;
        public int previousObjectScore = 0;
        public bool _isLevelActive;
        public float musicVolume = 1f;
        public float levelTimeLeft;
        public ObjectToBuild[] objectsLeft;
        private bool _isChangingLevel = false;
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject); 
                return;
            }
            
            instance = this;
            _scoreBoardController = FindAnyObjectByType<ScoreBoardController>();
            _timerMonitorController = FindAnyObjectByType<TimerMonitorController>();
            
        }
        private void OnEnable()
        {
            DeliveryBoxController.EvaluateObject += OnEvaluateObject;
        }
        
        private void OnDisable()
        {
            DeliveryBoxController.EvaluateObject -= OnEvaluateObject;
            instance = null;
        }
        
        private void Start()
        {
            if (levels.Count <= 0) return;
            LoadLevel(currentLevelIndex);
            MusicChannel.PlayerSoundTrack(backgroundMusic, musicVolume);
        }

        private void Update()
        {
            if (!_isLevelActive) return;

            if (Input.GetKeyDown(KeyCode.R))
            {
                StartCoroutine(NextLevel());
            }

            LevelTimer();
        }

        private void EvaluateObjects()
        {
            var allObjectsBuilt = true;
            foreach (var objectToBuild in objectsLeft)
            {
                if (objectToBuild.quantity != 0)
                {
                    allObjectsBuilt = false;
                }
            }

            if (allObjectsBuilt)
            {
                StartCoroutine(NextLevel());
            }
        }

        private IEnumerator NextLevel()
        {
            if (_isChangingLevel) yield break;
            _isChangingLevel = true;

            _isLevelActive = false;
            Debug.Log("Level completed! Raising LevelDone event...");
            LevelDone?.Invoke();

            Debug.Log($"Waiting for {timeBetweenRounds} seconds...");
            yield return new WaitForSeconds(timeBetweenRounds);

            Debug.Log("Loading the next level...");
            LoadLevel(currentLevelIndex + 1);
            _isChangingLevel = false;
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
            objectsLeft = currentLevel.objectsToBuild
                .Select(obj => new ObjectToBuild
                {
                    objectData = obj.objectData,
                    quantity = obj.quantity
                }).ToArray();            foreach (var objectToBuild in objectsLeft)
            {
            }
            LoadNewLevel?.Invoke(currentLevel);
        }

        private void OnEvaluateObject(GameObject playerObject)
        {
            string playerGameObjectName = playerObject.name; 
            ;
            foreach (var prefabObject in currentLevel.objectsToBuild)
            { 
                if (prefabObject.objectData.objectPrefab.name != playerGameObjectName)
                {
                    Debug.Log("CONTINUA");
                    continue;
                }
                previousObjectScore = (int)ScoreManager.Instance.CompareObjects(prefabObject.objectData.objectPrefab, 
                    playerObject);
                currentScore += previousObjectScore;
                playerGameObjectName = ScoreManager.Instance.ObjectCreated(prefabObject.objectData.objectPrefab, playerObject);
                foreach (var objectToBuild in objectsLeft)
                {
                    if (objectToBuild.objectData.objectPrefab.name != playerGameObjectName) continue;
                    objectToBuild.quantity = Mathf.Max(0, objectToBuild.quantity - 1); 
                    break;
                }
            }

            if (previousObjectScore < 60)
            {
                _strikes++;
            }

            if (_strikes > allowedStrikes)
            {
                LevelTimeUp?.Invoke();
            }

            _scoreBoardController.UpdateScores();
            EvaluateObjects();
            Destroy(playerObject);
        }


        private void LevelTimer()
        {
            levelTimeLeft -= Time.deltaTime;
            if (!(levelTimeLeft <= 0)) return;
            LevelTimeUp?.Invoke();
            _isLevelActive = false;
            SavePlayerScore(currentScore);
            UpdateHighScore(currentScore);

        }
        
        private void SavePlayerScore(int score)
        {
            PlayerPrefs.SetInt("PlayerScore", score);
            PlayerPrefs.Save(); 
            Debug.Log($"Player score saved: {score}");
        }

        public int GetPlayerScore()
        {
            return PlayerPrefs.GetInt("PlayerScore", 0); 
        }
        
        private void UpdateHighScore(int score)
        {
            var highScore = GetHighScore();
            if (score <= highScore) return;
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
            Debug.Log($"New high score: {score}");
        }
        
        public int GetHighScore()
        {
            return PlayerPrefs.GetInt("HighScore", 0); 
        }
        
    }
}

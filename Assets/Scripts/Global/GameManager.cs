using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects;
using UnityEngine;
using Environment;

namespace Global
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private AudioClip[] backgroundMusic;
        [SerializeField] private float timeBetweenRounds;
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
        private ObjectToBuild[] _objectsLeft;
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

        private void EvaluateObjects()
        {
            var allObjectsBuilt = true;
            foreach (var objectToBuild in _objectsLeft)
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
            _isLevelActive = false;
            yield return new WaitForSeconds(timeBetweenRounds);
            LoadLevel(currentLevelIndex + 1);
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
            _objectsLeft = currentLevel.objectsToBuild;
            foreach (var objectToBuild in _objectsLeft)
            {
                Debug.Log($"{objectToBuild.objectData.name}, {objectToBuild.quantity}");
            }
            LoadNewLevel?.Invoke(currentLevel);
        }

        private void OnEvaluateObject(GameObject playerObject)
        {
            string playerGameObjectName;
            foreach (var prefabObject in currentLevel.objectsToBuild)
            { 
                previousObjectScore = (int)ScoreManager.Instance.CompareObjects(prefabObject.objectData.objectPrefab, 
                    playerObject);
                currentScore += previousObjectScore;
                playerGameObjectName = ScoreManager.Instance.ObjectCreated(prefabObject.objectData.objectPrefab, playerObject);
                foreach (var objectToBuild in _objectsLeft)
                {
                    if (objectToBuild.objectData.objectPrefab.name != playerGameObjectName) continue;
                    objectToBuild.quantity = Mathf.Max(0, objectToBuild.quantity - 1); 
                    break;
                }
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
        }
        
    }
}

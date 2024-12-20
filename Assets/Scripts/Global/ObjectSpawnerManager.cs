using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Global
{
    public class ObjectSpawnerManager : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;
        private LevelData _currentLevel;
        private float _spawnChance;
        private List<PartData> _junkPieces = new List<PartData>();
        private Dictionary<PartData, int> _requiredPieces = new Dictionary<PartData, int>();
        
        private bool _inGame = true;
        
        private void OnEnable()
        {
            GameManager.LoadNewLevel += OnLoadNewLevel;
            GameManager.LevelDone += PauseSpawn;
            GameManager.LevelTimeUp += PauseSpawn;
        }

        private void PauseSpawn()
        { 
            StopAllCoroutines();
        }

        private void OnLoadNewLevel(LevelData obj)
        {
            _inGame = true;
            InitializeParts(obj);
        }

        private void OnDisable()
        {
            GameManager.LevelDone -= PauseSpawn;
            GameManager.LoadNewLevel -= OnLoadNewLevel;
            GameManager.LevelTimeUp -= PauseSpawn;

        }
        
        public void InitializeParts(LevelData data)
        {
            _currentLevel = data;
            _requiredPieces.Clear();
            _junkPieces.Clear();
            foreach (var requiredObject in _currentLevel.objectsToBuild)
            {
                var objectData = requiredObject.objectData;
                foreach (var requiredPart in objectData.requiredParts)
                {
                    if (!_requiredPieces.TryAdd(requiredPart, 1))
                    {
                        _requiredPieces[requiredPart]++;
                    }
                }
            }
            _junkPieces.AddRange(_currentLevel.junkParts);
            StartCoroutine(LevelSpawnLoop());
        }

        private IEnumerator LevelSpawnLoop()
        {
            while (_inGame)
            {
                RandomizeSpawn();
                yield return new WaitForSeconds(_currentLevel.spawnInterval);
            }
        }

        private void RandomizeSpawn()
        {
            var spawnChance = Random.Range(0f, 1f);

            if (spawnChance <= _currentLevel.requiredPartProbability)
            {
                SpawnRequiredPart();
            }
            else
            {
                SpawnJunkPart();
            }
        }

        private void SpawnJunkPart()
        {
            var randomJunkPart = _junkPieces[Random.Range(0, _junkPieces.Count)];
            ObjectPoolManager.Instance.SpawnFromPool(randomJunkPart.partName, spawnPoint.position, transform.rotation);
        }

        private void SpawnRequiredPart()
        {
            var requiredPartKeys = new List<PartData>(_requiredPieces.Keys);
            var randomPart = requiredPartKeys[Random.Range(0, requiredPartKeys.Count)];
            
            ObjectPoolManager.Instance.SpawnFromPool(randomPart.partName, spawnPoint.position, transform.rotation);

            _requiredPieces[randomPart]--;
        }
    }
}

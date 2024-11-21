using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Global
{
    public class ObjectSpawnerManager : MonoBehaviour
    {
        public LevelData currentLevel;
        private float _spawnChance;
        private List<PartData> _junkPieces = new List<PartData>();
        private Dictionary<PartData, int> _requiredPieces = new Dictionary<PartData, int>();
        
        private bool _inGame = true;
        private void Start()
        {
            InitializeParts();
            StartCoroutine(LevelSpawnLoop());
        }

        private void InitializeParts()
        {
            _requiredPieces.Clear();
            _junkPieces.Clear();
            foreach (var requiredObject in currentLevel.objectsToBuild)
            {
                foreach (var requiredPart in requiredObject.requiredParts)
                {
                    if (!_requiredPieces.TryAdd(requiredPart, 1))
                    {
                        _requiredPieces[requiredPart]++;
                    }
                }
            }
            _junkPieces.AddRange(currentLevel.junkParts);
        }

        private IEnumerator LevelSpawnLoop()
        {
            while (_inGame)
            {
                RandomizeSpawn();
                yield return new WaitForSeconds(currentLevel.spawnInterval);
            }
        }

        private void RandomizeSpawn()
        {
            var spawnChance = Random.Range(0f, 1f);

            if (spawnChance <= currentLevel.requiredPartProbability)
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
            ObjectPoolManager.Instance.SpawnFromPool(randomJunkPart.partName, transform.position, transform.rotation);
        }

        private void SpawnRequiredPart()
        {
            var requiredPartKeys = new List<PartData>(_requiredPieces.Keys);
            var randomPart = requiredPartKeys[Random.Range(0, requiredPartKeys.Count)];
            
            ObjectPoolManager.Instance.SpawnFromPool(randomPart.partName, transform.position, transform.rotation);

            _requiredPieces[randomPart]--;
        }
    }
}

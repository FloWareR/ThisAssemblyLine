using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewLevelData", menuName = "Game/LevelData")]
    public class LevelData : ScriptableObject
    {
        [Header("Level Details")]
        public int levelNumber;
        public float timeLimit;
        public ObjectData[] objectsToBuild;
        
        [Header("Difficulty")]
        public float conveyorSpeed;
        [Tooltip("How likely required parts are to be spawned")] [Range(0f, 1f)] public float requiredPartProbability;
        public float spawnInterval;
        public PartData[] junkParts;
        
    }
}
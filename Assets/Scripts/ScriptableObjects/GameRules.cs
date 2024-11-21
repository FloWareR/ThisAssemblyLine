using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Managers/Game Manager", fileName = "GameManager")]
    public class GameSettings : ScriptableObject
    {
        [Header("Conveyor Settings")] 
        public float conveyorSpeed;
        public float spawnInterval;

        [Header("GameRules")] 
        public float mistakesAllowed;
        public float timeLimitPerAssembly;

        [Header("Scoring")]
        public int pointsPerCorrectPiece;
        public int penaltyPerMistake;
    }
}

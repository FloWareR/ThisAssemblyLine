using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewPartData", menuName = "Game/PartData")]
    public class PartData : ScriptableObject
    {
        public string partName;
        public GameObject partPrefab;
    }
}
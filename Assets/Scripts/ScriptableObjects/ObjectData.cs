using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewObjectData", menuName = "Game/ObjectData")]
    public class ObjectData : ScriptableObject
    {
        public string objectName;
        public GameObject objectPrefab;
        public Sprite objectImage;
        public PartData[] requiredParts;
    }
}
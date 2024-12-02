using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Global
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance;
        
        [SerializeField] private float maxDistance = 1.0f; 
        [SerializeField ]private float tolerance = 0.005f;  
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);
        }

        public float CompareObjects(GameObject prefabObject, GameObject playerObject)
        {
            var prefabChildren = GetChildren(prefabObject.transform);
            var playerChildren = GetChildren(playerObject.transform);

            if (!AreSnapPointsMatching(prefabChildren, playerChildren))
            {
                return -50f;
            }

            var prefabDistance = CalculateDistance(prefabChildren);
            var playerDistance = CalculateDistance(playerChildren);

            return CalculateScore(prefabDistance, playerDistance);
        }


        private List<GameObject> GetChildren(Transform parent)
        {
            var snapPointsList = new List<GameObject>();
            foreach (Transform child in parent)
            {
                if(child.CompareTag($"SnapPoint"))
                {
                    snapPointsList.Add(child.gameObject);
                }

                foreach (Transform miniChild in child)
                {
                    if(miniChild.CompareTag($"SnapPoint"))
                    {
                        snapPointsList.Add(miniChild.gameObject);
                    }
                }
            }

            return snapPointsList;
        }

        private float CalculateDistance(List<GameObject> objectList)
        {
            var object1 = objectList[0];
            var object2 = objectList[1];
            var distance = Vector3.Distance(object1.transform.position, object2.transform.position);
            return (float)System.Math.Round(distance, 4);

        }
        
        private float CalculateScore(float prefabDistance, float playerDistance)
        {
            if (playerDistance >= prefabDistance + maxDistance)
            {
                return 0f;
            }

            var difference = Mathf.Abs(prefabDistance - playerDistance);

            if (difference <= tolerance)
            {
                return 100f; 
            }

            var maxDifference = maxDistance - tolerance;
            var score = 100f * (1f - (difference / maxDifference));
            return Mathf.Clamp(score, 0f, 100f);
        }
        
        private bool AreSnapPointsMatching(List<GameObject> prefabChildren, List<GameObject> playerChildren)
        {
            if (prefabChildren.Count != playerChildren.Count)
                return false;

            var prefabNames = new HashSet<string>(prefabChildren.ConvertAll(child => child.name));
            var playerNames = new HashSet<string>(playerChildren.ConvertAll(child => child.name));

            return prefabNames.SetEquals(playerNames);
        }

    }
}

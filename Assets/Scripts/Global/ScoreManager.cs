using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Global
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance;
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);
        }

        public float CompareObjects(GameObject prefabObject, GameObject playerObject)
        {
            var prefabChildren = GetChildren(prefabObject.transform);
            var playerChildren = GetChildren(playerObject.transform);

            if (!AreSnapPointsMatching(prefabObject, playerObject))
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

                snapPointsList.AddRange(from Transform miniChild in child where miniChild.CompareTag($"SnapPoint") select miniChild.gameObject);
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
            var score = Mathf.Abs((playerDistance / prefabDistance) - 100);
            if (score > 98.5f) score = 100;
            if (score < 65f) score *= .5f;
            
            return score;
        }

        
        
        private bool AreSnapPointsMatching(GameObject prefabObject, GameObject playerObject)
        {
            var prefabChildren = (from Transform child in prefabObject.transform select child.gameObject).ToList();
            var playerChildren = (from Transform child in playerObject.transform select child.gameObject).ToList();

            if (prefabChildren.Count != playerChildren.Count)
                return false;

            var prefabNames = new HashSet<string>(prefabChildren.ConvertAll(child => child.name));
            var playerNames = new HashSet<string>(playerChildren.ConvertAll(child => child.name.Replace("(Clone)", "").Trim())
            );
            
            return prefabNames.SetEquals(playerNames);
        }
        


    }
}

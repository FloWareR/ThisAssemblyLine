using System.Collections.Generic;
using UnityEngine;

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
            if (prefabObject == null || playerObject == null) return 0f;
            var prefabValues = GetPrefabValues(prefabObject);
            var playerValues = GetPrefabValues(playerObject);
            
            DebugMetrics(prefabValues);
            DebugMetrics(playerValues);

            return 0; 
        }

        private void DebugMetrics(Dictionary<(string, string), float> metrics)
        {
            foreach (var entry in metrics)
            {
                var key1 = entry.Key.Item1;  
                var key2 = entry.Key.Item2; 
                var distance = entry.Value;

                Debug.Log($"Piece 1: {key1}, Piece 2: {key2}, Distance: {distance}");
            }
        }

        public Dictionary<(string, string), float> GetPrefabValues(GameObject objectPrefab)
        {
            var prefabPieces = GetChildPieces(objectPrefab.transform);
            return GetPieceMetrics(prefabPieces);
        }

        private Dictionary<(string, string), float> GetPieceMetrics(List<Transform> prefabPieces)
        {
            var metrics = new Dictionary<(string, string), float>();

            for (var i = 0; i < prefabPieces.Count; i++)
            {
                var snapPoint1 = prefabPieces[i].Find("SnapPoint");
                for (var j = i + 1; j < prefabPieces.Count; j++)
                {
                    var snapPoint2 = prefabPieces[j].Find("SnapPoint");

                    if (snapPoint1 != null && snapPoint2 != null)
                    {
                        var distance = Vector3.Distance(snapPoint1.position, snapPoint2.position);

                        metrics.Add((prefabPieces[i].name, prefabPieces[j].name), distance);
                    }
                }
            }

            return metrics;
        }

        private List<Transform> GetChildPieces(Transform parent)
        {
            var pieces = new List<Transform>();
            foreach (Transform child in parent)
            {
                pieces.Add(child);
            }
            return pieces;
        }
        
    }
}

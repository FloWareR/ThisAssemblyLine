using System.Collections;
using System.Collections.Generic;
using Global;
using UnityEngine;

namespace Environment
{
    public class DeliveryBoxController : MonoBehaviour
    {
        [SerializeField] private float combineDelay = 2f;
        private readonly HashSet<GameObject> _objectsInBox = new HashSet<GameObject>();
        private GameManager _gameManager;
        private void Awake()
        {
            _gameManager = FindAnyObjectByType<GameManager>();
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.CompareTag("Weldable"))
            {
                _objectsInBox.Add(collision.gameObject);
            }        
        }
        

        private void OnTriggerExit(Collider collision)
        {
            if (collision.gameObject.CompareTag("Weldable"))
            {
                _objectsInBox.Remove(collision.gameObject);
            }
        }

        public void CombineAndSendToScoreManager()
        {
            foreach (var objectToBuild in _gameManager.currentLevel.objectsToBuild)
            {
                CombineObjectsRoutine(objectToBuild.objectData.objectPrefab);
                
            }
        }

        private void CombineObjectsRoutine(GameObject objectPrefab)
        {
            var combinedObject = new GameObject("CombinedObject");
            
            foreach (var obj in _objectsInBox)
            {
                obj.transform.SetParent(combinedObject.transform);
            }

            ScoreManager.Instance.CompareObjects(objectPrefab, combinedObject);
            _objectsInBox.Clear();
        }
    }
}
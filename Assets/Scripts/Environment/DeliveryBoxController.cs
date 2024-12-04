using System;
using System.Collections.Generic;
using Global;
using UnityEngine;

namespace Environment
{
    public class DeliveryBoxController : MonoBehaviour
    {
        private HashSet<GameObject> _objectsInBox = new HashSet<GameObject>();
        private GameManager _gameManager;

        // Define the EvaluateObject event
        public static event Action<GameObject> EvaluateObject;

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

        private void CombineAndSendToScoreManager()
        {
            if (_objectsInBox.Count == 0)
            {
                return; 
            }            
            var combinedObject = new GameObject("CombinedObject");
            foreach (var obj in _objectsInBox)
            {
                obj.transform.SetParent(combinedObject.transform);
                combinedObject.name = obj.name.Replace("_A(Clone)", "");
                combinedObject.name = obj.name.Replace("_B(Clone)", "");
            }
Debug.Log(combinedObject.name);
            EvaluateObject?.Invoke(combinedObject);
            foreach (var objectUsed in _objectsInBox)
            {
                var sanitizedObjectName = objectUsed.name.Replace("(Clone)", "").Trim();
                ObjectPoolManager.Instance.ReturnToPool(sanitizedObjectName, objectUsed);
            }
            _objectsInBox.Clear();
        }
    }
}

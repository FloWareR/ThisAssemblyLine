using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Global;
using UnityEngine;

namespace Environment
{
    public class MineCartController : MonoBehaviour
    {
        [Header("Cart Settings")] 
        [SerializeField] private float moveSpeed;
        [SerializeField] private List<Transform> railSpawnList;
        [SerializeField] private int objectPerSide;
        [SerializeField] private Quaternion defaultRotation = Quaternion.Euler(0f, 90f, 0f);
        private List<GameObject> _rails = new List<GameObject>();
        private float _railInterval;
        private float _railWidth = 0f;
        
        private void Start()
        {
            SpawnRails();
            Debug.Log(_railWidth);
            StartCoroutine(SpawnRailsCoroutine());
        }

        private void Update()
        {
            _railInterval = _railWidth / moveSpeed;
            transform.position += transform.forward * (moveSpeed * Time.deltaTime);
        }


        IEnumerator SpawnRailsCoroutine()
        {
            do
            {
                yield return new WaitForSeconds(_railInterval);
                RelocateRail();
            } while (true);
            // ReSharper disable once IteratorNeverReturns
        }

        private void RelocateRail()
        {
            var currentRightPosition = _rails[^1].transform.position + new Vector3(0, 0 , _railWidth);
            var currentLeftPosition = _rails[^2].transform.position + new Vector3(0, 0 , _railWidth);

            _rails[0].transform.position = currentRightPosition;
            _rails[1].transform.position = currentLeftPosition;
            var rail = _rails[0];
            _rails.RemoveAt(0);
            _rails.Add(rail);
            rail = _rails[0];
            _rails.RemoveAt(0);
            _rails.Add(rail);
        }

        private void SpawnRails()
        {
            var currentRightPosition = railSpawnList[0].position;
            var currentLeftPosition = railSpawnList[1].position;

            for (var i = 0; i < objectPerSide; i++)
            {
                var railRight = ObjectPoolManager.Instance.SpawnFromPool("Rail", currentRightPosition, defaultRotation);
                _rails.Add(railRight);
                if(_railWidth == 0f)  _railWidth = railRight.GetComponent<Collider>().bounds.size.x;
                currentRightPosition = railRight.transform.position + new Vector3(0f, 0f, _railWidth);
                var railLeft = ObjectPoolManager.Instance.SpawnFromPool("Rail", currentLeftPosition, defaultRotation);
                _rails.Add(railLeft);
                currentLeftPosition = railLeft.transform.position + new Vector3(0f, 0f, _railWidth);
            }
        }
    }
}

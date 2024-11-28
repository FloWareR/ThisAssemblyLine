using System.Collections;
using System.Collections.Generic;
using Global;
using UnityEngine;
public class MineCartController : MonoBehaviour
{
    [Header("Cart Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float despawnDistance = 100f;
    [SerializeField] private List<Transform> railSpawnList;
    [SerializeField] private int objectPerSide;
    [SerializeField] private Quaternion defaultRotation = Quaternion.Euler(0f, 90f, 0f);

    private List<GameObject> _rails = new List<GameObject>();
    private float _railInterval;
    private float _railWidth = 0f;

    private void Start()
    {
        SpawnRails();
        StartCoroutine(SpawnRailsCoroutine());
        moveSpeed = Random.Range(1f, moveSpeed);
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
            var moveDirection = transform.forward.normalized;
            var railRotation = Quaternion.LookRotation(moveDirection) * defaultRotation;

            var currentRightPosition = _rails[^1].transform.position + (moveDirection * _railWidth);
            var currentLeftPosition = _rails[^2].transform.position + (moveDirection * _railWidth);

            _rails[0].transform.SetPositionAndRotation(currentRightPosition, railRotation);
            _rails[1].transform.SetPositionAndRotation(currentLeftPosition, railRotation);

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

            var moveDirection = transform.forward.normalized;
            var railRotation = Quaternion.LookRotation(moveDirection) * defaultRotation;

            for (var i = 0; i < objectPerSide; i++)
            {
                var railRight = ObjectPoolManager.Instance.SpawnFromPool("Rail", currentRightPosition, railRotation);
                _rails.Add(railRight);

                if (_railWidth == 0f) _railWidth = railRight.GetComponent<Collider>().bounds.size.x;

                currentRightPosition += moveDirection * _railWidth;

                var railLeft = ObjectPoolManager.Instance.SpawnFromPool("Rail", currentLeftPosition, railRotation);
                _rails.Add(railLeft);

                currentLeftPosition += moveDirection * _railWidth;
            }
        }

        public float CalculateDespawnTime()
        {
            return despawnDistance / moveSpeed;
        }
}

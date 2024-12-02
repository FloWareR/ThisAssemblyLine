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

    public List<GameObject> _rails = new List<GameObject>();
    private float _railInterval;
    private float _railWidth = 0f;
    private Coroutine _spawnRailsCoroutine;

    public void InitializeCart()
    {
        _rails.Clear();
        SpawnRails();
        StartSpawnRailsCoroutine();
    }

    private void Update()
    {
        _railInterval = _railWidth / moveSpeed;
        transform.position += transform.forward * (moveSpeed * Time.deltaTime);
    }

    private void OnDisable()
    {
        StopSpawnRailsCoroutine();
    }

    private void StartSpawnRailsCoroutine()
    {
        if (_spawnRailsCoroutine == null)
        {
            foreach (GameObject rail in _rails)
            {
                rail.SetActive(true);
            }
            _spawnRailsCoroutine = StartCoroutine(SpawnRailsCoroutine());
        }
    }

    private void StopSpawnRailsCoroutine()
    {
        if (_spawnRailsCoroutine != null)
        {
            StopCoroutine(_spawnRailsCoroutine);
            _spawnRailsCoroutine = null;
        }
    }

    IEnumerator SpawnRailsCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_railInterval);
            RelocateRail();
        }
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
            railRight.SetActive(true);
            _rails.Add(railRight);

            if (_railWidth == 0f) _railWidth = railRight.GetComponent<Collider>().bounds.size.x;

            currentRightPosition += moveDirection * _railWidth;

            var railLeft = ObjectPoolManager.Instance.SpawnFromPool("Rail", currentLeftPosition, railRotation);
            railLeft.SetActive(true);
            _rails.Add(railLeft);

            currentLeftPosition += moveDirection * _railWidth;
        }
    }

    public float CalculateDespawnTime()
    {
        return despawnDistance / moveSpeed;
    }
}

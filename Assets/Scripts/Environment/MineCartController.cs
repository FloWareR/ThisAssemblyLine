using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;

public class MineCartController : MonoBehaviour
{
    [SerializeField] private List<Transform> railSpawnList;

    [SerializeField] private int objectPerSide;

    [SerializeField] private Quaternion defaultRotation = Quaternion.Euler(0f, 90f, 0f);
    private void Start()
    {
        StartCoroutine(SpawnRailsCoroutine());
    }

    private void Update()
    {

    }

    IEnumerator SpawnRailsCoroutine()
    {
        var currentRightPosition = railSpawnList[0].position;
        var currentLeftPosition = railSpawnList[1].position;

        for (int i = 0; i < objectPerSide; i++)
        {
            var railRight = ObjectPoolManager.Instance.SpawnFromPool("Rail", currentRightPosition, defaultRotation);
            var railRightCollider = railRight.GetComponent<Collider>();
            var railWidth = railRightCollider.bounds.size.x;

            currentRightPosition = railRight.transform.position + new Vector3(0f, 0f, railWidth);

            var railLeft = ObjectPoolManager.Instance.SpawnFromPool("Rail", currentLeftPosition, defaultRotation);
            currentLeftPosition = railLeft.transform.position + new Vector3(0f, 0f, railWidth);
        }
        yield return new WaitForSeconds(1);
        while (true)
        {

            yield return new WaitForSeconds(1);
        }
    }
}

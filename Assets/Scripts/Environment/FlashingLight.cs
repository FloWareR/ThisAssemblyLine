using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashingLight : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectsToControl;
    [SerializeField] private float flashTime = 1f;

    private Coroutine controlCoroutine;
    public void StartEnableDisableCycle()
    {
        if (controlCoroutine != null)
            StopCoroutine(controlCoroutine);

        controlCoroutine = StartCoroutine(EnableDisableCycle());
    }

    public void StopEnableDisableCycle()
    {
        if (controlCoroutine != null)
            StopCoroutine(controlCoroutine);
    }

    private IEnumerator EnableDisableCycle()
    {
        while (true)
        {
            foreach (GameObject obj in objectsToControl)
            {
                obj.SetActive(false);
                yield return new WaitForSeconds(flashTime);
                obj.SetActive(true);
            }

        }
    }
}

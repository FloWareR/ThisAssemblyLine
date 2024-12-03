using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ScoreBoardLights : MonoBehaviour
{
    [SerializeField] private List<GameObject> lightsToControl;
    [SerializeField] public float alertDuration = 2f;
    [SerializeField] public float alertSpeed = 2f;
    private Coroutine controlCoroutine;

    private void Update()
    {
        foreach (GameObject light in lightsToControl)
        {
            light.transform.rotation *= Quaternion.Euler(0f, alertSpeed, 0f);
        }
    }
    public void StartAlertCycle()
    {
        if (controlCoroutine != null)
            StopCoroutine(controlCoroutine);

        controlCoroutine = StartCoroutine(AlertCycle());
    }

    public void StopAlertCycle()
    {
        if (controlCoroutine != null)
            StopCoroutine(controlCoroutine);
    }

    private IEnumerator AlertCycle()
    {
        foreach (GameObject light in lightsToControl)
        {
            light.SetActive(false);
            yield return new WaitForSeconds(alertDuration);
            light.SetActive(true);
        }
    }
}
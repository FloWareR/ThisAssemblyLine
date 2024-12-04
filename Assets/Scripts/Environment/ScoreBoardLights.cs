using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;
using System;
using ScriptableObjects;

public class ScoreBoardLights : MonoBehaviour
{
    [SerializeField] private List<GameObject> lightsToControl;
    [SerializeField] private List<ScoreLightCoroutine> scoreLightCoroutines;

    [SerializeField] public float goodDuration = 2f;
    [SerializeField] public float warningDuration = 2f;
    [SerializeField] public float alertDuration = 2f;
    [SerializeField] public float alertLightSpeed = 4f;
    [SerializeField] public float LevelCompletedLightFlashTime = 1f;

    private Coroutine controlCoroutine;

    private void OnEnable()
    {
        GameManager.LevelDone += StartLevelCompletedCycle;
        GameManager.LoadNewLevel += StopLevelCompletedCycle;
    }

    private void OnDisable()
    {
        GameManager.LevelDone -= StartLevelCompletedCycle;
        GameManager.LoadNewLevel -= StopLevelCompletedCycle;
    }

    private void StartLevelCompletedCycle()
    {
        StartCycle(LightCycleType.LevelCompleted);

    }
    private void StopLevelCompletedCycle(LevelData data)
    {
        StopCycle();
    }

    public void StartCycle(LightCycleType cycleType)
    {
        if (controlCoroutine != null)
            StopCoroutine(controlCoroutine);

        switch (cycleType)
        {
            case LightCycleType.Good:
                controlCoroutine = StartCoroutine(LightGood());
                break;
            case LightCycleType.Warning:
                controlCoroutine = StartCoroutine(LightWarning());
                break;
            case LightCycleType.Alert:
                controlCoroutine = StartCoroutine(LightAlert());
                break;
            case LightCycleType.LevelCompleted:
                controlCoroutine = StartCoroutine(LightLevelCompleted());
                break;
        }
    }

    public void StopCycle()
    {
        if (controlCoroutine != null)
        {
            StopCoroutine(controlCoroutine);
            controlCoroutine = null;
        }
    }
    private IEnumerator LightLevelCompleted()
    {
        int lightLimit = 0;
        while (true)
        {
            foreach (GameObject light in lightsToControl)
            {
                if (lightLimit > 1)
                {
                    lightLimit = 0;
                    break;
                }

                Light lightColor = light.GetComponent<Light>();
                lightColor.color = new Color(0f / 255f, 150f / 255f, 255f / 255f);
                light.SetActive(true);
                lightLimit += 1;
            }

            yield return new WaitForSeconds(LevelCompletedLightFlashTime);

            foreach (GameObject light in lightsToControl)
            {
                if (lightLimit > 1)
                {
                    lightLimit = 0;
                    break;
                }

                light.SetActive(false);
                Light lightColor = light.GetComponent<Light>();
                lightColor.color = Color.white;

                lightLimit += 1;
            }

            yield return new WaitForSeconds(LevelCompletedLightFlashTime);

            yield return null;
        }
    }

    private IEnumerator AlertCycle()
    {
        foreach (GameObject light in lightsToControl)
        {
            Light lightColor = light.GetComponent<Light>();
            lightColor.color = Color.red;
            light.SetActive(true);
        }
        yield return new WaitForSeconds(alertDuration);
        foreach (GameObject light in lightsToControl)
        {
            light.SetActive(false);
            Light lightColor = light.GetComponent<Light>();
            lightColor.color = Color.white;
        }
        StopCycle();
    }

    private IEnumerator LightAlert()
    {
        StartCoroutine(AlertCycle());
        while (true)
        {
            foreach (GameObject light in lightsToControl)
            {
                light.transform.rotation *= Quaternion.Euler(0f, alertLightSpeed, 0f);
            }
            yield return null;
        }
    }

    private IEnumerator LightWarning()
    {
        Light light = lightsToControl[1].GetComponent<Light>();
        light.type = LightType.Point;
        light.color = Color.yellow;
        lightsToControl[1].SetActive(true);

        yield return new WaitForSeconds(warningDuration);

        lightsToControl[1].SetActive(false);
        light.type = LightType.Spot;
        light.color = Color.white;

        StopCycle();
    }

    private IEnumerator LightGood()
    {
        Light light = lightsToControl[0].GetComponent<Light>();
        light.type = LightType.Point;
        light.color = Color.green;
        lightsToControl[0].SetActive(true);

        yield return new WaitForSeconds(goodDuration);

        lightsToControl[0].SetActive(false);
        light.type = LightType.Spot;
        light.color = Color.white;

        StopCycle();
    }

    public void EvaluateScore(int currentScore)
    {
        foreach (var scoreLightCoroutine in scoreLightCoroutines)
        {
            if (currentScore >= scoreLightCoroutine.scoreThreshold)
            {
                StartCycle(scoreLightCoroutine.cycleType);
                break;
            }
        }
    }
}

[System.Serializable]
public class ScoreLightCoroutine
{
    public int scoreThreshold;
    public LightCycleType cycleType;
}

public enum LightCycleType
{
    Good,
    Warning,
    Alert,
    LevelCompleted
}
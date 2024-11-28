using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField] private FlashingLight enableDisableObjects;
    [SerializeField] private bool autoStart = true;

    private void Start()
    {
        if (autoStart)
        {
            StartCycle();
        }
    }

    // Public method to start the enable/disable cycle
    public void StartCycle()
    {
        if (enableDisableObjects != null)
        {
            enableDisableObjects.StartEnableDisableCycle();
        }
        else
        {
            Debug.LogWarning("EnableDisableObjects reference is missing.");
        }
    }
    public void StopCycle()
    {
        if (enableDisableObjects != null)
        {
            enableDisableObjects.StopEnableDisableCycle();
        }
        else
        {
            Debug.LogWarning("EnableDisableObjects reference is missing.");
        }
    }
    public void RestartCycle()
    {
        StopCycle();
        StartCycle();
    }
}

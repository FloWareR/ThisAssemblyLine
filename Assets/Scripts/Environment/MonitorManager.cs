using System.Collections.Generic;
using UnityEngine;
using Global;
using ScriptableObjects;

namespace Environment
{
    public class MonitorManager : MonoBehaviour
    {
        [SerializeField] private string monitorTag;
        [SerializeField] private List<Transform> monitorSpawnPoints; // List of spawn points
        [SerializeField] private List<GameObject> monitorList = new List<GameObject>();
        
        public static MonitorManager Instance;

        private int currentSpawnIndex = 0; // To track which spawn point to use next

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);
        }

        private void OnEnable()
        {
            GameManager.LoadNewLevel += OnLoadNewLevel;
            GameManager.LevelDone += OnLevelDone; // Subscribe to LevelDone event
        }

        private void OnDisable()
        {
            GameManager.LoadNewLevel -= OnLoadNewLevel;
            GameManager.LevelDone -= OnLevelDone; // Unsubscribe to LevelDone event
        }

        private void OnLoadNewLevel(LevelData obj)
        {
            ClearMonitors();
            foreach (var objectToBuild in obj.objectsToBuild)
            {
                SpawnMonitor(objectToBuild.objectData.objectImage, objectToBuild.quantity);
            }
        }

        public void SpawnMonitor(Sprite objectImage, int quantity)
        {
            // Ensure we have enough spawn points
            if (monitorSpawnPoints.Count == 0)
            {
                Debug.LogError("No monitor spawn points set!");
                return;
            }
            Transform spawnPoint = monitorSpawnPoints[currentSpawnIndex];
            GameObject newMonitor = ObjectPoolManager.Instance.SpawnFromPool(
                monitorTag,
                spawnPoint.position,
                spawnPoint.rotation
            );

            // Update the monitor with the image and quantity
            newMonitor.GetComponent<MonitorController>().UpdateMonitorInfo(objectImage, quantity);

            // Add the spawned monitor to the list
            monitorList.Add(newMonitor);

            // Update the spawn index for the next monitor
            currentSpawnIndex = (currentSpawnIndex + 1) % monitorSpawnPoints.Count;
        }

        // Method to clear all monitors
        private void ClearMonitors()
        {
            foreach (var monitor in monitorList)
            {
                ObjectPoolManager.Instance.ReturnToPool(monitor.name, monitor); // Return monitor to pool
            }
            monitorList.Clear(); // Clear the list
        }

        // Method called when Level is done (to return all monitors to pool)
        private void OnLevelDone()
        {
            Debug.Log("Level done! Returning all monitors to pool.");
            ClearMonitors(); // Clear and return all monitors to pool
        }
    }
}

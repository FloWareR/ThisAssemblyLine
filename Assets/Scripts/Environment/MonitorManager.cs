using System.Collections.Generic;
using System.Linq;
using Global;
using UnityEngine;

namespace Environment
{
    public class MonitorManager : MonoBehaviour
    {
        [SerializeField] private string monitorTag;
        [SerializeField] private Transform monitorSpawnPoint;

        private List<GameObject> _monitorList = new List<GameObject>();
        
        public static MonitorManager Instance;
        

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);
        }
        
        public void SpawnMonitor(Sprite objectImage, int quantity)
        {
           var newMonitor =  ObjectPoolManager.Instance.SpawnFromPool(
               monitorTag, 
               monitorSpawnPoint.position, 
               monitorSpawnPoint.rotation);
           
           newMonitor.GetComponent<MonitorController>().UpdateMonitorInfo(objectImage, quantity);
        }
    }
}

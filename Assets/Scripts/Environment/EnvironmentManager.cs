using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Global;
using UnityEngine;

namespace Environment
{
    public class EnvironmentManager : MonoBehaviour
    {
        [SerializeField] private FlashingLight[] enableDisableFlashes;
        [SerializeField] private Transform[] MinecartSpawnPoints;
        [SerializeField] private List<Transform> availableSpawnPoints;
        [SerializeField] private float spawnRate = 5f;
        [SerializeField] private bool autoStart = true;

        private List<(GameObject MineCart, Transform SpawnPoint)> spawnedMineCarts = new List<(GameObject, Transform)>();

        private void Start()
        {
            availableSpawnPoints = new List<Transform>(MinecartSpawnPoints);

            if (autoStart)
            {
                StartCycle();
            }
        }

        public void StartCycle()
        {
            if (enableDisableFlashes != null)
            {
                foreach (FlashingLight flashObject in enableDisableFlashes)
                {
                    flashObject.StartEnableDisableCycle();
                }
            }
            else
            {
                Debug.LogWarning("EnableDisableFlashes reference is missing.");
            }

            if (MinecartSpawnPoints != null)
            {
                StartCoroutine(SpawnMineCarts());
            }
            else
            {
                Debug.LogWarning("MinecartSpawnPoints reference is missing.");
            }
        }

        public void StopCycle()
        {
            if (enableDisableFlashes != null)
            {
                foreach (FlashingLight flashObject in enableDisableFlashes)
                {
                    flashObject.StopEnableDisableCycle();
                }
            }
            else
            {
                Debug.LogWarning("EnableDisableFlashes reference is missing.");
            }
        }

        public void RestartCycle()
        {
            StopCycle();
            StartCycle();
        }

        private IEnumerator SpawnMineCarts()
        {
            while (true)
            {
                yield return new WaitUntil(() => availableSpawnPoints.Count > 0);
                int randomIndex = Random.Range(0, availableSpawnPoints.Count);
                Transform spawnPosition = availableSpawnPoints[randomIndex];

                var mineCart = ObjectPoolManager.Instance.SpawnFromPool("MineCart", spawnPosition.position, spawnPosition.rotation);
                spawnedMineCarts.Add((mineCart, spawnPosition));

                availableSpawnPoints.Remove(spawnPosition);

                MineCartController mineCartController = mineCart.GetComponent<MineCartController>();
                if (mineCartController != null)
                {
                    float despawnTime = mineCartController.CalculateDespawnTime();
                    StartCoroutine(ReturnMineCartToPool(mineCart, despawnTime));
                }
                yield return new WaitForSeconds(spawnRate);
            }
        }

        private IEnumerator ReturnMineCartToPool(GameObject mineCart, float despawnTime)
        {
            yield return new WaitForSeconds(despawnTime);
            var mineCartEntry = spawnedMineCarts.FirstOrDefault(item => item.MineCart == mineCart);
            if (mineCartEntry.MineCart != null)
            {
                availableSpawnPoints.Add(mineCartEntry.SpawnPoint);
                spawnedMineCarts.RemoveAll(item => item.MineCart == mineCart);
            }
            ObjectPoolManager.Instance.ReturnToPool("MineCart", mineCart);

        }
    }
}

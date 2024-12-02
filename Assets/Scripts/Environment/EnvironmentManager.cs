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
                    mineCartController.InitializeCart();
                    float despawnTime = mineCartController.CalculateDespawnTime();
                    List<GameObject> _rails = mineCartController._rails;

                    StartCoroutine(ReturnMineCartToPool(mineCart, despawnTime, _rails));
                }
                yield return new WaitForSeconds(spawnRate);
            }
        }

        private IEnumerator ReturnMineCartToPool(GameObject mineCart, float despawnTime, List<GameObject> _rails)
        {
            yield return new WaitForSeconds(despawnTime);

            // Return the rails to the pool
            foreach (GameObject rail in _rails)
            {
                rail.SetActive(false); // Deactivate before returning to the pool
                ObjectPoolManager.Instance.ReturnToPool("Rail", rail);
            }

            // Return the minecart to the pool
            ObjectPoolManager.Instance.ReturnToPool("MineCart", mineCart);

            // Re-add the spawn point to available list
            var mineCartEntry = spawnedMineCarts.FirstOrDefault(item => item.MineCart == mineCart);
            if (mineCartEntry.MineCart != null)
            {
                availableSpawnPoints.Add(mineCartEntry.SpawnPoint);
                spawnedMineCarts.RemoveAll(item => item.MineCart == mineCart);
            }
        }
    }
}

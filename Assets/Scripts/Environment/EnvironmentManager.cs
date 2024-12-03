using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Global;
using UnityEngine;
using UnityEngine.Serialization;

namespace Environment
{
    public class EnvironmentManager : MonoBehaviour
    {
        [SerializeField] private FlashingLight[] enableDisableFlashes;
        [FormerlySerializedAs("MinecartSpawnPoints")] [SerializeField] private Transform[] minecartSpawnPoints;
        [SerializeField] private List<Transform> availableSpawnPoints;
        [SerializeField] private float spawnRate = 5f;
        [SerializeField] private bool autoStart = true;

        private readonly List<(GameObject MineCart, Transform SpawnPoint)> _spawnedMineCarts = new List<(GameObject, Transform)>();

        private void Start()
        {
            availableSpawnPoints = new List<Transform>(minecartSpawnPoints);

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

            if (minecartSpawnPoints != null)
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
                var randomIndex = Random.Range(0, availableSpawnPoints.Count);
                var spawnPosition = availableSpawnPoints[randomIndex];

                var mineCart = ObjectPoolManager.Instance.SpawnFromPool("MineCart", spawnPosition.position, spawnPosition.rotation);
                _spawnedMineCarts.Add((mineCart, spawnPosition));

                availableSpawnPoints.Remove(spawnPosition);

                var mineCartController = mineCart.GetComponent<MineCartController>();
                if (mineCartController)
                {
                    mineCartController.InitializeCart();
                    var despawnTime = mineCartController.CalculateDespawnTime();
                    var rails = mineCartController._rails;

                    StartCoroutine(ReturnMineCartToPool(mineCart, despawnTime, rails));
                }
                yield return new WaitForSeconds(spawnRate);
            }
        }

        private IEnumerator ReturnMineCartToPool(GameObject mineCart, float despawnTime, List<GameObject> _rails)
        {
            yield return new WaitForSeconds(despawnTime);

            foreach (GameObject rail in _rails)
            {
                rail.SetActive(false); 
                ObjectPoolManager.Instance.ReturnToPool("Rail", rail);
            }

            ObjectPoolManager.Instance.ReturnToPool("MineCart", mineCart);

            var mineCartEntry = _spawnedMineCarts.FirstOrDefault(item => item.MineCart == mineCart);
            if (!mineCartEntry.MineCart) yield break;
            {
                availableSpawnPoints.Add(mineCartEntry.SpawnPoint);
                _spawnedMineCarts.RemoveAll(item => item.MineCart == mineCart);
            }
        }
    }
}

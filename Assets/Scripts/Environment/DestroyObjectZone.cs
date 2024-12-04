using UnityEngine;
using Global;

namespace Environment
{
    public class DestroyObjectZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.tag);
            if (other.CompareTag("Player")) return;
            ObjectPoolManager.Instance.ReturnToPool(other.name, other.gameObject);
        }
    }
}

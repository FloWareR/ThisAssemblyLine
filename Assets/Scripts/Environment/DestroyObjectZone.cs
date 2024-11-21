using UnityEngine;
using Global;

namespace Environment
{
    public class DestroyObjectZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            ObjectPoolManager.Instance.ReturnToPool(other.name, other.gameObject);
        }
    }
}

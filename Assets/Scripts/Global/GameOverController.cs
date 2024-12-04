using System;
using UnityEngine;

namespace Global
{
    public class GameOverController : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            SceneManager.Instance.LoadScene(0);
        }
    }
}

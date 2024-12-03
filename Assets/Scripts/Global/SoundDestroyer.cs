using System.Collections;
using UnityEngine;

namespace Global
{
    public class SoundDestroyer : MonoBehaviour
    {
        private AudioSource _audioSource;
        private float _clipLength;
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private IEnumerator Start()
        {
            _clipLength = _audioSource.clip.length;
            yield return new WaitForSeconds(_clipLength);
            Destroy(gameObject);
        }
    }
}
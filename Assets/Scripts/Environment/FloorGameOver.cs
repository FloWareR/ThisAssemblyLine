using UnityEngine;
using Global;
using System;

public class FloorGameOver : MonoBehaviour
{
    [SerializeField] private AudioClip floorAudio;

    private void OnEnable()
    {
        GameManager.LevelTimeUp += animationTrigger;
    }
    private void OnDisable()
    {
        GameManager.LevelTimeUp -= animationTrigger;
    }

    private void animationTrigger()
    {
        Animator animation = gameObject.GetComponent<Animator>();
        SoundEffectChannel.PlaySoundFxClip(floorAudio, transform.position, 1f);
        animation.SetTrigger("GameOver");
    }
}

using UnityEngine;

[CreateAssetMenu(menuName = "Managers/Sound Effect Channel", fileName = "SoundEffectChannel")]
public class SoundEffectChannel : ScriptableObject
{
    private static SoundEffectChannel _instance;
    public static SoundEffectChannel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<SoundEffectChannel>($"SoundEffectChannel");
            }

            return _instance;
        }
    }
    public AudioSource soundObject;

    private const float VolumeChangeMultiplier = 0.15f;
    private const float PitchChangeMultiplier = 0.1f;

    public static void PlaySoundFxClip(AudioClip clip, Vector3 soundPosition, float volume)
    {
        var randVolume = Random.Range(volume - VolumeChangeMultiplier,  volume + VolumeChangeMultiplier);
        var randPitch = Random.Range(volume - PitchChangeMultiplier,  volume + PitchChangeMultiplier);

        var audio = Instantiate(Instance.soundObject, soundPosition, Quaternion.identity);
        audio.clip = clip;
        audio.volume = randVolume;
        audio.volume = randPitch;
        audio.Play();
    }
        
    public static void PlaySoundFxClip(AudioClip[] clips, Vector3 soundPosition, float volume)
    {
        var randClip = Random.Range(0, clips.Length);
        var randVolume = Random.Range(volume - VolumeChangeMultiplier,  volume + VolumeChangeMultiplier);
        var randPitch = Random.Range(volume - PitchChangeMultiplier,  volume + PitchChangeMultiplier);

        var audio = Instantiate(Instance.soundObject, soundPosition, Quaternion.identity);
        audio.clip = clips[randClip];
        audio.volume = randVolume;
        audio.volume = randPitch;
        audio.Play();
    }
    
}
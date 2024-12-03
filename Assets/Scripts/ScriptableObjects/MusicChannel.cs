using UnityEngine;

[CreateAssetMenu(menuName = "Managers/Music Channel Channel", fileName = "MusicChannel")]
public class MusicChannel : ScriptableObject
{
    private static MusicChannel _instance;
    public static MusicChannel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<MusicChannel>($"MusicChannel");
            }

            return _instance;
        }
    }
    public AudioSource musicObject;
    
    public static void PlayerSoundTrack(AudioClip[] clips, float volume)
    {
        var randClip = Random.Range(0, clips.Length);
        Debug.Log(randClip);
        var audio = Instantiate(Instance.musicObject);
        audio.volume = volume;
        audio.clip = clips[randClip];
        audio.Play();
    }
    
}
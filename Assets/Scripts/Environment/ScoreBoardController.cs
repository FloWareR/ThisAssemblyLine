using Global;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Environment
{
    public class ScoreBoardController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI scoreCounter;
        [SerializeField] private TextMeshProUGUI prevObjectCounter;

        [Header("Audio Clips")]
        [SerializeField] private List<ScoreSoundPair> scoreSoundPairs;
        private Dictionary<int, AudioClip> _scoreAudioClipDict;

        private void Awake()
        {
            _scoreAudioClipDict = new Dictionary<int, AudioClip>();
            foreach (var pair in scoreSoundPairs)
            {
                _scoreAudioClipDict[pair.scoreThreshold] = pair.audioClip;
            }
            scoreCounter.text = GameManager.instance.currentScore.ToString();
            prevObjectCounter.text = GameManager.instance.previousObjectScore.ToString();
        }

        public void UpdateScores()
        {
            scoreCounter.text = GameManager.instance.currentScore.ToString();
            prevObjectCounter.text = GameManager.instance.previousObjectScore.ToString();
            PlayScoreSound(GameManager.instance.previousObjectScore);
            gameObject.GetComponent<ScoreBoardLights>().EvaluateScore(GameManager.instance.previousObjectScore);
        }

        private void PlayScoreSound(int previousScore)
        {
            var clipToPlay = GetScoreSoundClip(previousScore);

            if (clipToPlay != null)
            {
                SoundEffectChannel.PlaySoundFxClip(clipToPlay, transform.position, 1f);
            }
        }

        private AudioClip GetScoreSoundClip(int score)
        {
            return (from pair in _scoreAudioClipDict where score >= pair.Key select pair.Value).FirstOrDefault();
        }
    }

    [System.Serializable]
    public class ScoreSoundPair
    {
        public int scoreThreshold;
        public AudioClip audioClip;
    }
}

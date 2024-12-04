using System;
using Global;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects;
using UnityEngine.UI;

namespace Environment
{
    public class ScoreBoardController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI scoreCounter;
        [SerializeField] private TextMeshProUGUI prevObjectCounter;
        [SerializeField] private TextMeshProUGUI levelCounter;
        [SerializeField] private TextMeshProUGUI finalScore;
        [SerializeField] private Image objectImage;


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
        }
        
        private void OnEnable()
        {
            GameManager.LoadNewLevel += OnLoadNewLevel;
            GameManager.LevelTimeUp += OnTimeUp;
            GameManager.LevelDone += OnLevelDone;
        }
        
        private void OnDisable()
        {
            GameManager.LoadNewLevel -= OnLoadNewLevel;
            GameManager.LevelTimeUp -= OnTimeUp;
            GameManager.LevelDone -= OnLevelDone;
        }

        private void OnLoadNewLevel(LevelData obj)
        {
            Debug.Log(obj.levelNumber);
            levelCounter.text = (obj.levelNumber).ToString();
        }

        private void OnTimeUp()
        {
            PlayScoreSound(-100);
            objectImage.color = Color.black;
            finalScore.gameObject.transform.parent.gameObject.SetActive(true);
            finalScore.gameObject.SetActive(true);
            finalScore.text = GameManager.instance.currentScore.ToString();
            scoreCounter.gameObject.transform.parent.gameObject.SetActive(false);
            levelCounter.gameObject.transform.parent.gameObject.SetActive(false);
            prevObjectCounter.gameObject.transform.parent.gameObject.SetActive(false);
        }
        
        private void OnLevelDone()
        {
            PlayScoreSound(-200);
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

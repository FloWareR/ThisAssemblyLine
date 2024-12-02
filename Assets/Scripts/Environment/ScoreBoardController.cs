using Global;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Environment
{
    public class ScoreBoardController : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI scoreCounter;
        [SerializeField] private TextMeshProUGUI prevObjectCounter;

        private void Start()
        {
            scoreCounter.text = GameManager.instance.currentScore.ToString();
            prevObjectCounter.text = GameManager.instance.previousObjectScore.ToString();
        }


        public void UpdateScores()
        {
            scoreCounter.text = GameManager.instance.currentScore.ToString();
            prevObjectCounter.text = GameManager.instance.previousObjectScore.ToString();
        }
    }
}

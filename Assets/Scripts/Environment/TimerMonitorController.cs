using System.Collections;
using Global;
using ScriptableObjects;
using TMPro;
using UnityEngine;


namespace Environment
{
    public class TimerMonitorController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;

        private void OnEnable()
        {
            GameManager.LoadNewLevel += StartTimer;
        }

        public void UpdateText(int timeLeft)
        {
            timerText.text = $"{timeLeft}s";
        }
        
        public void StartTimer(LevelData totalTime)
        {
            StartCoroutine(ChangeTimerColor((int)totalTime.timeLimit));
        }

        private IEnumerator ChangeTimerColor(int totalTime)
        {
            Color startColor = new Color32(10, 255, 0, 255); 
            Color endColor = new Color32(255, 0, 0, 255);    
            for (var timeLeft = totalTime; timeLeft >= 0; timeLeft--)
            {
                UpdateText(timeLeft);

                var t = 1f - (float)timeLeft / totalTime;
                timerText.color = Color.Lerp(startColor, endColor, t);

                yield return new WaitForSeconds(1f);
            }
            timerText.color = endColor;
        }
    }
}

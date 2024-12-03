using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Environment
{
    public class MonitorController : MonoBehaviour
    {
        [SerializeField] private Image objectImage;
        [SerializeField] private TextMeshProUGUI objectQty;

        
        public void UpdateMonitorInfo(Sprite newImage, int newQty)
        {
            objectImage.sprite = newImage;
            objectQty.text = $"x{newQty}";
        }
    }
}

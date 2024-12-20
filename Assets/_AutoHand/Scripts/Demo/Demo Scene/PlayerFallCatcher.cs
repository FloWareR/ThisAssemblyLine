using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Autohand.Demo{
    public class PlayerFallCatcher : MonoBehaviour{
        Vector3 startPos;

        void Start(){
            if(AutoHandPlayer.Instance != null) {
                startPos = AutoHandPlayer.Instance.transform.position;
                if(!SceneManager.GetActiveScene().name.ToLower().Contains("demo"))
                    enabled = false;
            }
        }
        
        void FixedUpdate() {
            if(AutoHandPlayer.Instance != null) {
                if(AutoHandPlayer.Instance.transform.position.y < -10f) {
                    AutoHandPlayer.Instance.SetPosition(startPos);
                }
            }
        }
    }
}

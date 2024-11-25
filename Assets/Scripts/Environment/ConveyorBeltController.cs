using System.Collections;
using Global;
using ScriptableObjects;
using UnityEngine;

namespace Environment
{
    public class ConveyorBeltController : MonoBehaviour
    {
        
        [Tooltip("In Meters/Seconds (doesnt apply to main belt")] public float speed;
        [SerializeField] private bool mainBelt;
        [SerializeField] private LayerMask ignore;
        public bool active = true;
        private float _trueSpeed;
        private float _time;
        
        public LevelData currentLevel;

        //References
        private ConveyorBeltController _conveyor;
        private Renderer _conveyorRenderer;
    
    
        private static readonly int Speed = Shader.PropertyToID("_Speed");
        private static readonly int CurrentTime = Shader.PropertyToID("_currentTime");

        private void Start()
        {
            _conveyorRenderer = GetComponent<Renderer>();
        }
        private void OnEnable()
        {
            GameManager.LoadNewLevel += OnLoadNewLevel;
        }

        private void OnLoadNewLevel(LevelData obj)
        {
            StartNewLevel(obj);
        }

        private void OnDisable()
        {
            GameManager.LoadNewLevel -= OnLoadNewLevel;
        }
        
        public void StartNewLevel(LevelData newLevel)
        {
            currentLevel = newLevel;
        }
        
        private void Update()
        {
            if (!currentLevel) return;
            var effectiveSpeed = mainBelt ? currentLevel.conveyorSpeed : speed;
            _trueSpeed =  effectiveSpeed;
            if (!active)
            {
                _conveyorRenderer.material.SetFloat(CurrentTime, _time);
                return;
            }
            _time += Time.deltaTime;
            _conveyorRenderer.material.SetFloat(CurrentTime, _time);
            _conveyorRenderer.material.SetFloat(Speed, effectiveSpeed); 
        }
        
        private void OnCollisionStay(Collision collision)
        {
            if ((ignore.value & (1 << collision.gameObject.layer)) != 0) return;            
            var attachedRigidbody = collision.collider.attachedRigidbody;
            if (attachedRigidbody == null || attachedRigidbody.isKinematic) return;
            var conveyorDirection = transform.forward;
            var movement = conveyorDirection * _trueSpeed * Time.deltaTime; 
            attachedRigidbody.MovePosition(attachedRigidbody.position + movement);
        }
        
        public void StopAllMovement()
        {
            active = false;
        }
        
        public void ResumeAllMovement()
        {
            active = true;
        }
    }
}
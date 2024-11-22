using System.Collections;
using ScriptableObjects;
using UnityEngine;

namespace Environment
{
    public class ConveyorBeltController : MonoBehaviour
    {
        
        [Tooltip("In Meters/Seconds (doesnt apply to main belt")] public float speed;
        [Tooltip("In Seconds")] public float moveInterval;
        [Tooltip("In Meters")] public float moveDistance;
        [SerializeField] private bool mainBelt;
        [SerializeField] private LayerMask ignore;
        public bool active = true;
        private bool _isMoving;
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
            StartCoroutine(MoveBelt());
        }
        
        private void Update()
        {
            var effectiveSpeed = mainBelt ? currentLevel.conveyorSpeed : speed;
            _trueSpeed = _isMoving ? effectiveSpeed : 0;
            if (!_isMoving)
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

        private IEnumerator MoveBelt()
        {
            while (active)
            {
                _isMoving = true;
                var effectiveSpeed = mainBelt ? currentLevel.conveyorSpeed : speed;
                if (mainBelt)
                {
                    moveInterval = currentLevel.spawnInterval - moveDistance;
                }
                yield return new WaitForSeconds(moveDistance / effectiveSpeed);
                _isMoving = false;
                yield return new WaitForSeconds(moveInterval);
            }
        }

        public void StopAllMovement()
        {
            active = false;
            _isMoving = false;
        }
        
        public void ResumeAllMovement()
        {
            active = true;
            _isMoving = true;
            StartCoroutine(MoveBelt());
        }
    }
}
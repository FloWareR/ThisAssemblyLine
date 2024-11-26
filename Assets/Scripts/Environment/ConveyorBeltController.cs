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
        [SerializeField] private Vector3 direction = Vector3.right;

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

        private void OnTriggerStay(Collider collision)
        {
            if ((ignore.value & (1 << collision.gameObject.layer)) != 0) return;            

            var attachedRigidbody = collision.GetComponent<Collider>().attachedRigidbody;
            if (attachedRigidbody == null || attachedRigidbody.isKinematic) return;
            var conveyorCenter = GetComponent<Collider>().bounds.center;
            var objectPosition = attachedRigidbody.position;
            var perpendicularDirection = Vector3.Cross(direction, Vector3.up).normalized;
            var offset = Vector3.Project(new Vector3(objectPosition.x - conveyorCenter.x, 0, objectPosition.z - conveyorCenter.z), perpendicularDirection);
            var correctiveForce = -offset * .5f * Time.deltaTime;
            var movement = direction.normalized * _trueSpeed * Time.deltaTime;
            var finalMovement = movement + correctiveForce;
            attachedRigidbody.MovePosition(objectPosition + finalMovement);
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
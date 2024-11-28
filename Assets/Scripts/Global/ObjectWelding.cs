using System;
using UnityEngine;
using System.Collections;

namespace Global
{
    public class ObjectWelding : MonoBehaviour
    {
        [Header("Joint Settings")]
        public float defaultBreakForce = 700f;
        public float defaultBreakTorque = 500f;

        [Header("Break Settings")]
        public float jointBreakDelay = 0.5f;

        public bool isWelded;
        public LayerMask weldableLayer;

        private Rigidbody _rigidbody;

        [Header("VR Holding Settings")]
        public bool isHeld;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnDisable()
        {
            var joints = GetComponents<Joint>();
            foreach (var joint in joints)
            {
                if (joint != null)
                {
                    Destroy(joint);
                }
            }
            isWelded = false;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!isHeld || ((1 << other.gameObject.layer) & weldableLayer) == 0 || isWelded) return;
            var otherWelding = other.gameObject.GetComponent<ObjectWelding>();
            if (otherWelding != null && otherWelding.isHeld)
            {
                otherWelding.isWelded = true;
            }
            isWelded = true;
            CombineRigidbodies(other.gameObject);
        }

        private void CombineRigidbodies(GameObject other)
        {
            if (GetComponent<FixedJoint>() != null) return;

            var joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = other.GetComponent<Rigidbody>();
            joint.breakForce = defaultBreakForce;
            joint.breakTorque = defaultBreakTorque;
        }

        private void OnJointBreak(float breakForce)
        {
            Debug.Log($"Joint broken with force: {breakForce}");
            StartCoroutine(DelayedHandleJointBreak());
        }

        private IEnumerator DelayedHandleJointBreak()
        {
            yield return new WaitForSeconds(jointBreakDelay);
            isWelded = false;
        }

        public void SetHeldState(bool held)
        {
            isHeld = held;
        }
    }
}
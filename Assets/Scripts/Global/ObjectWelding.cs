using UnityEngine;
using System.Collections;

namespace Global
{
    public class ObjectWelding : MonoBehaviour
    {
        public bool isWelded;

        private void OnCollisionEnter(Collision other)
        {
            Debug.Log(isWelded);
            if (!other.gameObject.CompareTag($"Weldable") || isWelded) return;
            var otherWelding = other.gameObject.GetComponent<ObjectWelding>();
            if (otherWelding != null)
            {
                otherWelding.isWelded = true;
            }

            isWelded = true;
            CombineRigidbodies(other.gameObject);
        }
        
        private void CombineRigidbodies(GameObject other)
        {
            var joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = other.GetComponent<Rigidbody>();

            joint.breakForce = 700f; 
            joint.breakTorque = 500f;
        }
        
        private void OnJointBreak(float breakForce)
        {
            StartCoroutine(DelayedHandleJointBreak());
        }

        private IEnumerator DelayedHandleJointBreak()
        {
            yield return new WaitForSeconds(.5f);
            isWelded = false;
        }
    }
}
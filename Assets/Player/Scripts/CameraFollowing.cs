using System;
using UnityEngine;

namespace Player.Scripts
{
    public class CameraFollowing : MonoBehaviour
    {
        private Vector3 _cameraPosition;
        
        public float cameraRange = 10f;
        
        public Transform target;
        public float moveSmoothingCoefficient = 3f;

        private void Awake()
        {
            if (target == null) Debug.LogError("No target assigned!");
            
            var offset = Mathf.Sqrt(cameraRange * cameraRange / 2);
            _cameraPosition = new Vector3(0, offset, -offset);
        }

        private void LateUpdate()
        {
            MoveToTarget();
        }

        private void MoveToTarget()
        {
            transform.position = Vector3.Lerp(transform.position, target.position + _cameraPosition, moveSmoothingCoefficient * Time.deltaTime);
        }
    }
}

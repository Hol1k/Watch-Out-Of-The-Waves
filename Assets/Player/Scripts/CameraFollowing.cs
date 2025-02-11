using UnityEngine;

namespace Player.Scripts
{
    public sealed class CameraFollowing : MonoBehaviour
    {
        private Vector3 _cameraPosition;
        private float _currCameraRange;
        private bool _changeRangeRequested = true;
        
        public float cameraRange = 10f;
        
        public Transform target;
        [Min(0f)] public float moveSmoothingCoefficient = 0.5f;

        private void Awake()
        {
            if (target == null) Debug.LogError("No target assigned!");
            
            ChangeCameraPosition();
        }

        public void FixedUpdate()
        {
            ChangeCameraPosition();
        }

        public void Update()
        {
            CheckCameraRangeChange();
        }

        private void LateUpdate()
        {
            MoveToTarget();
        }

        private void MoveToTarget()
        {
            transform.position = Vector3.Lerp(transform.position, target.position + _cameraPosition, 1 / moveSmoothingCoefficient * Time.deltaTime);
        }

        private void CheckCameraRangeChange()
        {
            if (Mathf.Approximately(_currCameraRange, cameraRange))
                _changeRangeRequested = true;
        }

        private void ChangeCameraPosition()
        {
            if (_changeRangeRequested)
            {
                _currCameraRange = cameraRange;
                
                var offset = Mathf.Sqrt(_currCameraRange * _currCameraRange / 2);
                _cameraPosition = new Vector3(0, offset, -offset);
            }
        }
    }
}

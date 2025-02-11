using UnityEngine;

namespace Player.Scripts
{
    public class CameraFollowing : MonoBehaviour
    {
        private Vector3 _cameraPosition;
        private float _currRange;
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
            if (Mathf.Approximately(_currRange, cameraRange))
                _changeRangeRequested = true;
        }

        private void ChangeCameraPosition()
        {
            if (_changeRangeRequested)
            {
                _currRange = cameraRange;
                
                var offset = Mathf.Sqrt(_currRange * _currRange / 2);
                _cameraPosition = new Vector3(0, offset, -offset);
            }
        }
    }
}

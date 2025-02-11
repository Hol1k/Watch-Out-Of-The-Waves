using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Scripts
{
    public sealed class LookAtCursor : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        private InputAction _mouseLookAction;

        private void Awake()
        {
            _mouseLookAction = InputSystem.actions.FindAction("Look");
        }

        private void FixedUpdate()
        {
            LookAtPointer();
        }

        private void LookAtPointer()
        {
            var ray = cam.ScreenPointToRay(_mouseLookAction.ReadValue<Vector2>());
            
            if (!Physics.Raycast(ray, out var hit)) return;
            
            transform.LookAt(hit.point);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts.Scenes.GameScene
{
    public class CameraBehaviour : MonoBehaviour
    {
        private void OnEnable()
        {
            var moveAction = InputSystem.actions.FindAction("Move");
            moveAction.performed += OnMove;
            var zoomAction = InputSystem.actions.FindAction("Zoom");
            zoomAction.performed += OnZoom;
        }

        private void OnDisable()
        {
            var moveAction = InputSystem.actions.FindAction("Move");
            moveAction.performed -= OnMove;
            var zoomAction = InputSystem.actions.FindAction("Zoom");
            zoomAction.performed -= OnZoom;
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            var sensivity = Base.Core.Game.Options.ScrollSensivity;
            var moveInput = context.ReadValue<Vector2>();
            var p = transform.position;
            transform.position = new Vector3(p.x + sensivity * moveInput.x, p.y + sensivity * moveInput.y, p.z);
        }


        private void OnZoom(InputAction.CallbackContext context)
        {
            var sensivity = Base.Core.Game.Options.ZoomSensivity;
            var zoomInput = context.ReadValue<Vector2>();
            var p = transform.position;
            transform.position = new Vector3(p.x, p.y, p.z + sensivity * zoomInput.y);
        }
    }
}
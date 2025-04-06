using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts.Scenes.GameScene
{
    public class CameraBehaviour : MonoBehaviour
    {
        [Header("Mobile Settings")]
        [SerializeField] private float touchMoveSensitivity = 0.01f;
        [SerializeField] private float pinchZoomSensitivity = 0.1f;

        [Header("Keyboard Settings")]
        [SerializeField] private float keyboardMoveSpeed = 8f;

        [Header("Mouse Settings")]
        [SerializeField] private float mouseDragSensitivity = 0.01f;
        [SerializeField] private bool invertMouseDrag = true;

        private bool isKeyboardMoving = false;
        private Vector2 keyboardMoveDirection = Vector2.zero;
        private Vector2 lastTouchDelta;
        private float lastTouchDistance;
        private bool isTouching = false;

        // Mouse drag variables
        private bool isMouseDragging = false;
        private Vector3 lastMousePosition;

        private void OnEnable()
        {
            var moveAction = InputSystem.actions.FindAction("Move");
            moveAction.performed += OnMove;
            moveAction.started += OnMoveStarted;
            moveAction.canceled += OnMoveCanceled;
            var zoomAction = InputSystem.actions.FindAction("Zoom");
            zoomAction.performed += OnZoom;
        }

        private void OnDisable()
        {
            var moveAction = InputSystem.actions.FindAction("Move");
            moveAction.performed -= OnMove;
            moveAction.started -= OnMoveStarted;
            moveAction.canceled -= OnMoveCanceled;
            var zoomAction = InputSystem.actions.FindAction("Zoom");
            zoomAction.performed -= OnZoom;
        }

        private void Update()
        {
            // Handle continuous keyboard movement
            if (isKeyboardMoving)
            {
                MoveCamera(keyboardMoveDirection);
            }

            // Handle mouse dragging (only on non-mobile platforms)
            if (!Application.isMobilePlatform)
            {
                HandleMouseDrag();
            }

            // Handle mobile touch input
            if (Input.touchCount > 0)
            {
                HandleTouchInput();
            }
            else if (isTouching)
            {
                // Reset touch state when touches end
                isTouching = false;
            }
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            keyboardMoveDirection = context.ReadValue<Vector2>();
        }

        private void OnMoveStarted(InputAction.CallbackContext context)
        {
            isKeyboardMoving = true;
            keyboardMoveDirection = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            isKeyboardMoving = false;
            keyboardMoveDirection = Vector2.zero;
        }

        private void OnZoom(InputAction.CallbackContext context)
        {
            ZoomCamera(context.ReadValue<Vector2>());
        }

        private void HandleMouseDrag()
        {
            // Ignore mouse interaction over UI elements
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            // Start dragging on middle mouse button down or right mouse button down
            if (Input.GetMouseButtonDown(2) || Input.GetMouseButtonDown(1))
            {
                isMouseDragging = true;
                lastMousePosition = Input.mousePosition;
            }
            // End dragging when button is released
            else if (Input.GetMouseButtonUp(2) || Input.GetMouseButtonUp(1))
            {
                isMouseDragging = false;
            }

            // Handle the actual dragging
            if (isMouseDragging)
            {
                Vector3 currentMousePosition = Input.mousePosition;
                Vector3 mouseDelta = currentMousePosition - lastMousePosition;

                // Apply the movement
                Vector2 moveInput = new Vector2(mouseDelta.x, mouseDelta.y);
                if (invertMouseDrag)
                {
                    moveInput = -moveInput; // Invert for natural "grab and drag" feel
                }

                MoveCamera(moveInput * mouseDragSensitivity);

                // Update last position for next frame
                lastMousePosition = currentMousePosition;
            }
        }

        private void HandleTouchInput()
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return; // Ignore touches over UI elements

            // Handle pinch to zoom with two fingers
            if (Input.touchCount == 2)
            {
                Touch touch0 = Input.GetTouch(0);
                Touch touch1 = Input.GetTouch(1);

                // Calculate current and previous touch positions
                Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
                Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

                // Calculate the distances between touch points
                float prevTouchDistance = Vector2.Distance(touch0PrevPos, touch1PrevPos);
                float currentTouchDistance = Vector2.Distance(touch0.position, touch1.position);

                // Calculate the zoom value based on the change in distance
                float zoomDelta = currentTouchDistance - prevTouchDistance;

                if (isTouching)
                {
                    // Apply zoom (negative value to match mouse wheel behavior)
                    ZoomCamera(new Vector2(0, -zoomDelta * pinchZoomSensitivity));
                }

                lastTouchDistance = currentTouchDistance;
                isTouching = true;
            }
            // Handle camera panning with one finger
            else if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == UnityEngine.TouchPhase.Moved)
                {
                    Vector2 touchDelta = touch.deltaPosition;

                    if (isTouching)
                    {
                        // Invert delta for natural feeling (dragging content)
                        MoveCamera(new Vector2(-touchDelta.x, -touchDelta.y) * touchMoveSensitivity);
                    }

                    lastTouchDelta = touchDelta;
                    isTouching = true;
                }
            }
        }

        private void MoveCamera(Vector2 moveInput)
        {
            var sensitivity = Base.Core.Game.Options.ScrollSensivity;
            var p = transform.position;

            // For keyboard input, use deltaTime to ensure smooth movement regardless of framerate
            if (isKeyboardMoving)
            {
                transform.position = new Vector3(
                    p.x + sensitivity * moveInput.x * keyboardMoveSpeed * Time.deltaTime,
                    p.y + sensitivity * moveInput.y * keyboardMoveSpeed * Time.deltaTime,
                    p.z
                );
            }
            // For touch input, apply directly (as deltaPosition is already frame-independent)
            else
            {
                transform.position = new Vector3(
                    p.x + sensitivity * moveInput.x,
                    p.y + sensitivity * moveInput.y,
                    p.z
                );
            }
        }

        private void ZoomCamera(Vector2 zoomInput)
        {
            var sensitivity = Base.Core.Game.Options.ZoomSensivity;
            var p = transform.position;
            transform.position = new Vector3(p.x, p.y, p.z + sensitivity * zoomInput.y);
        }
    }
}
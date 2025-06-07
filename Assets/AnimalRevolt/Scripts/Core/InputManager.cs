using UnityEngine;
using UnityEngine.InputSystem;

namespace AnimalRevolt.Core
{
    /// <summary>
    /// Input Manager - Quản lý toàn bộ input của game
    /// Sử dụng Input System package
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        [Header("Input Settings")]
        [SerializeField] private float mouseSensitivity = 1f;
        [SerializeField] private bool invertMouseY = false;
        [SerializeField] private float touchSensitivity = 1f;
        
        [Header("Input Actions")]
        [SerializeField] private InputAction attackAction = new InputAction("Attack", InputActionType.Button, "<Mouse>/leftButton");
        [SerializeField] private InputAction blockAction = new InputAction("Block", InputActionType.Button, "<Mouse>/rightButton");
        [SerializeField] private InputAction jumpAction = new InputAction("Jump", InputActionType.Button, "<Keyboard>/space");
        [SerializeField] private InputAction specialAction = new InputAction("Special", InputActionType.Button, "<Keyboard>/e");
        [SerializeField] private InputAction pauseAction = new InputAction("Pause", InputActionType.Button, "<Keyboard>/escape");
        [SerializeField] private InputAction menuAction = new InputAction("Menu", InputActionType.Button, "<Keyboard>/tab");
        
        [Header("Movement Actions")]
        [SerializeField] private InputAction moveAction = new InputAction("Move", InputActionType.Value, "<Keyboard>/wasd");
        [SerializeField] private InputAction lookAction = new InputAction("Look", InputActionType.Value, "<Mouse>/delta");
        
        // Input States
        private Vector2 movementInput;
        private Vector2 lookInput;
        private bool attackPressed;
        private bool blockPressed;
        private bool jumpPressed;
        private bool specialPressed;
        private bool pausePressed;
        
        // Touch Input
        private Vector2 touchStartPos;
        private Vector2 touchCurrentPos;
        private bool isTouching;
        
        // Events
        public System.Action<Vector2> OnMovementInput;
        public System.Action<Vector2> OnLookInput;
        public System.Action OnAttackPressed;
        public System.Action OnAttackReleased;
        public System.Action OnBlockPressed;
        public System.Action OnBlockReleased;
        public System.Action OnJumpPressed;
        public System.Action OnSpecialPressed;
        public System.Action OnPausePressed;
        
        private void Awake()
        {
            LoadInputSettings();
            SetupInputActions();
        }
        
        private void Update()
        {
            HandleInputSystem();
            HandleTouchInput();
        }
        
        /// <summary>
        /// Handle Input System actions
        /// </summary>
        private void HandleInputSystem()
        {
            // Movement
            Vector2 movement = moveAction.ReadValue<Vector2>();
            if (movement != movementInput)
            {
                movementInput = movement;
                OnMovementInput?.Invoke(movementInput);
            }
            
            // Look
            Vector2 lookDelta = lookAction.ReadValue<Vector2>();
            if (invertMouseY) lookDelta.y = -lookDelta.y;
            lookDelta *= mouseSensitivity;
            
            if (lookDelta != Vector2.zero)
            {
                lookInput = lookDelta;
                OnLookInput?.Invoke(lookInput);
            }
            
            // Combat
            if (attackAction.WasPressedThisFrame())
            {
                attackPressed = true;
                OnAttackPressed?.Invoke();
            }
            if (attackAction.WasReleasedThisFrame())
            {
                attackPressed = false;
                OnAttackReleased?.Invoke();
            }
            
            if (blockAction.WasPressedThisFrame())
            {
                blockPressed = true;
                OnBlockPressed?.Invoke();
            }
            if (blockAction.WasReleasedThisFrame())
            {
                blockPressed = false;
                OnBlockReleased?.Invoke();
            }
            
            if (jumpAction.WasPressedThisFrame())
            {
                jumpPressed = true;
                OnJumpPressed?.Invoke();
            }
            
            if (specialAction.WasPressedThisFrame())
            {
                specialPressed = true;
                OnSpecialPressed?.Invoke();
            }
            
            if (pauseAction.WasPressedThisFrame())
            {
                pausePressed = true;
                OnPausePressed?.Invoke();
            }
        }
        
        /// <summary>
        /// Handle touch input cho mobile
        /// </summary>
        private void HandleTouchInput()
        {
            if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
            {
                var touch = Touchscreen.current.touches[0];
                
                switch (touch.phase.ReadValue())
                {
                    case UnityEngine.InputSystem.TouchPhase.Began:
                        touchStartPos = touch.position.ReadValue();
                        isTouching = true;
                        break;
                        
                    case UnityEngine.InputSystem.TouchPhase.Moved:
                        if (isTouching)
                        {
                            touchCurrentPos = touch.position.ReadValue();
                            Vector2 touchDelta = (touchCurrentPos - touchStartPos) * touchSensitivity * Time.deltaTime;
                            OnLookInput?.Invoke(touchDelta);
                        }
                        break;
                        
                    case UnityEngine.InputSystem.TouchPhase.Ended:
                    case UnityEngine.InputSystem.TouchPhase.Canceled:
                        isTouching = false;
                        break;
                }
            }
        }
        
        /// <summary>
        /// Get current movement input
        /// </summary>
        public Vector2 GetMovementInput()
        {
            return movementInput;
        }
        
        /// <summary>
        /// Get current look input
        /// </summary>
        public Vector2 GetLookInput()
        {
            return lookInput;
        }
        
        /// <summary>
        /// Check if attack is pressed
        /// </summary>
        public bool IsAttackPressed()
        {
            return attackPressed;
        }
        
        /// <summary>
        /// Check if block is pressed
        /// </summary>
        public bool IsBlockPressed()
        {
            return blockPressed;
        }
        
        /// <summary>
        /// Set input enabled/disabled
        /// </summary>
        public void SetInputEnabled(bool enabled)
        {
            this.enabled = enabled;
        }
        
        /// <summary>
        /// Load input settings
        /// </summary>
        private void LoadInputSettings()
        {
            mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1f);
            invertMouseY = PlayerPrefs.GetInt("InvertMouseY", 0) == 1;
            touchSensitivity = PlayerPrefs.GetFloat("TouchSensitivity", 1f);
        }
        
        /// <summary>
        /// Save input settings
        /// </summary>
        public void SaveInputSettings()
        {
            PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity);
            PlayerPrefs.SetInt("InvertMouseY", invertMouseY ? 1 : 0);
            PlayerPrefs.SetFloat("TouchSensitivity", touchSensitivity);
            PlayerPrefs.Save();
        }
        
        /// <summary>
        /// Update sensitivity settings
        /// </summary>
        public void SetMouseSensitivity(float sensitivity)
        {
            mouseSensitivity = Mathf.Clamp(sensitivity, 0.1f, 5f);
        }
        
        public void SetTouchSensitivity(float sensitivity)
        {
            touchSensitivity = Mathf.Clamp(sensitivity, 0.1f, 5f);
        }
        
        public void SetInvertMouseY(bool invert)
        {
            invertMouseY = invert;
        }
        
        /// <summary>
        /// Setup Input Actions
        /// </summary>
        private void SetupInputActions()
        {
            attackAction.Enable();
            blockAction.Enable();
            jumpAction.Enable();
            specialAction.Enable();
            pauseAction.Enable();
            menuAction.Enable();
            moveAction.Enable();
            lookAction.Enable();
        }
        
        private void OnDestroy()
        {
            // Dispose Input Actions
            attackAction?.Dispose();
            blockAction?.Dispose();
            jumpAction?.Dispose();
            specialAction?.Dispose();
            pauseAction?.Dispose();
            menuAction?.Dispose();
            moveAction?.Dispose();
            lookAction?.Dispose();
        }
    }
}
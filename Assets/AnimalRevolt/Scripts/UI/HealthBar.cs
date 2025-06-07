using UnityEngine;
using UnityEngine.UI;

namespace AnimalRevolt.UI
{
    /// <summary>
    /// Health Bar UI Component - Hiển thị thanh máu của nhân vật
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Image fillImage;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Text healthText;
        
        [Header("Visual Settings")]
        [SerializeField] private Gradient healthGradient;
        [SerializeField] private bool showHealthText = true;
        [SerializeField] private bool smoothTransition = true;
        [SerializeField] private float transitionSpeed = 5f;
        
        [Header("World Space Settings")]
        [SerializeField] private bool isWorldSpace = true;
        [SerializeField] private UnityEngine.Camera targetCamera;
        [SerializeField] private Vector3 offset = Vector3.up * 2f;
        [SerializeField] private bool alwaysFaceCamera = true;
        
        // Health data
        private float maxHealth = 100f;
        private float currentHealth = 100f;
        private float targetHealthPercentage = 1f;
        private float currentHealthPercentage = 1f;
        
        // Animation
        private bool isAnimating = false;
        
        // Events
        public System.Action<float> OnHealthChanged;
        public System.Action OnHealthDepleted;
        
        private void Awake()
        {
            InitializeComponents();
            SetupHealthBar();
        }
        
        private void Start()
        {
            if (targetCamera == null)
                targetCamera = UnityEngine.Camera.main;
                
            UpdateHealthDisplay();
        }
        
        private void Update()
        {
            if (isWorldSpace && alwaysFaceCamera && targetCamera != null)
            {
                FaceCamera();
            }
            
            if (smoothTransition && isAnimating)
            {
                UpdateSmoothTransition();
            }
        }
        
        private void LateUpdate()
        {
            if (isWorldSpace)
            {
                UpdateWorldPosition();
            }
        }
        
        /// <summary>
        /// Initialize UI components
        /// </summary>
        private void InitializeComponents()
        {
            // Auto-find components if not assigned
            if (healthSlider == null)
                healthSlider = GetComponentInChildren<Slider>();
                
            if (fillImage == null && healthSlider != null)
                fillImage = healthSlider.fillRect.GetComponent<Image>();
                
            if (backgroundImage == null && healthSlider != null)
                backgroundImage = healthSlider.GetComponentInChildren<Image>();
                
            if (healthText == null)
                healthText = GetComponentInChildren<Text>();
        }
        
        /// <summary>
        /// Setup health bar initial state
        /// </summary>
        private void SetupHealthBar()
        {
            if (healthSlider != null)
            {
                healthSlider.minValue = 0f;
                healthSlider.maxValue = 1f;
                healthSlider.value = 1f;
            }
            
            // Setup gradient if not assigned
            if (healthGradient.colorKeys.Length == 0)
            {
                SetupDefaultGradient();
            }
            
            UpdateHealthDisplay();
        }
        
        /// <summary>
        /// Setup default health gradient (red to green)
        /// </summary>
        private void SetupDefaultGradient()
        {
            healthGradient = new Gradient();
            
            GradientColorKey[] colorKeys = new GradientColorKey[3];
            colorKeys[0] = new GradientColorKey(Color.red, 0f);      // 0% health = red
            colorKeys[1] = new GradientColorKey(Color.yellow, 0.5f); // 50% health = yellow
            colorKeys[2] = new GradientColorKey(Color.green, 1f);    // 100% health = green
            
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0] = new GradientAlphaKey(1f, 0f);
            alphaKeys[1] = new GradientAlphaKey(1f, 1f);
            
            healthGradient.SetKeys(colorKeys, alphaKeys);
        }
        
        /// <summary>
        /// Set maximum health
        /// </summary>
        public void SetMaxHealth(float maxHp)
        {
            maxHealth = Mathf.Max(1f, maxHp);
            
            // Maintain current percentage
            float percentage = currentHealth / maxHealth;
            currentHealth = maxHealth * percentage;
            
            UpdateHealthDisplay();
        }
        
        /// <summary>
        /// Set current health
        /// </summary>
        public void SetCurrentHealth(float currentHp)
        {
            currentHealth = Mathf.Clamp(currentHp, 0f, maxHealth);
            targetHealthPercentage = currentHealth / maxHealth;
            
            if (smoothTransition)
            {
                isAnimating = true;
            }
            else
            {
                currentHealthPercentage = targetHealthPercentage;
                UpdateHealthDisplay();
            }
            
            OnHealthChanged?.Invoke(currentHealth);
            
            if (currentHealth <= 0f)
            {
                OnHealthDepleted?.Invoke();
            }
        }
        
        /// <summary>
        /// Add health (healing)
        /// </summary>
        public void AddHealth(float amount)
        {
            SetCurrentHealth(currentHealth + amount);
        }
        
        /// <summary>
        /// Subtract health (damage)
        /// </summary>
        public void SubtractHealth(float amount)
        {
            SetCurrentHealth(currentHealth - amount);
        }
        
        /// <summary>
        /// Update health display
        /// </summary>
        private void UpdateHealthDisplay()
        {
            float healthPercentage = currentHealthPercentage;
            
            // Update slider
            if (healthSlider != null)
            {
                healthSlider.value = healthPercentage;
            }
            
            // Update fill color
            if (fillImage != null)
            {
                fillImage.color = healthGradient.Evaluate(healthPercentage);
            }
            
            // Update text
            if (healthText != null && showHealthText)
            {
                healthText.text = $"{Mathf.Ceil(currentHealth)}/{Mathf.Ceil(maxHealth)}";
            }
        }
        
        /// <summary>
        /// Update smooth transition animation
        /// </summary>
        private void UpdateSmoothTransition()
        {
            currentHealthPercentage = Mathf.Lerp(currentHealthPercentage, targetHealthPercentage, 
                                               transitionSpeed * Time.deltaTime);
            
            if (Mathf.Abs(currentHealthPercentage - targetHealthPercentage) < 0.01f)
            {
                currentHealthPercentage = targetHealthPercentage;
                isAnimating = false;
            }
            
            UpdateHealthDisplay();
        }
        
        /// <summary>
        /// Make health bar face camera
        /// </summary>
        private void FaceCamera()
        {
            if (targetCamera == null) return;
            
            Vector3 directionToCamera = targetCamera.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(-directionToCamera);
        }
        
        /// <summary>
        /// Update world space position
        /// </summary>
        private void UpdateWorldPosition()
        {
            if (transform.parent != null)
            {
                transform.position = transform.parent.position + offset;
            }
        }
        
        /// <summary>
        /// Set health bar visibility
        /// </summary>
        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
        
        /// <summary>
        /// Flash health bar (for damage indication)
        /// </summary>
        public void FlashHealthBar(Color flashColor, float duration = 0.2f)
        {
            if (fillImage != null)
            {
                StartCoroutine(FlashCoroutine(flashColor, duration));
            }
        }
        
        private System.Collections.IEnumerator FlashCoroutine(Color flashColor, float duration)
        {
            Color originalColor = fillImage.color;
            fillImage.color = flashColor;
            
            yield return new WaitForSeconds(duration);
            
            fillImage.color = originalColor;
        }
        
        /// <summary>
        /// Animate health bar scale (for critical health)
        /// </summary>
        public void PulseHealthBar(bool enable)
        {
            if (enable)
            {
                StartCoroutine(PulseCoroutine());
            }
            else
            {
                StopAllCoroutines();
                transform.localScale = Vector3.one;
            }
        }
        
        private System.Collections.IEnumerator PulseCoroutine()
        {
            while (true)
            {
                // Scale up
                float t = 0f;
                while (t < 1f)
                {
                    t += Time.deltaTime * 2f;
                    float scale = Mathf.Lerp(1f, 1.1f, t);
                    transform.localScale = Vector3.one * scale;
                    yield return null;
                }
                
                // Scale down
                t = 0f;
                while (t < 1f)
                {
                    t += Time.deltaTime * 2f;
                    float scale = Mathf.Lerp(1.1f, 1f, t);
                    transform.localScale = Vector3.one * scale;
                    yield return null;
                }
            }
        }
        
        /// <summary>
        /// Set health bar offset
        /// </summary>
        public void SetOffset(Vector3 newOffset)
        {
            offset = newOffset;
        }
        
        /// <summary>
        /// Set target camera
        /// </summary>
        public void SetTargetCamera(UnityEngine.Camera camera)
        {
            targetCamera = camera;
        }
        
        // Getters
        public float GetCurrentHealth() => currentHealth;
        public float GetMaxHealth() => maxHealth;
        public float GetHealthPercentage() => currentHealth / maxHealth;
        public bool IsEmpty() => currentHealth <= 0f;
        public bool IsFull() => currentHealth >= maxHealth;
        
        // Unity Editor
        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                UpdateHealthDisplay();
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            if (isWorldSpace)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(transform.position + offset, Vector3.one * 0.1f);
                
                if (transform.parent != null)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(transform.parent.position, transform.position + offset);
                }
            }
        }
    }
}
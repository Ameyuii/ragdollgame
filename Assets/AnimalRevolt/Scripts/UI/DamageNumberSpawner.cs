using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace AnimalRevolt.UI
{
    /// <summary>
    /// Damage Number Spawner - Tạo và quản lý damage numbers
    /// </summary>
    public class DamageNumberSpawner : MonoBehaviour
    {
        [Header("Damage Number Settings")]
        [SerializeField] private GameObject damageNumberPrefab;
        [SerializeField] private Canvas worldCanvas;
        [SerializeField] private UnityEngine.Camera targetCamera;
        
        [Header("Animation Settings")]
        [SerializeField] private float animationDuration = 1f;
        [SerializeField] private float moveDistance = 2f;
        [SerializeField] private AnimationCurve moveCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        [SerializeField] private AnimationCurve scaleCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));
        [SerializeField] private AnimationCurve alphaCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));
        
        [Header("Visual Settings")]
        [SerializeField] private Color normalDamageColor = Color.white;
        [SerializeField] private Color criticalDamageColor = Color.red;
        [SerializeField] private Color healingColor = Color.green;
        [SerializeField] private Font damageFont;
        [SerializeField] private int fontSize = 24;
        [SerializeField] private int criticalFontSize = 32;
        
        [Header("Spawn Settings")]
        [SerializeField] private Vector3 spawnOffset = Vector3.zero;
        [SerializeField] private float spawnRadius = 0.5f;
        [SerializeField] private bool randomizeDirection = true;
        
        // Object pooling
        private Queue<DamageNumber> damageNumberPool = new Queue<DamageNumber>();
        private List<DamageNumber> activeDamageNumbers = new List<DamageNumber>();
        private int poolSize = 20;
        
        // Events
        public System.Action<float, Vector3> OnDamageNumberSpawned;
        
        private void Awake()
        {
            InitializeComponents();
            SetupDefaultCurves();
            CreateDamageNumberPool();
        }
        
        private void Start()
        {
            if (targetCamera == null)
                targetCamera = UnityEngine.Camera.main;
        }
        
        private void Update()
        {
            UpdateActiveDamageNumbers();
        }
        
        /// <summary>
        /// Setup default animation curves
        /// </summary>
        private void SetupDefaultCurves()
        {
            // Setup move curve (ease out)
            if (moveCurve == null || moveCurve.keys.Length == 0)
            {
                moveCurve = new AnimationCurve();
                moveCurve.AddKey(new Keyframe(0f, 0f, 0f, 2f));
                moveCurve.AddKey(new Keyframe(1f, 1f, 0f, 0f));
            }
            
            // Setup scale curve (start big, shrink)
            if (scaleCurve == null || scaleCurve.keys.Length == 0)
            {
                scaleCurve = new AnimationCurve();
                scaleCurve.AddKey(new Keyframe(0f, 1.2f, 0f, 0f));
                scaleCurve.AddKey(new Keyframe(0.3f, 1f, 0f, 0f));
                scaleCurve.AddKey(new Keyframe(1f, 0.8f, 0f, 0f));
            }
            
            // Setup alpha curve (fade out)
            if (alphaCurve == null || alphaCurve.keys.Length == 0)
            {
                alphaCurve = new AnimationCurve();
                alphaCurve.AddKey(new Keyframe(0f, 1f, 0f, 0f));
                alphaCurve.AddKey(new Keyframe(0.7f, 1f, 0f, 0f));
                alphaCurve.AddKey(new Keyframe(1f, 0f, -2f, 0f));
            }
        }
        
        /// <summary>
        /// Initialize components
        /// </summary>
        private void InitializeComponents()
        {
            // Create world canvas if not assigned
            if (worldCanvas == null)
            {
                GameObject canvasGO = new GameObject("DamageNumberCanvas");
                canvasGO.transform.SetParent(transform);
                
                worldCanvas = canvasGO.AddComponent<Canvas>();
                worldCanvas.renderMode = RenderMode.WorldSpace;
                
                CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
                scaler.dynamicPixelsPerUnit = 10f;
                
                GraphicRaycaster raycaster = canvasGO.AddComponent<GraphicRaycaster>();
            }
            
            // Create default damage number prefab if not assigned
            if (damageNumberPrefab == null)
            {
                CreateDefaultDamageNumberPrefab();
            }
        }
        
        /// <summary>
        /// Create default damage number prefab
        /// </summary>
        private void CreateDefaultDamageNumberPrefab()
        {
            GameObject prefab = new GameObject("DamageNumber");
            
            Text textComponent = prefab.AddComponent<Text>();
            textComponent.font = damageFont != null ? damageFont : Resources.GetBuiltinResource<Font>("Arial.ttf");
            textComponent.fontSize = fontSize;
            textComponent.color = normalDamageColor;
            textComponent.alignment = TextAnchor.MiddleCenter;
            textComponent.text = "0";
            
            // Add outline for better visibility
            Outline outline = prefab.AddComponent<Outline>();
            outline.effectColor = Color.black;
            outline.effectDistance = new Vector2(1, -1);
            
            DamageNumber damageNumberScript = prefab.AddComponent<DamageNumber>();
            
            damageNumberPrefab = prefab;
        }
        
        /// <summary>
        /// Create object pool for damage numbers
        /// </summary>
        private void CreateDamageNumberPool()
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject damageNumberGO = Instantiate(damageNumberPrefab, worldCanvas.transform);
                DamageNumber damageNumber = damageNumberGO.GetComponent<DamageNumber>();
                damageNumber.Initialize(this);
                damageNumberGO.SetActive(false);
                damageNumberPool.Enqueue(damageNumber);
            }
        }
        
        /// <summary>
        /// Spawn damage number
        /// </summary>
        public void SpawnDamageNumber(float damage, Vector3 worldPosition, bool isCritical = false, bool isHealing = false)
        {
            DamageNumber damageNumber = GetPooledDamageNumber();
            if (damageNumber == null) return;
            
            // Calculate spawn position with randomization
            Vector3 spawnPos = worldPosition + spawnOffset;
            if (randomizeDirection && spawnRadius > 0)
            {
                Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
                spawnPos += new Vector3(randomCircle.x, 0, randomCircle.y);
            }
            
            // Setup damage number
            damageNumber.Setup(damage, spawnPos, isCritical, isHealing);
            damageNumber.gameObject.SetActive(true);
            activeDamageNumbers.Add(damageNumber);
            
            OnDamageNumberSpawned?.Invoke(damage, worldPosition);
        }
        
        /// <summary>
        /// Spawn critical damage number
        /// </summary>
        public void SpawnCriticalDamage(float damage, Vector3 worldPosition)
        {
            SpawnDamageNumber(damage, worldPosition, true, false);
        }
        
        /// <summary>
        /// Spawn healing number
        /// </summary>
        public void SpawnHealingNumber(float healing, Vector3 worldPosition)
        {
            SpawnDamageNumber(healing, worldPosition, false, true);
        }
        
        /// <summary>
        /// Get pooled damage number
        /// </summary>
        private DamageNumber GetPooledDamageNumber()
        {
            if (damageNumberPool.Count > 0)
            {
                return damageNumberPool.Dequeue();
            }
            
            // If pool is empty, create new one
            GameObject damageNumberGO = Instantiate(damageNumberPrefab, worldCanvas.transform);
            DamageNumber damageNumber = damageNumberGO.GetComponent<DamageNumber>();
            damageNumber.Initialize(this);
            return damageNumber;
        }
        
        /// <summary>
        /// Return damage number to pool
        /// </summary>
        public void ReturnToPool(DamageNumber damageNumber)
        {
            activeDamageNumbers.Remove(damageNumber);
            damageNumber.gameObject.SetActive(false);
            damageNumberPool.Enqueue(damageNumber);
        }
        
        /// <summary>
        /// Update all active damage numbers
        /// </summary>
        private void UpdateActiveDamageNumbers()
        {
            for (int i = activeDamageNumbers.Count - 1; i >= 0; i--)
            {
                if (activeDamageNumbers[i] != null)
                {
                    activeDamageNumbers[i].UpdateDamageNumber();
                }
            }
        }
        
        /// <summary>
        /// Clear all active damage numbers
        /// </summary>
        public void ClearAllDamageNumbers()
        {
            for (int i = activeDamageNumbers.Count - 1; i >= 0; i--)
            {
                if (activeDamageNumbers[i] != null)
                {
                    ReturnToPool(activeDamageNumbers[i]);
                }
            }
        }
        
        // Getters for damage number animation
        public float GetAnimationDuration() => animationDuration;
        public float GetMoveDistance() => moveDistance;
        public AnimationCurve GetMoveCurve() => moveCurve;
        public AnimationCurve GetScaleCurve() => scaleCurve;
        public AnimationCurve GetAlphaCurve() => alphaCurve;
        public Color GetNormalDamageColor() => normalDamageColor;
        public Color GetCriticalDamageColor() => criticalDamageColor;
        public Color GetHealingColor() => healingColor;
        public int GetFontSize() => fontSize;
        public int GetCriticalFontSize() => criticalFontSize;
        public UnityEngine.Camera GetTargetCamera() => targetCamera;
    }
    
    /// <summary>
    /// Individual damage number component
    /// </summary>
    public class DamageNumber : MonoBehaviour
    {
        private Text textComponent;
        private DamageNumberSpawner spawner;
        private float startTime;
        private Vector3 startPosition;
        private bool isInitialized = false;
        
        /// <summary>
        /// Initialize damage number
        /// </summary>
        public void Initialize(DamageNumberSpawner damageSpawner)
        {
            spawner = damageSpawner;
            textComponent = GetComponent<Text>();
            isInitialized = true;
        }
        
        /// <summary>
        /// Setup damage number for display
        /// </summary>
        public void Setup(float damage, Vector3 worldPosition, bool isCritical, bool isHealing)
        {
            if (!isInitialized) return;
            
            startTime = Time.time;
            startPosition = worldPosition;
            transform.position = worldPosition;
            
            // Set text
            string damageText = Mathf.Ceil(damage).ToString();
            if (isHealing) damageText = "+" + damageText;
            textComponent.text = damageText;
            
            // Set color and size
            if (isHealing)
            {
                textComponent.color = spawner.GetHealingColor();
                textComponent.fontSize = spawner.GetFontSize();
            }
            else if (isCritical)
            {
                textComponent.color = spawner.GetCriticalDamageColor();
                textComponent.fontSize = spawner.GetCriticalFontSize();
            }
            else
            {
                textComponent.color = spawner.GetNormalDamageColor();
                textComponent.fontSize = spawner.GetFontSize();
            }
            
            // Reset scale and alpha
            transform.localScale = Vector3.one;
            Color color = textComponent.color;
            color.a = 1f;
            textComponent.color = color;
        }
        
        /// <summary>
        /// Update damage number animation
        /// </summary>
        public void UpdateDamageNumber()
        {
            if (!isInitialized || spawner == null) return;
            
            float elapsed = Time.time - startTime;
            float normalizedTime = elapsed / spawner.GetAnimationDuration();
            
            if (normalizedTime >= 1f)
            {
                // Animation finished, return to pool
                spawner.ReturnToPool(this);
                return;
            }
            
            // Update position
            float moveProgress = spawner.GetMoveCurve().Evaluate(normalizedTime);
            Vector3 targetPosition = startPosition + Vector3.up * spawner.GetMoveDistance();
            transform.position = Vector3.Lerp(startPosition, targetPosition, moveProgress);
            
            // Update scale
            float scaleProgress = spawner.GetScaleCurve().Evaluate(normalizedTime);
            transform.localScale = Vector3.one * scaleProgress;
            
            // Update alpha
            float alphaProgress = spawner.GetAlphaCurve().Evaluate(normalizedTime);
            Color color = textComponent.color;
            color.a = alphaProgress;
            textComponent.color = color;
            
            // Face camera
            if (spawner.GetTargetCamera() != null)
            {
                Vector3 directionToCamera = spawner.GetTargetCamera().transform.position - transform.position;
                transform.rotation = Quaternion.LookRotation(-directionToCamera);
            }
        }
    }
}
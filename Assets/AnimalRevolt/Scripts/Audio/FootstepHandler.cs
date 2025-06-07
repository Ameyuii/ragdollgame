using UnityEngine;

/// <summary>
/// Component xử lý Animation Events cho footstep sounds
/// Khắc phục lỗi "AnimationEvent 'OnFootstep' has no receiver"
/// </summary>
public class FootstepHandler : MonoBehaviour
{
    [Header("Footstep Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private bool enableFootstepSounds = true;
    [SerializeField] private float volumeRange = 0.1f;
    [SerializeField] private bool debugMode = false;
    
    [Header("Performance")]
    [SerializeField] private float minTimeBetweenSteps = 0.1f;
    
    private float lastFootstepTime;
    
    private void Awake()
    {
        // Auto-find AudioSource nếu chưa có
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
            
        // Tạo AudioSource nếu không có
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            ConfigureAudioSource();
        }
    }
    
    /// <summary>
    /// Configure AudioSource settings
    /// </summary>
    private void ConfigureAudioSource()
    {
        audioSource.playOnAwake = false;
        audioSource.volume = 0.5f;
        audioSource.pitch = 1f;
        audioSource.spatialBlend = 1f; // 3D sound
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.maxDistance = 20f;
    }
    
    /// <summary>
    /// Animation Event receiver - được gọi từ animation 'Walk_N'
    /// </summary>
    public void OnFootstep()
    {
        if (!enableFootstepSounds) return;
        
        // Throttle để tránh spam sounds
        if (Time.time - lastFootstepTime < minTimeBetweenSteps)
            return;
            
        PlayFootstepSound();
        lastFootstepTime = Time.time;
        
        if (debugMode)
            Debug.Log($"👟 {gameObject.name} footstep played");
    }
    
    /// <summary>
    /// Play random footstep sound
    /// </summary>
    private void PlayFootstepSound()
    {
        if (audioSource == null) return;
        
        // Sử dụng default footstep nếu không có clips
        if (footstepClips == null || footstepClips.Length == 0)
        {
            // Play a simple beep sound as fallback
            PlayDefaultFootstep();
            return;
        }
        
        // Play random footstep clip
        AudioClip clipToPlay = footstepClips[Random.Range(0, footstepClips.Length)];
        if (clipToPlay != null)
        {
            // Add slight volume variation
            float volume = audioSource.volume + Random.Range(-volumeRange, volumeRange);
            volume = Mathf.Clamp01(volume);
            
            audioSource.pitch = Random.Range(0.9f, 1.1f); // Slight pitch variation
            audioSource.PlayOneShot(clipToPlay, volume);
        }
    }
    
    /// <summary>
    /// Play default footstep sound nếu không có AudioClips
    /// </summary>
    private void PlayDefaultFootstep()
    {
        // Tạo simple footstep sound bằng code
        if (audioSource != null)
        {
            // Tạm thời disable để không spam console
            // audioSource.pitch = Random.Range(0.8f, 1.2f);
            // audioSource.volume = 0.3f;
            
            if (debugMode)
                Debug.Log($"🔇 {gameObject.name} footstep (no audio clips assigned)");
        }
    }
    
    /// <summary>
    /// Set footstep clips từ code
    /// </summary>
    public void SetFootstepClips(AudioClip[] clips)
    {
        footstepClips = clips;
    }
    
    /// <summary>
    /// Enable/disable footstep sounds
    /// </summary>
    public void SetFootstepEnabled(bool enabled)
    {
        enableFootstepSounds = enabled;
    }
    
    /// <summary>
    /// Alternative Animation Event names - support multiple event names
    /// </summary>
    public void OnFootStep() => OnFootstep(); // Alternative spelling
    public void Footstep() => OnFootstep();   // Simple name
    public void Step() => OnFootstep();       // Short name
    public void OnStep() => OnFootstep();     // Another variant
    
    /// <summary>
    /// Animation Event cho left/right foot
    /// </summary>
    public void OnLeftFootstep() => OnFootstep();
    public void OnRightFootstep() => OnFootstep();
}
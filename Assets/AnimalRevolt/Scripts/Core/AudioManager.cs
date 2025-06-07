using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Collections;

namespace AnimalRevolt.Core
{
    /// <summary>
    /// Audio Manager - Quản lý toàn bộ âm thanh trong game
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Mixer")]
        [SerializeField] private AudioMixerGroup masterMixerGroup;
        [SerializeField] private AudioMixerGroup musicMixerGroup;
        [SerializeField] private AudioMixerGroup sfxMixerGroup;
        [SerializeField] private AudioMixerGroup voiceMixerGroup;
        
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource voiceSource;
        
        [Header("Audio Clips")]
        [SerializeField] private AudioClip[] backgroundMusics;
        [SerializeField] private AudioClip[] combatMusics;
        [SerializeField] private AudioClip[] menuMusics;
        
        [Header("SFX Clips")]
        [SerializeField] private AudioClip[] punchSounds;
        [SerializeField] private AudioClip[] kickSounds;
        [SerializeField] private AudioClip[] impactSounds;
        [SerializeField] private AudioClip[] uiSounds;
        
        [Header("Voice Clips")]
        [SerializeField] private AudioClip[] characterVoices;
        [SerializeField] private AudioClip[] announcerVoices;
        
        [Header("Settings")]
        [Range(0f, 1f)]
        public float masterVolume = 1f;
        [Range(0f, 1f)]
        public float musicVolume = 0.8f;
        [Range(0f, 1f)]
        public float sfxVolume = 1f;
        [Range(0f, 1f)]
        public float voiceVolume = 1f;
        
        [SerializeField] private bool muteOnFocusLoss = true;
        
        // Audio Source Pool cho SFX
        private Queue<AudioSource> sfxPool = new Queue<AudioSource>();
        private int poolSize = 10;
        
        // Current playing music
        private AudioClip currentMusic;
        private MusicType currentMusicType = MusicType.None;
        
        public enum MusicType
        {
            None,
            Menu,
            Battle,
            Background
        }
        
        public enum SFXType
        {
            Punch,
            Kick,
            Impact,
            UI,
            Custom
        }
        
        // Events
        public System.Action<AudioClip> OnMusicChanged;
        public System.Action<bool> OnMusicMuted;
        
        private void Awake()
        {
            InitializeAudioSources();
            CreateSFXPool();
            LoadAudioSettings();
        }
        
        private void Start()
        {
            ApplyVolumeSettings();
        }
        
        /// <summary>
        /// Khởi tạo audio sources
        /// </summary>
        private void InitializeAudioSources()
        {
            if (musicSource == null)
            {
                GameObject musicGO = new GameObject("Music Source");
                musicGO.transform.SetParent(transform);
                musicSource = musicGO.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
                musicSource.outputAudioMixerGroup = musicMixerGroup;
            }
            
            if (sfxSource == null)
            {
                GameObject sfxGO = new GameObject("SFX Source");
                sfxGO.transform.SetParent(transform);
                sfxSource = sfxGO.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
                sfxSource.outputAudioMixerGroup = sfxMixerGroup;
            }
            
            if (voiceSource == null)
            {
                GameObject voiceGO = new GameObject("Voice Source");
                voiceGO.transform.SetParent(transform);
                voiceSource = voiceGO.AddComponent<AudioSource>();
                voiceSource.loop = false;
                voiceSource.playOnAwake = false;
                voiceSource.outputAudioMixerGroup = voiceMixerGroup;
            }
        }
        
        /// <summary>
        /// Tạo pool các AudioSource cho SFX
        /// </summary>
        private void CreateSFXPool()
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject sfxGO = new GameObject($"SFX Pool {i}");
                sfxGO.transform.SetParent(transform);
                AudioSource source = sfxGO.AddComponent<AudioSource>();
                source.loop = false;
                source.playOnAwake = false;
                source.outputAudioMixerGroup = sfxMixerGroup;
                sfxPool.Enqueue(source);
            }
        }
        
        /// <summary>
        /// Play nhạc nền
        /// </summary>
        public void PlayMusic(MusicType type, int index = 0, bool fadeIn = true)
        {
            AudioClip[] targetArray = null;
            
            switch (type)
            {
                case MusicType.Menu:
                    targetArray = menuMusics;
                    break;
                case MusicType.Battle:
                    targetArray = combatMusics;
                    break;
                case MusicType.Background:
                    targetArray = backgroundMusics;
                    break;
            }
            
            if (targetArray == null || targetArray.Length == 0 || index >= targetArray.Length)
            {
                Debug.LogWarning($"[AudioManager] No music found for type: {type}, index: {index}");
                return;
            }
            
            AudioClip newMusic = targetArray[index];
            if (newMusic == currentMusic && musicSource.isPlaying) return;
            
            if (fadeIn && musicSource.isPlaying)
            {
                StartCoroutine(CrossfadeMusic(newMusic));
            }
            else
            {
                musicSource.clip = newMusic;
                musicSource.Play();
            }
            
            currentMusic = newMusic;
            currentMusicType = type;
            OnMusicChanged?.Invoke(newMusic);
            
            Debug.Log($"[AudioManager] Playing music: {newMusic.name}");
        }
        
        /// <summary>
        /// Crossfade giữa các nhạc nền
        /// </summary>
        private IEnumerator CrossfadeMusic(AudioClip newMusic)
        {
            float fadeDuration = 1f;
            float startVolume = musicSource.volume;
            
            // Fade out
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
                yield return null;
            }
            
            // Switch music
            musicSource.clip = newMusic;
            musicSource.Play();
            
            // Fade in
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                musicSource.volume = Mathf.Lerp(0, startVolume, t / fadeDuration);
                yield return null;
            }
            
            musicSource.volume = startVolume;
        }
        
        /// <summary>
        /// Dừng nhạc nền
        /// </summary>
        public void StopMusic(bool fadeOut = true)
        {
            if (fadeOut)
            {
                StartCoroutine(FadeOutMusic());
            }
            else
            {
                musicSource.Stop();
            }
        }
        
        private IEnumerator FadeOutMusic()
        {
            float fadeDuration = 1f;
            float startVolume = musicSource.volume;
            
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
                yield return null;
            }
            
            musicSource.Stop();
            musicSource.volume = startVolume;
        }
        
        /// <summary>
        /// Play SFX
        /// </summary>
        public void PlaySFX(SFXType type, int index = 0, float volume = 1f)
        {
            AudioClip[] targetArray = null;
            
            switch (type)
            {
                case SFXType.Punch:
                    targetArray = punchSounds;
                    break;
                case SFXType.Kick:
                    targetArray = kickSounds;
                    break;
                case SFXType.Impact:
                    targetArray = impactSounds;
                    break;
                case SFXType.UI:
                    targetArray = uiSounds;
                    break;
            }
            
            if (targetArray == null || targetArray.Length == 0 || index >= targetArray.Length)
            {
                Debug.LogWarning($"[AudioManager] No SFX found for type: {type}, index: {index}");
                return;
            }
            
            PlaySFX(targetArray[index], volume);
        }
        
        /// <summary>
        /// Play SFX với AudioClip
        /// </summary>
        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;
            
            if (sfxPool.Count > 0)
            {
                AudioSource source = sfxPool.Dequeue();
                StartCoroutine(PlaySFXFromPool(source, clip, volume));
            }
            else
            {
                sfxSource.PlayOneShot(clip, volume);
            }
        }
        
        private IEnumerator PlaySFXFromPool(AudioSource source, AudioClip clip, float volume)
        {
            source.clip = clip;
            source.volume = volume;
            source.Play();
            
            yield return new WaitForSeconds(clip.length);
            
            sfxPool.Enqueue(source);
        }
        
        /// <summary>
        /// Play random SFX từ array
        /// </summary>
        public void PlayRandomSFX(SFXType type, float volume = 1f)
        {
            AudioClip[] targetArray = null;
            
            switch (type)
            {
                case SFXType.Punch:
                    targetArray = punchSounds;
                    break;
                case SFXType.Kick:
                    targetArray = kickSounds;
                    break;
                case SFXType.Impact:
                    targetArray = impactSounds;
                    break;
                case SFXType.UI:
                    targetArray = uiSounds;
                    break;
            }
            
            if (targetArray == null || targetArray.Length == 0) return;
            
            int randomIndex = Random.Range(0, targetArray.Length);
            PlaySFX(targetArray[randomIndex], volume);
        }
        
        /// <summary>
        /// Apply volume settings
        /// </summary>
        public void ApplyVolumeSettings()
        {
            if (masterMixerGroup != null)
                masterMixerGroup.audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
            if (musicMixerGroup != null)
                musicMixerGroup.audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
            if (sfxMixerGroup != null)
                sfxMixerGroup.audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
            if (voiceMixerGroup != null)
                voiceMixerGroup.audioMixer.SetFloat("VoiceVolume", Mathf.Log10(voiceVolume) * 20);
        }
        
        /// <summary>
        /// Load audio settings
        /// </summary>
        private void LoadAudioSettings()
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            voiceVolume = PlayerPrefs.GetFloat("VoiceVolume", 1f);
        }
        
        /// <summary>
        /// Mute/Unmute tất cả audio
        /// </summary>
        public void SetMasterMute(bool mute)
        {
            AudioListener.volume = mute ? 0f : 1f;
            OnMusicMuted?.Invoke(mute);
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (muteOnFocusLoss)
            {
                SetMasterMute(!hasFocus);
            }
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (muteOnFocusLoss)
            {
                SetMasterMute(pauseStatus);
            }
        }
    }
}
using UnityEngine;

namespace AnimalRevolt.Core
{
    /// <summary>
    /// ScriptableObject chứa các cài đặt chung của game
    /// </summary>
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Animal Revolt/Core/Game Settings")]
    public class GameSettings : ScriptableObject
    {
        [Header("Graphics Settings")]
        public int targetFrameRate = 60;
        public bool vSyncEnabled = true;
        public int qualityLevel = 2; // 0=Low, 1=Medium, 2=High
        
        [Header("Audio Settings")]
        [Range(0f, 1f)]
        public float masterVolume = 1f;
        
        [Range(0f, 1f)]
        public float musicVolume = 0.8f;
        
        [Range(0f, 1f)]
        public float sfxVolume = 1f;
        
        [Range(0f, 1f)]
        public float voiceVolume = 1f;
        
        [Header("Gameplay Settings")]
        public float battleTimeLimit = 300f; // 5 phút
        public int maxRounds = 3;
        public bool showDamageNumbers = true;
        public bool showHealthBars = true;
        
        [Header("Control Settings")]
        public float mouseSensitivity = 1f;
        public bool invertMouseY = false;
        
        [Header("Mobile Settings")]
        public float touchSensitivity = 1f;
        public bool hapticFeedback = true;
        
        [Header("Developer Settings")]
        public bool debugMode = false;
        public bool showFPS = false;
        public bool godMode = false;
        
        /// <summary>
        /// Apply tất cả settings
        /// </summary>
        public void ApplySettings()
        {
            // Graphics
            Application.targetFrameRate = targetFrameRate;
            QualitySettings.vSyncCount = vSyncEnabled ? 1 : 0;
            QualitySettings.SetQualityLevel(qualityLevel);
            
            // Audio (sẽ được AudioManager xử lý)
            
            Debug.Log("[GameSettings] Settings applied successfully");
        }
        
        /// <summary>
        /// Load settings từ PlayerPrefs
        /// </summary>
        public void LoadFromPlayerPrefs()
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            voiceVolume = PlayerPrefs.GetFloat("VoiceVolume", 1f);
            
            targetFrameRate = PlayerPrefs.GetInt("TargetFrameRate", 60);
            vSyncEnabled = PlayerPrefs.GetInt("VSyncEnabled", 1) == 1;
            qualityLevel = PlayerPrefs.GetInt("QualityLevel", 2);
            
            mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1f);
            invertMouseY = PlayerPrefs.GetInt("InvertMouseY", 0) == 1;
            
            touchSensitivity = PlayerPrefs.GetFloat("TouchSensitivity", 1f);
            hapticFeedback = PlayerPrefs.GetInt("HapticFeedback", 1) == 1;
            
            showDamageNumbers = PlayerPrefs.GetInt("ShowDamageNumbers", 1) == 1;
            showHealthBars = PlayerPrefs.GetInt("ShowHealthBars", 1) == 1;
            
            debugMode = PlayerPrefs.GetInt("DebugMode", 0) == 1;
            showFPS = PlayerPrefs.GetInt("ShowFPS", 0) == 1;
            
            Debug.Log("[GameSettings] Settings loaded from PlayerPrefs");
        }
        
        /// <summary>
        /// Save settings to PlayerPrefs
        /// </summary>
        public void SaveToPlayerPrefs()
        {
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
            PlayerPrefs.SetFloat("VoiceVolume", voiceVolume);
            
            PlayerPrefs.SetInt("TargetFrameRate", targetFrameRate);
            PlayerPrefs.SetInt("VSyncEnabled", vSyncEnabled ? 1 : 0);
            PlayerPrefs.SetInt("QualityLevel", qualityLevel);
            
            PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity);
            PlayerPrefs.SetInt("InvertMouseY", invertMouseY ? 1 : 0);
            
            PlayerPrefs.SetFloat("TouchSensitivity", touchSensitivity);
            PlayerPrefs.SetInt("HapticFeedback", hapticFeedback ? 1 : 0);
            
            PlayerPrefs.SetInt("ShowDamageNumbers", showDamageNumbers ? 1 : 0);
            PlayerPrefs.SetInt("ShowHealthBars", showHealthBars ? 1 : 0);
            
            PlayerPrefs.SetInt("DebugMode", debugMode ? 1 : 0);
            PlayerPrefs.SetInt("ShowFPS", showFPS ? 1 : 0);
            
            PlayerPrefs.Save();
            Debug.Log("[GameSettings] Settings saved to PlayerPrefs");
        }
        
        /// <summary>
        /// Reset về default values
        /// </summary>
        public void ResetToDefault()
        {
            masterVolume = 1f;
            musicVolume = 0.8f;
            sfxVolume = 1f;
            voiceVolume = 1f;
            
            targetFrameRate = 60;
            vSyncEnabled = true;
            qualityLevel = 2;
            
            mouseSensitivity = 1f;
            invertMouseY = false;
            
            touchSensitivity = 1f;
            hapticFeedback = true;
            
            showDamageNumbers = true;
            showHealthBars = true;
            
            debugMode = false;
            showFPS = false;
            godMode = false;
            
            Debug.Log("[GameSettings] Settings reset to default");
        }
    }
}
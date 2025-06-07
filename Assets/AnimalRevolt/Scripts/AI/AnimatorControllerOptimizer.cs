using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;

/// <summary>
/// 🎬 Optimizer cho Animator Controller transitions - Khắc phục animation delay cho AI NPC
/// Tự động configure transitions để có instant response, no exit time, minimal duration
/// </summary>
[System.Serializable]
public class AnimatorControllerOptimizer : MonoBehaviour
{
    [Header("🎯 Animator Controller Settings")]
    [SerializeField] private Animator targetAnimator;
    [SerializeField] private bool autoOptimizeOnStart = true;
    [SerializeField] private bool debugMode = true;
    
    [Header("⚡ Transition Optimization Settings")]
    [SerializeField] private float transitionDuration = 0.05f;
    [SerializeField] private bool disableExitTime = true;
    [SerializeField] private TransitionInterruptionSource interruptionSource = TransitionInterruptionSource.Source;
    
    [Header("📊 Parameters to Configure")]
    [SerializeField] private bool addSpeedParameterIfMissing = true;
    [SerializeField] private bool addIsWalkingParameterIfMissing = true;
    
    [Header("🔧 Inspector Controls")]
    [SerializeField] private bool showOptimizationDetails = true;
    
    private AnimatorController animatorController;
    private List<string> optimizationLog = new List<string>();
    
    private void Awake()
    {
        if (targetAnimator == null)
            targetAnimator = GetComponent<Animator>();
    }
    
    private void Start()
    {
        if (autoOptimizeOnStart)
        {
            OptimizeAnimatorController();
        }
    }
    
    /// <summary>
    /// 🎬 Main optimization method - Optimize toàn bộ Animator Controller
    /// </summary>
    [ContextMenu("Optimize Animator Controller")]
    public void OptimizeAnimatorController()
    {
        optimizationLog.Clear();
        
        if (!ValidateComponents())
            return;
            
        LogDebug("🚀 Bắt đầu optimize Animator Controller...");
        
#if UNITY_EDITOR
        // Chỉ chạy trong Editor mode
        if (Application.isPlaying)
        {
            LogDebug("⚠️ Runtime optimization không được hỗ trợ. Chạy trong Editor mode.");
            return;
        }
        
        animatorController = targetAnimator.runtimeAnimatorController as AnimatorController;
        if (animatorController == null)
        {
            LogDebug("❌ Không tìm thấy AnimatorController!");
            return;
        }
        
        // 1. Add missing parameters
        AddMissingParameters();
        
        // 2. Optimize transitions
        OptimizeAllTransitions();
        
        // 3. Configure blend trees
        OptimizeBlendTrees();
        
        // Mark dirty để save changes
        EditorUtility.SetDirty(animatorController);
        AssetDatabase.SaveAssets();
        
        LogDebug("✅ Hoàn thành optimize Animator Controller!");
        
        if (showOptimizationDetails)
        {
            ShowOptimizationSummary();
        }
#else
        LogDebug("⚠️ Optimization chỉ có thể chạy trong Unity Editor!");
#endif
    }
    
    /// <summary>
    /// 📊 Add missing animation parameters
    /// </summary>
    private void AddMissingParameters()
    {
#if UNITY_EDITOR
        var parameters = animatorController.parameters;
        List<string> existingParams = new List<string>();
        
        foreach (var param in parameters)
        {
            existingParams.Add(param.name);
        }
        
        // Add Speed parameter (Float)
        if (addSpeedParameterIfMissing && !existingParams.Contains("Speed"))
        {
            animatorController.AddParameter("Speed", AnimatorControllerParameterType.Float);
            LogDebug("➕ Added Speed parameter (Float)");
        }
        
        // Add IsWalking parameter (Bool)
        if (addIsWalkingParameterIfMissing && !existingParams.Contains("IsWalking"))
        {
            animatorController.AddParameter("IsWalking", AnimatorControllerParameterType.Bool);
            LogDebug("➕ Added IsWalking parameter (Bool)");
        }
        
        // Thêm các parameters hữu ích khác
        if (!existingParams.Contains("MotionSpeed"))
        {
            animatorController.AddParameter("MotionSpeed", AnimatorControllerParameterType.Float);
            LogDebug("➕ Added MotionSpeed parameter (Float)");
        }
#endif
    }
    
    /// <summary>
    /// ⚡ Optimize tất cả transitions trong controller
    /// </summary>
    private void OptimizeAllTransitions()
    {
#if UNITY_EDITOR
        int optimizedCount = 0;
        
        foreach (var layer in animatorController.layers)
        {
            optimizedCount += OptimizeStateMachineTransitions(layer.stateMachine);
        }
        
        LogDebug($"⚡ Optimized {optimizedCount} transitions");
#endif
    }
    
    /// <summary>
    /// 🔧 Optimize transitions trong một state machine
    /// </summary>
    private int OptimizeStateMachineTransitions(AnimatorStateMachine stateMachine)
    {
#if UNITY_EDITOR
        int count = 0;
        
        // Optimize transitions từ states
        foreach (var state in stateMachine.states)
        {
            foreach (var transition in state.state.transitions)
            {
                OptimizeTransition(transition, state.state.name);
                count++;
            }
        }
        
        // Optimize any state transitions
        foreach (var transition in stateMachine.anyStateTransitions)
        {
            OptimizeTransition(transition, "AnyState");
            count++;
        }
        
        // Recursively optimize sub-state machines
        foreach (var subStateMachine in stateMachine.stateMachines)
        {
            count += OptimizeStateMachineTransitions(subStateMachine.stateMachine);
        }
        
        return count;
#else
        return 0;
#endif
    }
    
    /// <summary>
    /// 🎯 Optimize một transition cụ thể
    /// </summary>
    private void OptimizeTransition(AnimatorStateTransition transition, string fromStateName)
    {
#if UNITY_EDITOR
        bool wasModified = false;
        
        // 1. ⚡ Set Transition Duration = 0.05 (minimal)
        if (transition.duration != transitionDuration)
        {
            transition.duration = transitionDuration;
            wasModified = true;
        }
        
        // 2. 🚫 Disable Exit Time
        if (disableExitTime && transition.hasExitTime)
        {
            transition.hasExitTime = false;
            wasModified = true;
        }
        
        // 3. 🔄 Set Interruption Source = Current State
        if (transition.interruptionSource != interruptionSource)
        {
            transition.interruptionSource = interruptionSource;
            wasModified = true;
        }
        
        // 4. ⚡ Enable Fixed Duration
        if (!transition.hasFixedDuration)
        {
            transition.hasFixedDuration = true;
            wasModified = true;
        }
        
        // 5. 🎯 Set Offset = 0
        if (transition.offset != 0f)
        {
            transition.offset = 0f;
            wasModified = true;
        }
        
        // 6. ✅ Enable Ordered Interruption
        if (!transition.orderedInterruption)
        {
            transition.orderedInterruption = true;
            wasModified = true;
        }
        
        if (wasModified)
        {
            string destinationName = transition.destinationState != null ? 
                transition.destinationState.name : "Exit";
            
            LogDebug($"🔧 Optimized transition: {fromStateName} → {destinationName}");
        }
#endif
    }
    
    /// <summary>
    /// 🌳 Optimize blend trees để sử dụng Speed parameter
    /// </summary>
    private void OptimizeBlendTrees()
    {
#if UNITY_EDITOR
        foreach (var layer in animatorController.layers)
        {
            OptimizeBlendTreesInStateMachine(layer.stateMachine);
        }
#endif
    }
    
    /// <summary>
    /// 🔧 Optimize blend trees trong state machine
    /// </summary>
    private void OptimizeBlendTreesInStateMachine(AnimatorStateMachine stateMachine)
    {
#if UNITY_EDITOR
        foreach (var state in stateMachine.states)
        {
            if (state.state.motion is BlendTree blendTree)
            {
                // Ensure blend tree sử dụng Speed parameter
                if (blendTree.blendParameter != "Speed")
                {
                    blendTree.blendParameter = "Speed";
                    LogDebug($"🌳 Set blend parameter to 'Speed' for {state.state.name}");
                }
                
                if (blendTree.blendParameterY != "Speed")
                {
                    blendTree.blendParameterY = "Speed";
                }
            }
        }
        
        // Recursively process sub-state machines
        foreach (var subStateMachine in stateMachine.stateMachines)
        {
            OptimizeBlendTreesInStateMachine(subStateMachine.stateMachine);
        }
#endif
    }
    
    /// <summary>
    /// ✅ Validate components
    /// </summary>
    private bool ValidateComponents()
    {
        if (targetAnimator == null)
        {
            LogDebug("❌ Target Animator không được tìm thấy!");
            return false;
        }
        
        if (targetAnimator.runtimeAnimatorController == null)
        {
            LogDebug("❌ Animator Controller không được gán!");
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// 📊 Show optimization summary
    /// </summary>
    private void ShowOptimizationSummary()
    {
        string summary = "🎬 ANIMATOR OPTIMIZATION SUMMARY:\n\n";
        
        foreach (string log in optimizationLog)
        {
            summary += log + "\n";
        }
        
        summary += "\n✅ OPTIMIZATION COMPLETED!";
        summary += "\n\n🎯 APPLIED SETTINGS:";
        summary += $"\n• Transition Duration: {transitionDuration}s";
        summary += $"\n• Exit Time: {(disableExitTime ? "DISABLED" : "ENABLED")}";
        summary += $"\n• Interruption Source: {interruptionSource}";
        summary += "\n• Fixed Duration: ENABLED";
        summary += "\n• Ordered Interruption: ENABLED";
        
        Debug.Log(summary);
    }
    
    /// <summary>
    /// 📝 Log debug message
    /// </summary>
    private void LogDebug(string message)
    {
        if (debugMode)
        {
            Debug.Log($"🎬 AnimatorOptimizer: {message}");
        }
        optimizationLog.Add(message);
    }
    
    /// <summary>
    /// 🔄 Reset all transitions về default Unity settings
    /// </summary>
    [ContextMenu("Reset Transitions to Default")]
    public void ResetTransitionsToDefault()
    {
#if UNITY_EDITOR
        if (!ValidateComponents()) return;
        
        animatorController = targetAnimator.runtimeAnimatorController as AnimatorController;
        if (animatorController == null) return;
        
        LogDebug("🔄 Resetting transitions to default settings...");
        
        foreach (var layer in animatorController.layers)
        {
            ResetStateMachineTransitions(layer.stateMachine);
        }
        
        EditorUtility.SetDirty(animatorController);
        AssetDatabase.SaveAssets();
        
        LogDebug("✅ Reset completed!");
#endif
    }
    
    /// <summary>
    /// 🔄 Reset transitions trong state machine
    /// </summary>
    private void ResetStateMachineTransitions(AnimatorStateMachine stateMachine)
    {
#if UNITY_EDITOR
        foreach (var state in stateMachine.states)
        {
            foreach (var transition in state.state.transitions)
            {
                transition.duration = 0.25f; // Unity default
                transition.hasExitTime = true; // Unity default
                transition.interruptionSource = TransitionInterruptionSource.None; // Unity default
                transition.hasFixedDuration = true;
                transition.offset = 0f;
            }
        }
        
        foreach (var transition in stateMachine.anyStateTransitions)
        {
            transition.duration = 0.25f;
            transition.hasExitTime = true;
            transition.interruptionSource = TransitionInterruptionSource.None;
        }
        
        foreach (var subStateMachine in stateMachine.stateMachines)
        {
            ResetStateMachineTransitions(subStateMachine.stateMachine);
        }
#endif
    }
    
    /// <summary>
    /// 🔍 Analyze current animator controller configuration
    /// </summary>
    [ContextMenu("Analyze Current Configuration")]
    public void AnalyzeCurrentConfiguration()
    {
        if (!ValidateComponents()) return;
        
        LogDebug("🔍 Analyzing current Animator Controller configuration...");
        
#if UNITY_EDITOR
        animatorController = targetAnimator.runtimeAnimatorController as AnimatorController;
        if (animatorController == null) return;
        
        int totalTransitions = 0;
        int transitionsWithExitTime = 0;
        int slowTransitions = 0;
        
        foreach (var layer in animatorController.layers)
        {
            AnalyzeStateMachine(layer.stateMachine, ref totalTransitions, ref transitionsWithExitTime, ref slowTransitions);
        }
        
        string analysis = $"📊 ANALYZER RESULTS:\n";
        analysis += $"• Total Transitions: {totalTransitions}\n";
        analysis += $"• Transitions with Exit Time: {transitionsWithExitTime}\n";
        analysis += $"• Slow Transitions (>0.1s): {slowTransitions}\n";
        analysis += $"• Parameters: {animatorController.parameters.Length}\n";
        
        bool needsOptimization = transitionsWithExitTime > 0 || slowTransitions > 0;
        analysis += needsOptimization ? "\n⚠️ OPTIMIZATION RECOMMENDED!" : "\n✅ CONFIGURATION LOOKS GOOD!";
        
        Debug.Log(analysis);
#endif
    }
    
    /// <summary>
    /// 📊 Analyze state machine
    /// </summary>
    private void AnalyzeStateMachine(AnimatorStateMachine stateMachine, ref int totalTransitions, ref int transitionsWithExitTime, ref int slowTransitions)
    {
#if UNITY_EDITOR
        foreach (var state in stateMachine.states)
        {
            foreach (var transition in state.state.transitions)
            {
                totalTransitions++;
                if (transition.hasExitTime) transitionsWithExitTime++;
                if (transition.duration > 0.1f) slowTransitions++;
            }
        }
        
        foreach (var transition in stateMachine.anyStateTransitions)
        {
            totalTransitions++;
            if (transition.hasExitTime) transitionsWithExitTime++;
            if (transition.duration > 0.1f) slowTransitions++;
        }
        
        foreach (var subStateMachine in stateMachine.stateMachines)
        {
            AnalyzeStateMachine(subStateMachine.stateMachine, ref totalTransitions, ref transitionsWithExitTime, ref slowTransitions);
        }
#endif
    }

#if UNITY_EDITOR
    /// <summary>
    /// 🎨 Custom Inspector GUI
    /// </summary>
    [CustomEditor(typeof(AnimatorControllerOptimizer))]
    public class AnimatorControllerOptimizerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("🎬 Animator Controller Optimizer", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Optimize animation transitions để loại bỏ delay và lag cho AI NPC.", MessageType.Info);
            
            AnimatorControllerOptimizer optimizer = (AnimatorControllerOptimizer)target;
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("🚀 Optimize Animator Controller", GUILayout.Height(40)))
            {
                optimizer.OptimizeAnimatorController();
            }
            
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("🔍 Analyze Configuration"))
            {
                optimizer.AnalyzeCurrentConfiguration();
            }
            
            if (GUILayout.Button("🔄 Reset to Default"))
            {
                if (EditorUtility.DisplayDialog("Reset Confirmation", 
                    "Reset tất cả transitions về Unity default settings?", "Yes", "No"))
                {
                    optimizer.ResetTransitionsToDefault();
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "🎯 OPTIMIZATION TARGETS:\n" +
                "• Set Transition Duration = 0.05s\n" +
                "• Disable Exit Time\n" +
                "• Set Interruption Source = Current State\n" +
                "• Add missing Speed/IsWalking parameters", 
                MessageType.None);
        }
    }
#endif
}
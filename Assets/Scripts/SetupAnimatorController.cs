using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class SetupAnimatorController : MonoBehaviour
{
    public static void Execute()
    {
        // Load the existing Animator Controller
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Animation/CharacterAnimationController.controller");
        
        if (controller == null)
        {
            Debug.LogError("Animator Controller not found!");
            return;
        }
        
        // Clear existing states (optional)
        controller.layers[0].stateMachine.states = new ChildAnimatorState[0];
        controller.layers[0].stateMachine.anyStateTransitions = new AnimatorStateTransition[0];
        
        // Load animation clips
        AnimationClip idleClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animation/Idle.anim");
        AnimationClip walkClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animation/Walk.anim");
        AnimationClip attackClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animation/Attack.anim");
        AnimationClip hitClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animation/Hit.anim");
        AnimationClip deathClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animation/Death.anim");
        
        // Create states
        AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;
        
        // Idle State (Default)
        AnimatorState idleState = rootStateMachine.AddState("Idle");
        idleState.motion = idleClip;
        rootStateMachine.defaultState = idleState;
        
        // Walk State
        AnimatorState walkState = rootStateMachine.AddState("Walk");
        walkState.motion = walkClip;
        
        // Attack State
        AnimatorState attackState = rootStateMachine.AddState("Attack");
        attackState.motion = attackClip;
        
        // Hit State
        AnimatorState hitState = rootStateMachine.AddState("Hit");
        hitState.motion = hitClip;
        
        // Death State
        AnimatorState deathState = rootStateMachine.AddState("Death");
        deathState.motion = deathClip;
        
        // Add parameters
        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Hit", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Death", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("IsAlive", AnimatorControllerParameterType.Bool);
        
        // Set default parameter values
        controller.parameters[4].defaultBool = true; // IsAlive = true
        
        // Create transitions
        
        // Idle to Walk
        AnimatorStateTransition idleToWalk = idleState.AddTransition(walkState);
        idleToWalk.AddCondition(AnimatorConditionMode.Greater, 0.1f, "Speed");
        idleToWalk.hasExitTime = false;
        idleToWalk.duration = 0.2f;
        
        // Walk to Idle
        AnimatorStateTransition walkToIdle = walkState.AddTransition(idleState);
        walkToIdle.AddCondition(AnimatorConditionMode.Less, 0.1f, "Speed");
        walkToIdle.hasExitTime = false;
        walkToIdle.duration = 0.2f;
        
        // Any State to Attack
        AnimatorStateTransition anyToAttack = rootStateMachine.AddAnyStateTransition(attackState);
        anyToAttack.AddCondition(AnimatorConditionMode.If, 0, "Attack");
        anyToAttack.AddCondition(AnimatorConditionMode.If, 0, "IsAlive");
        anyToAttack.hasExitTime = false;
        anyToAttack.duration = 0.1f;
        
        // Attack back to Idle
        AnimatorStateTransition attackToIdle = attackState.AddTransition(idleState);
        attackToIdle.hasExitTime = true;
        attackToIdle.exitTime = 0.9f;
        attackToIdle.duration = 0.1f;
        
        // Any State to Hit
        AnimatorStateTransition anyToHit = rootStateMachine.AddAnyStateTransition(hitState);
        anyToHit.AddCondition(AnimatorConditionMode.If, 0, "Hit");
        anyToHit.AddCondition(AnimatorConditionMode.If, 0, "IsAlive");
        anyToHit.hasExitTime = false;
        anyToHit.duration = 0.05f;
        
        // Hit back to Idle
        AnimatorStateTransition hitToIdle = hitState.AddTransition(idleState);
        hitToIdle.hasExitTime = true;
        hitToIdle.exitTime = 0.8f;
        hitToIdle.duration = 0.1f;
        
        // Any State to Death
        AnimatorStateTransition anyToDeath = rootStateMachine.AddAnyStateTransition(deathState);
        anyToDeath.AddCondition(AnimatorConditionMode.If, 0, "Death");
        anyToDeath.hasExitTime = false;
        anyToDeath.duration = 0.1f;
        
        // States positioned automatically by Unity
        
        // Save changes
        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();
        
        Debug.Log("Animator Controller setup completed with new animations!");
    }
}
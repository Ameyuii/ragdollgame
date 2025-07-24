using UnityEngine;
using UnityEditor;

public class CreateAnimations : MonoBehaviour
{
    public static void Execute()
    {
        // Create Idle Animation
        CreateIdleAnimation();
        
        // Create Walk Animation
        CreateWalkAnimation();
        
        // Create Attack Animation
        CreateAttackAnimation();
        
        // Create Hit Animation
        CreateHitAnimation();
        
        // Create Death Animation
        CreateDeathAnimation();
        
        Debug.Log("All animations created successfully!");
    }
    
    static void CreateIdleAnimation()
    {
        AnimationClip clip = new AnimationClip();
        clip.name = "Idle";
        clip.wrapMode = WrapMode.Loop;
        
        // Create subtle breathing animation
        AnimationCurve chestCurve = AnimationCurve.Linear(0f, 0f, 2f, 0.05f);
        chestCurve.AddKey(4f, 0f);
        chestCurve.postWrapMode = WrapMode.Loop;
        chestCurve.preWrapMode = WrapMode.Loop;
        
        // Apply to spine bones
        clip.SetCurve("root/pelvis/spine_01/spine_02", typeof(Transform), "localScale.y", chestCurve);
        
        // Create slight head movement
        AnimationCurve headCurve = AnimationCurve.Linear(0f, 0f, 3f, 2f);
        headCurve.AddKey(6f, 0f);
        headCurve.postWrapMode = WrapMode.Loop;
        headCurve.preWrapMode = WrapMode.Loop;
        
        clip.SetCurve("root/pelvis/spine_01/spine_02/spine_03/neck_01/head", typeof(Transform), "localRotation.y", headCurve);
        
        // Save animation
        AssetDatabase.CreateAsset(clip, "Assets/Animation/Idle.anim");
        AssetDatabase.SaveAssets();
        
        Debug.Log("Idle animation created!");
    }
    
    static void CreateWalkAnimation()
    {
        AnimationClip clip = new AnimationClip();
        clip.name = "Walk";
        clip.wrapMode = WrapMode.Loop;
        
        // Create walking cycle for legs
        float walkDuration = 1.0f;
        
        // Left leg forward/back movement
        AnimationCurve leftLegCurve = new AnimationCurve();
        leftLegCurve.AddKey(0f, 0f);
        leftLegCurve.AddKey(walkDuration * 0.25f, 20f);
        leftLegCurve.AddKey(walkDuration * 0.5f, 0f);
        leftLegCurve.AddKey(walkDuration * 0.75f, -20f);
        leftLegCurve.AddKey(walkDuration, 0f);
        leftLegCurve.postWrapMode = WrapMode.Loop;
        leftLegCurve.preWrapMode = WrapMode.Loop;
        
        // Right leg opposite movement
        AnimationCurve rightLegCurve = new AnimationCurve();
        rightLegCurve.AddKey(0f, 0f);
        rightLegCurve.AddKey(walkDuration * 0.25f, -20f);
        rightLegCurve.AddKey(walkDuration * 0.5f, 0f);
        rightLegCurve.AddKey(walkDuration * 0.75f, 20f);
        rightLegCurve.AddKey(walkDuration, 0f);
        rightLegCurve.postWrapMode = WrapMode.Loop;
        rightLegCurve.preWrapMode = WrapMode.Loop;
        
        // Apply to leg bones
        clip.SetCurve("root/pelvis/thigh_l", typeof(Transform), "localRotation.x", leftLegCurve);
        clip.SetCurve("root/pelvis/thigh_r", typeof(Transform), "localRotation.x", rightLegCurve);
        
        // Arm swinging
        AnimationCurve leftArmCurve = new AnimationCurve();
        leftArmCurve.AddKey(0f, 0f);
        leftArmCurve.AddKey(walkDuration * 0.5f, -15f);
        leftArmCurve.AddKey(walkDuration, 0f);
        leftArmCurve.postWrapMode = WrapMode.Loop;
        leftArmCurve.preWrapMode = WrapMode.Loop;
        
        AnimationCurve rightArmCurve = new AnimationCurve();
        rightArmCurve.AddKey(0f, 0f);
        rightArmCurve.AddKey(walkDuration * 0.5f, 15f);
        rightArmCurve.AddKey(walkDuration, 0f);
        rightArmCurve.postWrapMode = WrapMode.Loop;
        rightArmCurve.preWrapMode = WrapMode.Loop;
        
        clip.SetCurve("root/pelvis/spine_01/spine_02/spine_03/clavicle_l/upperarm_l", typeof(Transform), "localRotation.x", leftArmCurve);
        clip.SetCurve("root/pelvis/spine_01/spine_02/spine_03/clavicle_r/upperarm_r", typeof(Transform), "localRotation.x", rightArmCurve);
        
        // Save animation
        AssetDatabase.CreateAsset(clip, "Assets/Animation/Walk.anim");
        AssetDatabase.SaveAssets();
        
        Debug.Log("Walk animation created!");
    }
    
    static void CreateAttackAnimation()
    {
        AnimationClip clip = new AnimationClip();
        clip.name = "Attack";
        clip.wrapMode = WrapMode.Once;
        
        float attackDuration = 0.8f;
        
        // Right arm punch animation
        AnimationCurve rightArmCurve = new AnimationCurve();
        rightArmCurve.AddKey(0f, 0f);
        rightArmCurve.AddKey(attackDuration * 0.3f, -90f); // Wind up
        rightArmCurve.AddKey(attackDuration * 0.6f, 45f);  // Punch forward
        rightArmCurve.AddKey(attackDuration, 0f);          // Return
        
        clip.SetCurve("root/pelvis/spine_01/spine_02/spine_03/clavicle_r/upperarm_r", typeof(Transform), "localRotation.x", rightArmCurve);
        
        // Body lean forward
        AnimationCurve bodyLeanCurve = new AnimationCurve();
        bodyLeanCurve.AddKey(0f, 0f);
        bodyLeanCurve.AddKey(attackDuration * 0.6f, 15f);
        bodyLeanCurve.AddKey(attackDuration, 0f);
        
        clip.SetCurve("root/pelvis/spine_01", typeof(Transform), "localRotation.x", bodyLeanCurve);
        
        // Save animation
        AssetDatabase.CreateAsset(clip, "Assets/Animation/Attack.anim");
        AssetDatabase.SaveAssets();
        
        Debug.Log("Attack animation created!");
    }
    
    static void CreateHitAnimation()
    {
        AnimationClip clip = new AnimationClip();
        clip.name = "Hit";
        clip.wrapMode = WrapMode.Once;
        
        float hitDuration = 0.5f;
        
        // Body recoil backward
        AnimationCurve bodyRecoilCurve = new AnimationCurve();
        bodyRecoilCurve.AddKey(0f, 0f);
        bodyRecoilCurve.AddKey(hitDuration * 0.3f, -20f);
        bodyRecoilCurve.AddKey(hitDuration, 0f);
        
        clip.SetCurve("root/pelvis/spine_01", typeof(Transform), "localRotation.x", bodyRecoilCurve);
        
        // Head snap back
        AnimationCurve headSnapCurve = new AnimationCurve();
        headSnapCurve.AddKey(0f, 0f);
        headSnapCurve.AddKey(hitDuration * 0.2f, -15f);
        headSnapCurve.AddKey(hitDuration, 0f);
        
        clip.SetCurve("root/pelvis/spine_01/spine_02/spine_03/neck_01/head", typeof(Transform), "localRotation.x", headSnapCurve);
        
        // Arms flail
        AnimationCurve armFlailCurve = new AnimationCurve();
        armFlailCurve.AddKey(0f, 0f);
        armFlailCurve.AddKey(hitDuration * 0.3f, 30f);
        armFlailCurve.AddKey(hitDuration, 0f);
        
        clip.SetCurve("root/pelvis/spine_01/spine_02/spine_03/clavicle_l/upperarm_l", typeof(Transform), "localRotation.z", armFlailCurve);
        clip.SetCurve("root/pelvis/spine_01/spine_02/spine_03/clavicle_r/upperarm_r", typeof(Transform), "localRotation.z", armFlailCurve);
        
        // Save animation
        AssetDatabase.CreateAsset(clip, "Assets/Animation/Hit.anim");
        AssetDatabase.SaveAssets();
        
        Debug.Log("Hit animation created!");
    }
    
    static void CreateDeathAnimation()
    {
        AnimationClip clip = new AnimationClip();
        clip.name = "Death";
        clip.wrapMode = WrapMode.Once;
        
        float deathDuration = 2.0f;
        
        // Body collapse forward
        AnimationCurve bodyCollapseCurve = new AnimationCurve();
        bodyCollapseCurve.AddKey(0f, 0f);
        bodyCollapseCurve.AddKey(deathDuration * 0.3f, 45f);
        bodyCollapseCurve.AddKey(deathDuration, 90f);
        
        clip.SetCurve("root/pelvis/spine_01", typeof(Transform), "localRotation.x", bodyCollapseCurve);
        
        // Head drop
        AnimationCurve headDropCurve = new AnimationCurve();
        headDropCurve.AddKey(0f, 0f);
        headDropCurve.AddKey(deathDuration * 0.5f, 30f);
        headDropCurve.AddKey(deathDuration, 45f);
        
        clip.SetCurve("root/pelvis/spine_01/spine_02/spine_03/neck_01/head", typeof(Transform), "localRotation.x", headDropCurve);
        
        // Arms drop
        AnimationCurve armDropCurve = new AnimationCurve();
        armDropCurve.AddKey(0f, 0f);
        armDropCurve.AddKey(deathDuration * 0.4f, 90f);
        armDropCurve.AddKey(deathDuration, 120f);
        
        clip.SetCurve("root/pelvis/spine_01/spine_02/spine_03/clavicle_l/upperarm_l", typeof(Transform), "localRotation.x", armDropCurve);
        clip.SetCurve("root/pelvis/spine_01/spine_02/spine_03/clavicle_r/upperarm_r", typeof(Transform), "localRotation.x", armDropCurve);
        
        // Legs buckle
        AnimationCurve legBuckleCurve = new AnimationCurve();
        legBuckleCurve.AddKey(0f, 0f);
        legBuckleCurve.AddKey(deathDuration * 0.2f, 45f);
        legBuckleCurve.AddKey(deathDuration, 90f);
        
        clip.SetCurve("root/pelvis/thigh_l", typeof(Transform), "localRotation.x", legBuckleCurve);
        clip.SetCurve("root/pelvis/thigh_r", typeof(Transform), "localRotation.x", legBuckleCurve);
        
        // Save animation
        AssetDatabase.CreateAsset(clip, "Assets/Animation/Death.anim");
        AssetDatabase.SaveAssets();
        
        Debug.Log("Death animation created!");
    }
}
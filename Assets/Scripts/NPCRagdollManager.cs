using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Quản lý ragdoll cho tất cả NPC trong scene
/// Tự động phát hiện và setup ragdoll, tích hợp với animation tấn công
/// </summary>
public class NPCRagdollManager : MonoBehaviour
{
    [Header("NPC Ragdoll Settings")]
    [SerializeField, Tooltip("Tự động tìm và setup ragdoll cho tất cả NPC")]
    private bool tuDongSetupNPC = true;
    
    [SerializeField, Tooltip("Tag của các NPC trong scene")]
    private string tagNPC = "NPC";
    
    [SerializeField, Tooltip("Lực tấn công mặc định")]
    private float lucTanCong = 20f;
    
    [SerializeField, Tooltip("Thời gian ragdoll sau khi bị tấn công")]
    private float thoiGianRagdoll = 3f;
    
    [Header("Animation Integration")]
    [SerializeField, Tooltip("Tên trigger animation tấn công")]
    private string triggerTanCong = "TanCong";
    
    [SerializeField, Tooltip("Tên parameter animation bị hit")]
    private string parameterBiHit = "BiHit";
      [Header("Debug")]
    [SerializeField, Tooltip("Hiển thị debug info")]
    private bool hienThiDebug = false; // TẮT UI để tránh xung đột với Camera Settings
    
    // Danh sách các NPC và RagdollController của chúng
    private Dictionary<GameObject, RagdollController> danhSachNPCRagdoll = new Dictionary<GameObject, RagdollController>();
    private Dictionary<GameObject, Animator> danhSachNPCAnimator = new Dictionary<GameObject, Animator>();
    
    void Start()
    {
        if (tuDongSetupNPC)
        {
            TimVaSetupTatCaNPC();
        }
    }
    
    /// <summary>
    /// Tìm và setup ragdoll cho tất cả NPC trong scene
    /// </summary>
    private void TimVaSetupTatCaNPC()
    {
        // Tìm tất cả GameObject có tag NPC
        GameObject[] tatCaNPC = GameObject.FindGameObjectsWithTag(tagNPC);
          if (tatCaNPC.Length == 0)
        {
            // Nếu không có tag NPC, tìm theo tên
            tatCaNPC = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None)
                .Where(go => go.name.ToLower().Contains("npc"))
                .ToArray();
        }
        
        Debug.Log($"🎯 Tìm thấy {tatCaNPC.Length} NPC trong scene");
        
        foreach (GameObject npc in tatCaNPC)
        {
            SetupRagdollChoNPC(npc);
        }
        
        if (hienThiDebug)
        {
            Debug.Log($"✅ Đã setup ragdoll cho {danhSachNPCRagdoll.Count} NPC");
        }
    }
    
    /// <summary>
    /// Setup ragdoll cho một NPC cụ thể
    /// </summary>
    private void SetupRagdollChoNPC(GameObject npc)
    {
        if (npc == null) return;
        
        // Kiểm tra xem đã có RagdollController chưa
        RagdollController ragdollController = npc.GetComponent<RagdollController>();
        if (ragdollController == null)
        {
            ragdollController = npc.AddComponent<RagdollController>();
        }
        
        // Lấy Animator nếu có
        Animator animator = npc.GetComponent<Animator>();
        
        // Thêm vào danh sách quản lý
        danhSachNPCRagdoll[npc] = ragdollController;
        if (animator != null)
        {
            danhSachNPCAnimator[npc] = animator;
        }
        
        // Setup NPCHealthComponent để handle damage
        NPCHealthComponent health = npc.GetComponent<NPCHealthComponent>();
        if (health == null)
        {
            health = npc.AddComponent<NPCHealthComponent>();
        }
        
        // Đăng ký sự kiện khi NPC bị tấn công
        health.OnNPCBiTanCong += (huongTanCong, viTriTanCong, lucTanCong) => 
        {
            XuLyNPCBiTanCong(npc, huongTanCong, viTriTanCong, lucTanCong);
        };
        
        if (hienThiDebug)
        {
            Debug.Log($"🔧 Setup ragdoll cho NPC: {npc.name}");
        }
    }
    
    /// <summary>
    /// Xử lý khi NPC bị tấn công
    /// </summary>
    private void XuLyNPCBiTanCong(GameObject npc, Vector3 huongTanCong, Vector3 viTriTanCong, float luc)
    {
        if (!danhSachNPCRagdoll.ContainsKey(npc)) return;
        
        RagdollController ragdollController = danhSachNPCRagdoll[npc];
        
        // Kích hoạt ragdoll với lực tấn công
        ragdollController.KichHoatRagdoll(huongTanCong * luc, viTriTanCong);
        
        // Trigger animation bị hit nếu có animator
        if (danhSachNPCAnimator.ContainsKey(npc))
        {
            Animator animator = danhSachNPCAnimator[npc];
            if (animator != null && animator.parameters.Any(p => p.name == parameterBiHit))
            {
                animator.SetTrigger(parameterBiHit);
            }
        }
        
        // Tự động khôi phục sau thời gian
        StartCoroutine(KhoiPhucNPCSauThoiGian(npc, thoiGianRagdoll));
        
        if (hienThiDebug)
        {
            Debug.Log($"💥 NPC {npc.name} bị tấn công - Kích hoạt ragdoll");
        }
    }
    
    /// <summary>
    /// Khôi phục NPC về trạng thái bình thường sau thời gian
    /// </summary>
    private System.Collections.IEnumerator KhoiPhucNPCSauThoiGian(GameObject npc, float thoiGian)
    {
        yield return new WaitForSeconds(thoiGian);
        
        if (npc != null && danhSachNPCRagdoll.ContainsKey(npc))
        {
            RagdollController ragdollController = danhSachNPCRagdoll[npc];
            ragdollController.KhoiPhucAnimation();
            
            if (hienThiDebug)
            {
                Debug.Log($"🔄 NPC {npc.name} đã khôi phục animation");
            }
        }
    }
    
    /// <summary>
    /// Tấn công một NPC cụ thể (để test hoặc gọi từ script khác)
    /// </summary>
    public void TanCongNPC(GameObject npc, Vector3 huongTanCong, Vector3? viTriTanCong = null)
    {
        if (npc == null) return;
        
        Vector3 viTri = viTriTanCong ?? (npc.transform.position + Vector3.up * 1.5f);
        
        NPCHealthComponent health = npc.GetComponent<NPCHealthComponent>();
        if (health != null)
        {
            health.NhanTanCong(huongTanCong, viTri, lucTanCong);
        }
    }
    
    /// <summary>
    /// Tấn công tất cả NPC trong scene (để test)
    /// </summary>
    public void TanCongTatCaNPC()
    {
        foreach (var npc in danhSachNPCRagdoll.Keys)
        {
            Vector3 huongTanCong = (npc.transform.position - transform.position).normalized;
            TanCongNPC(npc, huongTanCong);
        }
        
        if (hienThiDebug)
        {
            Debug.Log($"💥 Đã tấn công tất cả {danhSachNPCRagdoll.Count} NPC");
        }
    }
      /// <summary>
    /// Tìm NPC gần nhất và tấn công
    /// </summary>
    public void TanCongNPCGanNhat()
    {
        GameObject? npcGanNhat = null;
        float khoangCachGanNhat = float.MaxValue;
        
        foreach (var npc in danhSachNPCRagdoll.Keys)
        {
            float khoangCach = Vector3.Distance(transform.position, npc.transform.position);
            if (khoangCach < khoangCachGanNhat)
            {
                khoangCachGanNhat = khoangCach;
                npcGanNhat = npc;
            }
        }
        
        if (npcGanNhat != null)
        {
            Vector3 huongTanCong = (npcGanNhat.transform.position - transform.position).normalized;
            TanCongNPC(npcGanNhat, huongTanCong);
        }
    }
    
    #region Debug UI
    
    void OnGUI()
    {
        if (!hienThiDebug) return;
        
        GUILayout.BeginArea(new Rect(10, 220, 300, 150));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("NPC Ragdoll Manager");
        GUILayout.Label($"Số NPC được quản lý: {danhSachNPCRagdoll.Count}");
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("Tấn công tất cả NPC"))
        {
            TanCongTatCaNPC();
        }
        
        if (GUILayout.Button("Tấn công NPC gần nhất"))
        {
            TanCongNPCGanNhat();
        }
        
        GUILayout.Space(5);
        GUILayout.Label("Keyboard Controls:");
        GUILayout.Label("X - Tấn công tất cả NPC");
        GUILayout.Label("C - Tấn công NPC gần nhất");
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
    
    #endregion
    
    void Update()
    {
        // Keyboard shortcuts để test
        if (UnityEngine.InputSystem.Keyboard.current != null)
        {
            // X - Tấn công tất cả NPC
            if (UnityEngine.InputSystem.Keyboard.current.xKey.wasPressedThisFrame)
            {
                TanCongTatCaNPC();
            }
            
            // C - Tấn công NPC gần nhất
            if (UnityEngine.InputSystem.Keyboard.current.cKey.wasPressedThisFrame)
            {
                TanCongNPCGanNhat();
            }
        }
    }
}

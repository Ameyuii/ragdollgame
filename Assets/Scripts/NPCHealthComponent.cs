using UnityEngine;
using System;

/// <summary>
/// Component quản lý sức khỏe và damage cho NPC
/// Tích hợp với ragdoll system khi nhận damage
/// </summary>
public class NPCHealthComponent : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField, Tooltip("Máu tối đa của NPC")]
    private float mauToiDa = 100f;
    
    [SerializeField, Tooltip("Máu hiện tại")]
    private float mauHienTai = 100f;
    
    [Header("Damage Settings")]
    [SerializeField, Tooltip("Damage tối thiểu để kích hoạt ragdoll")]
    private float damageToiThieuChoRagdoll = 20f;
    
    [SerializeField, Tooltip("Thời gian bất tử sau khi nhận damage")]
    private float thoiGianBatTu = 1f;
    
    [Header("Effects")]
    [SerializeField, Tooltip("Hiệu ứng khi bị tấn công")]
    private ParticleSystem hieUUngBiTanCong;
    
    [SerializeField, Tooltip("Âm thanh khi bị tấn công")]
    private AudioClip amThanhBiTanCong;
      // Events
    public event Action<Vector3, Vector3, float>? OnNPCBiTanCong; // hướng, vị trí, lực
    public event Action? OnNPCChet;
    
    private bool dangBatTu = false;
    private float thoiGianBatDauBatTu = 0f;
    private AudioSource audioSource;
    
    // Properties
    public float MauHienTai => mauHienTai;
    public float MauToiDa => mauToiDa;
    public bool DaChet => mauHienTai <= 0f;
    public float TyLeMau => mauHienTai / mauToiDa;    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && amThanhBiTanCong != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    void Start()
    {
        // Đồng bộ health với NPCController trong Start() để đảm bảo NPCController đã khởi tạo xong
        NPCController npcController = GetComponent<NPCController>();
        if (npcController != null)
        {
            // Sử dụng health từ NPCController thay vì tự quản lý
            mauToiDa = npcController.maxHealth;
            mauHienTai = npcController.currentHealth;
            
            Debug.Log($"🔄 NPCHealthComponent đồng bộ với NPCController health: {mauHienTai}/{mauToiDa}");
        }
        else
        {
            mauHienTai = mauToiDa;
            Debug.Log($"⚠️ NPCHealthComponent không tìm thấy NPCController, sử dụng health mặc định: {mauHienTai}/{mauToiDa}");
        }
    }
    
    void Update()
    {
        // Cập nhật trạng thái bất tử
        if (dangBatTu && Time.time - thoiGianBatDauBatTu >= thoiGianBatTu)
        {
            dangBatTu = false;
        }
    }
      /// <summary>
    /// Nhận damage từ tấn công
    /// </summary>
    /// <param name="damage">Lượng damage</param>
    /// <param name="huongTanCong">Hướng tấn công (normalized)</param>
    /// <param name="viTriTanCong">Vị trí bị tấn công</param>
    /// <param name="nguoiTanCong">Đối tượng tấn công (optional)</param>
    public void NhanDamage(float damage, Vector3 huongTanCong, Vector3 viTriTanCong, GameObject? nguoiTanCong = null)
    {
        // Kiểm tra bất tử
        if (dangBatTu || DaChet) return;
          // Đồng bộ với NPCController nếu có
        NPCController npcController = GetComponent<NPCController>();
        if (npcController != null)
        {            // Lấy NPCController từ attacker nếu có
            NPCController? attackerController = nguoiTanCong?.GetComponent<NPCController>();
            
            // Sử dụng NPCController's TakeDamage method thay vì tự trừ máu
            // Sử dụng null! để bypass null warning vì NPCController.TakeDamage có thể handle null
            npcController.TakeDamage(damage, attackerController!);
            
            // Cập nhật health từ NPCController
            mauHienTai = npcController.currentHealth;
            
            Debug.Log($"🔄 Đồng bộ health với NPCController: {mauHienTai}/{mauToiDa}");
        }
        else
        {
            // Trừ máu trực tiếp nếu không có NPCController
            mauHienTai = Mathf.Max(0f, mauHienTai - damage);
        }
        
        // Bắt đầu thời gian bất tử
        dangBatTu = true;
        thoiGianBatDauBatTu = Time.time;
        
        // Tính lực ragdoll dựa trên damage
        float lucRagdoll = Mathf.Max(damage, damageToiThieuChoRagdoll);
        
        // Trigger ragdoll nếu damage đủ lớn
        if (damage >= damageToiThieuChoRagdoll)
        {
            OnNPCBiTanCong?.Invoke(huongTanCong, viTriTanCong, lucRagdoll);
        }
        
        // Phát hiệu ứng
        PhatHieuUngBiTanCong(viTriTanCong);
        
        // Phát âm thanh
        if (audioSource != null && amThanhBiTanCong != null)
        {
            audioSource.PlayOneShot(amThanhBiTanCong);
        }
        
        // Kiểm tra chết (sử dụng NPCController's status nếu có)
        bool dachet = npcController != null ? npcController.IsDead() : DaChet;
        if (dachet)
        {
            OnNPCChet?.Invoke();
            if (npcController == null) // Chỉ xử lý chết nếu không có NPCController
            {
                XuLyNPCChet();
            }
        }
        
        Debug.Log($"💔 {gameObject.name} nhận {damage} damage. Máu còn: {mauHienTai}/{mauToiDa}");
    }
    
    /// <summary>
    /// Nhận tấn công (wrapper cho NhanDamage với giá trị mặc định)
    /// </summary>
    public void NhanTanCong(Vector3 huongTanCong, Vector3 viTriTanCong, float luc)
    {
        // Tính damage dựa trên lực
        float damage = luc * 2f; // Có thể điều chỉnh tỷ lệ này
        NhanDamage(damage, huongTanCong, viTriTanCong);
    }
    
    /// <summary>
    /// Hồi máu
    /// </summary>
    public void HoiMau(float luongHoi)
    {
        if (DaChet) return;
        
        mauHienTai = Mathf.Min(mauToiDa, mauHienTai + luongHoi);
        Debug.Log($"💚 {gameObject.name} hồi {luongHoi} máu. Máu hiện tại: {mauHienTai}/{mauToiDa}");
    }
    
    /// <summary>
    /// Reset về trạng thái ban đầu
    /// </summary>
    public void ResetTrangThai()
    {
        mauHienTai = mauToiDa;
        dangBatTu = false;
        Debug.Log($"🔄 {gameObject.name} đã reset trạng thái");
    }
    
    /// <summary>
    /// Phát hiệu ứng khi bị tấn công
    /// </summary>
    private void PhatHieuUngBiTanCong(Vector3 viTri)
    {
        if (hieUUngBiTanCong != null)
        {
            // Di chuyển particle system đến vị trí bị tấn công
            hieUUngBiTanCong.transform.position = viTri;
            hieUUngBiTanCong.Play();
        }
        else
        {
            // Tạo hiệu ứng đơn giản bằng debug visualization
            Debug.DrawRay(viTri, Vector3.up * 2f, Color.red, 1f);
        }
    }
    
    /// <summary>
    /// Xử lý khi NPC chết
    /// </summary>
    private void XuLyNPCChet()
    {
        Debug.Log($"💀 {gameObject.name} đã chết!");
        
        // Có thể thêm logic khác như:
        // - Tắt AI
        // - Spawn loot
        // - Trigger death animation
        // - Disable movement
        
        // Tạm thời disable movement components
        var navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
        }
        
        // Disable collider chính để không block movement
        var mainCollider = GetComponent<Collider>();
        if (mainCollider != null && !mainCollider.isTrigger)
        {
            mainCollider.enabled = false;
        }
    }
    
    /// <summary>
    /// Kiểm tra có thể nhận damage không
    /// </summary>
    public bool CoTheNhanDamage()
    {
        return !dangBatTu && !DaChet;
    }
    
    void OnDrawGizmosSelected()
    {
        // Vẽ health bar đơn giản
        if (Application.isPlaying)
        {
            Vector3 viTriHealthBar = transform.position + Vector3.up * 2.5f;
            
            // Background
            Gizmos.color = Color.red;
            Gizmos.DrawCube(viTriHealthBar, new Vector3(2f, 0.2f, 0.1f));
            
            // Health bar
            Gizmos.color = Color.green;
            float tyLe = TyLeMau;
            Gizmos.DrawCube(viTriHealthBar - Vector3.right * (1f - tyLe), 
                           new Vector3(2f * tyLe, 0.15f, 0.05f));
        }
    }
}

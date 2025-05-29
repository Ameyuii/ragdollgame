using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Hệ thống quản lý Ragdoll hybrid - kết hợp animation và vật lý
/// Cho phép chuyển đổi mượt mà giữa animated và physics-based movement
/// </summary>
public class RagdollController : MonoBehaviour
{
    [Header("Cấu hình Ragdoll")]
    [SerializeField, Tooltip("Animator component của nhân vật")]
    private Animator? animator;
    
    [SerializeField, Tooltip("Thời gian để chuyển từ animation sang ragdoll")]
    private float thoiGianChuyenSangRagdoll = 0.5f;
    
    [SerializeField, Tooltip("Thời gian để khôi phục từ ragdoll về animation")]
    private float thoiGianKhoiPhucAnimation = 2f;
    
    [SerializeField, Tooltip("Lực tác động tối thiểu để kích hoạt ragdoll")]
    private float lucKichHoatRagdoll = 10f;
    
    [SerializeField, Tooltip("Tỷ lệ blend giữa animation và physics (0 = full animation, 1 = full physics)")]
    [Range(0f, 1f)]
    private float tyLeBlendVatLy = 0f;
    
    [Header("Phụ thuộc Components")]
    [SerializeField, Tooltip("Rigidbody chính của nhân vật")]
    private Rigidbody? rigidbodyNhanVat;
    
    [SerializeField, Tooltip("Collider chính của nhân vật")]
    private Collider? colliderNhanVat;
    
    [Header("Cấu hình Chết")]
    [SerializeField, Tooltip("Thời gian tồn tại sau khi chết (0 = vĩnh viễn)")]
    private float thoiGianTonTaiSauKhiChet = 0f;
    
    [SerializeField, Tooltip("Có tự động chuyển sang ragdoll khi chết không")]
    private bool tuDongRagdollKhiChet = true;
    
    [SerializeField, Tooltip("Có vô hiệu hóa AI/Input sau khi chết không")]
    private bool voHieuHoaControlKhiChet = true;
    
    [Header("Debug")]
    [SerializeField, Tooltip("Hiển thị thông tin debug")]
    private bool hienThiDebug = true;
    
    // Trạng thái ragdoll
    public enum TrangThaiRagdoll
    {
        Animation,      // Chỉ dùng animation
        ChuyenDoiSangRagdoll,  // Đang chuyển từ animation sang ragdoll
        Ragdoll,        // Chỉ dùng physics
        KhoiPhucAnimation,     // Đang khôi phục về animation
        Hybrid,         // Kết hợp animation và physics
        Chet            // Nhân vật đã chết - ragdoll vĩnh viễn
    }
    
    private TrangThaiRagdoll trangThaiHienTai = TrangThaiRagdoll.Animation;
    private Dictionary<Rigidbody, Vector3> viTriGocCacBoPhan = new Dictionary<Rigidbody, Vector3>();
    private Dictionary<Rigidbody, Quaternion> xoayGocCacBoPhan = new Dictionary<Rigidbody, Quaternion>();
    private List<Rigidbody> danhSachRigidbody = new List<Rigidbody>();
    private List<Collider> danhSachCollider = new List<Collider>();
    private float thoiGianBatDauChuyenDoi = 0f;
    private bool daChet = false;
    private float thoiGianChet = 0f;
      // References cho việc vô hiệu hóa control
    private MonoBehaviour[]? cacScriptCanVoHieuHoa;
    private CharacterController? characterController;
    
    // Properties
    public TrangThaiRagdoll TrangThai => trangThaiHienTai;
    public bool DangLaRagdoll => trangThaiHienTai == TrangThaiRagdoll.Ragdoll;
    public bool DangLaAnimation => trangThaiHienTai == TrangThaiRagdoll.Animation;
    public float TyLeBlendVatLy => tyLeBlendVatLy;
    public bool DaChet => daChet;
    public bool CoDangSong => !daChet && trangThaiHienTai != TrangThaiRagdoll.Chet;
    
    void Awake()
    {
        // Tự động tìm các component nếu chưa được gán
        if (animator == null)
            animator = GetComponent<Animator>();
            
        if (rigidbodyNhanVat == null)
            rigidbodyNhanVat = GetComponent<Rigidbody>();
            
        if (colliderNhanVat == null)
            colliderNhanVat = GetComponent<Collider>();
            
        // Tìm CharacterController nếu có
        characterController = GetComponent<CharacterController>();
        
        // Tìm các script cần vô hiệu hóa khi chết (AI, Input controllers, etc.)
        TimCacScriptCanVoHieuHoa();
    }
    
    void Start()
    {
        KhoiTaoRagdollSystem();
        
        // Nếu không tìm thấy ragdoll bodies, tự động tạo test setup
        if (danhSachRigidbody.Count == 0)
        {
            TaoRagdollTestDonGian();
        }
    }
      void Update()
    {
        // Nếu đã chết và có thời gian tồn tại, kiểm tra xem có cần xóa không
        if (daChet && thoiGianTonTaiSauKhiChet > 0)
        {
            if (Time.time - thoiGianChet >= thoiGianTonTaiSauKhiChet)
            {
                XoaNhanVat();
                return;
            }
        }
        
        // Chỉ cập nhật ragdoll nếu chưa chết hoặc đang ở trạng thái chết ragdoll
        if (!daChet || trangThaiHienTai == TrangThaiRagdoll.Chet)
        {
            CapNhatTrangThaiRagdoll();
        }
        
        // if (hienThiDebug)
        // {
        //     HienThiThongTinDebug();
        // }
    }
    
    /// <summary>
    /// Khởi tạo hệ thống ragdoll - tìm và setup các Rigidbody và Collider
    /// </summary>
    private void KhoiTaoRagdollSystem()
    {
        // Tìm tất cả Rigidbody con (ngoại trừ rigidbody chính)
        Rigidbody[] tatCaRigidbody = GetComponentsInChildren<Rigidbody>();
        foreach (var rb in tatCaRigidbody)
        {
            if (rb != rigidbodyNhanVat)
            {
                danhSachRigidbody.Add(rb);
                // Lưu vị trí và rotation gốc
                viTriGocCacBoPhan[rb] = rb.transform.localPosition;
                xoayGocCacBoPhan[rb] = rb.transform.localRotation;
            }
        }
        
        // Tìm tất cả Collider của ragdoll (ngoại trừ collider chính)
        Collider[] tatCaCollider = GetComponentsInChildren<Collider>();
        foreach (var col in tatCaCollider)
        {
            if (col != colliderNhanVat)
            {
                danhSachCollider.Add(col);
            }
        }
        
        // Khởi tạo ở trạng thái animation
        ThietLapTrangThaiAnimation();
        
        Debug.Log($"Đã khởi tạo Ragdoll System với {danhSachRigidbody.Count} Rigidbody và {danhSachCollider.Count} Collider");
    }
      /// <summary>
    /// Cập nhật trạng thái ragdoll theo thời gian
    /// </summary>
    private void CapNhatTrangThaiRagdoll()
    {
        // Không cập nhật nếu đã chết
        if (daChet && trangThaiHienTai == TrangThaiRagdoll.Chet)
            return;
            
        float thoiGianDaTroi = Time.time - thoiGianBatDauChuyenDoi;
        
        switch (trangThaiHienTai)
        {
            case TrangThaiRagdoll.ChuyenDoiSangRagdoll:
                float tienTrinhChuyenSangRagdoll = Mathf.Clamp01(thoiGianDaTroi / thoiGianChuyenSangRagdoll);
                tyLeBlendVatLy = Mathf.Lerp(0f, 1f, tienTrinhChuyenSangRagdoll);
                
                if (tienTrinhChuyenSangRagdoll >= 1f)
                {
                    ChuyenSangTrangThaiRagdoll();
                }
                break;
                
            case TrangThaiRagdoll.KhoiPhucAnimation:
                float tienTrinhKhoiPhuc = Mathf.Clamp01(thoiGianDaTroi / thoiGianKhoiPhucAnimation);
                tyLeBlendVatLy = Mathf.Lerp(1f, 0f, tienTrinhKhoiPhuc);
                
                // Lerp position và rotation về vị trí animation
                LerpVeViTriAnimation(tienTrinhKhoiPhuc);
                
                if (tienTrinhKhoiPhuc >= 1f)
                {
                    ChuyenSangTrangThaiAnimation();
                }
                break;
                
            case TrangThaiRagdoll.Hybrid:
                // Trong chế độ hybrid, có thể điều chỉnh tyLeBlendVatLy từ bên ngoài
                ApDungBlendVatLy();
                break;
        }
    }
    
    /// <summary>
    /// Thiết lập trạng thái animation (tắt ragdoll physics)
    /// </summary>
    private void ThietLapTrangThaiAnimation()
    {
        if (animator != null)
            animator.enabled = true;
            
        // Tắt physics cho các bộ phận ragdoll
        foreach (var rb in danhSachRigidbody)
        {
            rb.isKinematic = true;
        }
        
        // Tắt collision giữa các bộ phận ragdoll
        foreach (var col in danhSachCollider)
        {
            col.enabled = false;
        }
        
        // Bật main collider và rigidbody
        if (colliderNhanVat != null)
            colliderNhanVat.enabled = true;
            
        if (rigidbodyNhanVat != null)
            rigidbodyNhanVat.isKinematic = false;
            
        tyLeBlendVatLy = 0f;
        trangThaiHienTai = TrangThaiRagdoll.Animation;
    }
    
    /// <summary>
    /// Thiết lập trạng thái ragdoll (bật physics)
    /// </summary>
    private void ThietLapTrangThaiRagdoll()
    {
        if (animator != null)
            animator.enabled = false;
            
        // Bật physics cho các bộ phận ragdoll
        foreach (var rb in danhSachRigidbody)
        {
            rb.isKinematic = false;
        }
        
        // Bật collision cho các bộ phận ragdoll
        foreach (var col in danhSachCollider)
        {
            col.enabled = true;
        }
        
        // Tắt main collider và rigidbody
        if (colliderNhanVat != null)
            colliderNhanVat.enabled = false;
            
        if (rigidbodyNhanVat != null)
            rigidbodyNhanVat.isKinematic = true;
            
        tyLeBlendVatLy = 1f;
        trangThaiHienTai = TrangThaiRagdoll.Ragdoll;
    }
    
    /// <summary>
    /// Áp dụng blend giữa animation và physics
    /// </summary>
    private void ApDungBlendVatLy()
    {
        // Logic blend sẽ được implement trong các frame tiếp theo
        // Ở đây ta chỉ cập nhật trạng thái animator
        if (animator != null)
        {
            animator.enabled = tyLeBlendVatLy < 0.9f;
        }
    }
    
    /// <summary>
    /// Lerp các bộ phận ragdoll về vị trí animation gốc
    /// </summary>
    private void LerpVeViTriAnimation(float tienTrinh)
    {
        foreach (var rb in danhSachRigidbody)
        {
            if (viTriGocCacBoPhan.ContainsKey(rb) && xoayGocCacBoPhan.ContainsKey(rb))
            {
                Vector3 viTriMucTieu = rb.transform.parent.TransformPoint(viTriGocCacBoPhan[rb]);
                Quaternion xoayMucTieu = rb.transform.parent.rotation * xoayGocCacBoPhan[rb];
                
                rb.transform.position = Vector3.Lerp(rb.transform.position, viTriMucTieu, tienTrinh);
                rb.transform.rotation = Quaternion.Lerp(rb.transform.rotation, xoayMucTieu, tienTrinh);
                
                // Giảm velocity khi đang khôi phục
                rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, tienTrinh);
                rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, tienTrinh);
            }
        }
    }
    
    /// <summary>
    /// Chuyển sang trạng thái ragdoll
    /// </summary>
    private void ChuyenSangTrangThaiRagdoll()
    {
        ThietLapTrangThaiRagdoll();
        Debug.Log("Đã chuyển sang trạng thái Ragdoll");
    }
    
    /// <summary>
    /// Chuyển sang trạng thái animation
    /// </summary>
    private void ChuyenSangTrangThaiAnimation()
    {
        ThietLapTrangThaiAnimation();
        Debug.Log("Đã chuyển sang trạng thái Animation");
    }
    
    // ========== PUBLIC METHODS ==========
      /// <summary>
    /// Kích hoạt ragdoll với lực tác động
    /// </summary>
    /// <param name="lucTacDong">Vector lực tác động</param>
    /// <param name="viTriTacDong">Vị trí tác động lực</param>
    public void KichHoatRagdoll(Vector3 lucTacDong, Vector3 viTriTacDong)
    {
        // Không kích hoạt ragdoll nếu đã chết
        if (daChet) return;
        
        if (trangThaiHienTai == TrangThaiRagdoll.Animation && lucTacDong.magnitude >= lucKichHoatRagdoll)
        {
            trangThaiHienTai = TrangThaiRagdoll.ChuyenDoiSangRagdoll;
            thoiGianBatDauChuyenDoi = Time.time;
            
            // Áp dụng lực sau khi chuyển sang ragdoll
            StartCoroutine(ApDungLucSauKhiChuyenDoi(lucTacDong, viTriTacDong));
            
            Debug.Log($"Kích hoạt Ragdoll với lực: {lucTacDong.magnitude:F2}");
        }
    }
      /// <summary>
    /// Kích hoạt ragdoll ngay lập tức
    /// </summary>
    public void KichHoatRagdollNgayLapTuc()
    {
        // Không kích hoạt nếu đã chết
        if (daChet) return;
        
        if (trangThaiHienTai == TrangThaiRagdoll.Animation)
        {
            ThietLapTrangThaiRagdoll();
            Debug.Log("Kích hoạt Ragdoll ngay lập tức");
        }
    }

    /// <summary>
    /// Khôi phục về animation
    /// </summary>
    public void KhoiPhucAnimation()
    {
        // Không khôi phục nếu đã chết
        if (daChet) return;
        
        if (trangThaiHienTai == TrangThaiRagdoll.Ragdoll)
        {
            trangThaiHienTai = TrangThaiRagdoll.KhoiPhucAnimation;
            thoiGianBatDauChuyenDoi = Time.time;
            Debug.Log("Bắt đầu khôi phục Animation");
        }
    }
    
    /// <summary>
    /// Chuyển sang chế độ hybrid (kết hợp animation và physics)
    /// </summary>
    /// <param name="tyLe">Tỷ lệ physics (0 = full animation, 1 = full physics)</param>
    public void ChuyenSangCheDoHybrid(float tyLe = 0.5f)
    {
        trangThaiHienTai = TrangThaiRagdoll.Hybrid;
        tyLeBlendVatLy = Mathf.Clamp01(tyLe);
        ApDungBlendVatLy();
        Debug.Log($"Chuyển sang chế độ Hybrid với tỷ lệ physics: {tyLeBlendVatLy:F2}");
    }
    
    /// <summary>
    /// Điều chỉnh tỷ lệ blend trong chế độ hybrid
    /// </summary>
    /// <param name="tyLeMoi">Tỷ lệ mới (0-1)</param>
    public void DieuChinhTyLeBlend(float tyLeMoi)
    {
        if (trangThaiHienTai == TrangThaiRagdoll.Hybrid)
        {
            tyLeBlendVatLy = Mathf.Clamp01(tyLeMoi);
            ApDungBlendVatLy();
        }
    }
    
    /// <summary>
    /// Coroutine để áp dụng lực sau khi chuyển sang ragdoll
    /// </summary>
    private IEnumerator ApDungLucSauKhiChuyenDoi(Vector3 lucTacDong, Vector3 viTriTacDong)
    {        yield return new WaitForSeconds(thoiGianChuyenSangRagdoll);
        
        // Tìm Rigidbody gần nhất với vị trí tác động
        Rigidbody? rbGanNhat = TimRigidbodyGanNhat(viTriTacDong);
        if (rbGanNhat != null)
        {
            rbGanNhat.AddForceAtPosition(lucTacDong, viTriTacDong, ForceMode.Impulse);
            Debug.Log($"Đã áp dụng lực tại {rbGanNhat.name}");
        }
    }
    
    /// <summary>
    /// Tìm Rigidbody gần nhất với vị trí cho trước
    /// </summary>
    private Rigidbody? TimRigidbodyGanNhat(Vector3 viTri)
    {
        Rigidbody? rbGanNhat = null;
        float khoangCachGanNhat = float.MaxValue;
        
        foreach (var rb in danhSachRigidbody)
        {
            float khoangCach = Vector3.Distance(rb.transform.position, viTri);
            if (khoangCach < khoangCachGanNhat)
            {
                khoangCachGanNhat = khoangCach;
                rbGanNhat = rb;
            }
        }
        
        return rbGanNhat;
    }
      /// <summary>
    /// Hiển thị thông tin debug trên màn hình
    /// </summary>
    private void HienThiThongTinDebug()
    {
        if (hienThiDebug)
        {
            string thongTin = $"Ragdoll State: {trangThaiHienTai}\n" +
                              $"Physics Blend: {tyLeBlendVatLy:F2}\n" +
                              $"Rigidbodies: {danhSachRigidbody.Count}\n" +
                              $"Đã chết: {(daChet ? "CÓ" : "KHÔNG")}\n" +
                              $"Có thể hồi sinh: {(CoTheHoiSinh() ? "CÓ" : "KHÔNG")}";
                              
            if (daChet && thoiGianTonTaiSauKhiChet > 0)
            {
                float thoiGianConLai = thoiGianTonTaiSauKhiChet - (Time.time - thoiGianChet);
                thongTin += $"\nThời gian còn lại: {thoiGianConLai:F1}s";
            }
            
            // Có thể sử dụng UI Text hoặc Debug.Log theo nhu cầu
            Debug.Log(thongTin);
        }
    }
    
    /// <summary>
    /// Tạo ragdoll test đơn giản cho object không có humanoid setup
    /// </summary>
    private void TaoRagdollTestDonGian()
    {
        Debug.LogWarning("Không tìm thấy ragdoll setup. Tạo test setup đơn giản...");
        
        // Tạo một child object làm ragdoll body test
        GameObject testBody = new GameObject("TestRagdollBody");
        testBody.transform.SetParent(transform);
        testBody.transform.localPosition = Vector3.up * 0.5f;
        
        // Thêm rigidbody
        Rigidbody testRb = testBody.AddComponent<Rigidbody>();
        testRb.mass = 1f;
        testRb.isKinematic = true; // Bắt đầu ở animation mode
        
        // Thêm collider
        BoxCollider testCol = testBody.AddComponent<BoxCollider>();
        testCol.size = new Vector3(0.5f, 1f, 0.3f);
        testCol.isTrigger = false;
        
        // Thêm vào danh sách
        danhSachRigidbody.Add(testRb);
        danhSachCollider.Add(testCol);
        
        // Lưu vị trí gốc
        viTriGocCacBoPhan[testRb] = testRb.transform.localPosition;
        xoayGocCacBoPhan[testRb] = testRb.transform.localRotation;
        
        Debug.Log($"Đã tạo test ragdoll setup với {danhSachRigidbody.Count} Rigidbody");
    }
    
    /// <summary>
    /// Tìm các script cần vô hiệu hóa khi nhân vật chết
    /// </summary>
    private void TimCacScriptCanVoHieuHoa()
    {
        List<MonoBehaviour> scripts = new List<MonoBehaviour>();
        
        // Tìm các script thường cần vô hiệu hóa khi chết
        var aiScripts = GetComponents<MonoBehaviour>();
        foreach (var script in aiScripts)
        {
            string scriptName = script.GetType().Name.ToLower();
            // Tìm các script AI, Controller, Movement, Input
            if (scriptName.Contains("ai") || 
                scriptName.Contains("controller") || 
                scriptName.Contains("movement") || 
                scriptName.Contains("input") ||
                scriptName.Contains("player") ||
                scriptName.Contains("npc"))
            {
                scripts.Add(script);
            }
        }
        
        cacScriptCanVoHieuHoa = scripts.ToArray();
        Debug.Log($"Tìm thấy {cacScriptCanVoHieuHoa.Length} script cần vô hiệu hóa khi chết");
    }
    
    /// <summary>
    /// Xử lý khi nhân vật chết - chuyển sang ragdoll và không biến mất
    /// </summary>
    public void XuLyKhiChet()
    {
        if (daChet) return; // Đã chết rồi thì không xử lý nữa
        
        daChet = true;
        thoiGianChet = Time.time;
        trangThaiHienTai = TrangThaiRagdoll.Chet;
        
        // Chuyển sang ragdoll nếu được cấu hình
        if (tuDongRagdollKhiChet)
        {
            ThietLapTrangThaiRagdollChet();
        }
        
        // Vô hiệu hóa các script control nếu được cấu hình
        if (voHieuHoaControlKhiChet)
        {
            VoHieuHoaCacScriptControl();
        }
        
        Debug.Log($"Nhân vật {gameObject.name} đã chết. Chuyển sang trạng thái ragdoll vĩnh viễn.");
    }
    
    /// <summary>
    /// Thiết lập trạng thái ragdoll đặc biệt cho khi chết
    /// </summary>
    private void ThietLapTrangThaiRagdollChet()
    {
        if (animator != null)
            animator.enabled = false;
            
        // Bật physics cho các bộ phận ragdoll
        foreach (var rb in danhSachRigidbody)
        {
            rb.isKinematic = false;
            // Giảm drag để tạo hiệu ứng chết tự nhiên hơn
            rb.linearDamping = 5f;
            rb.angularDamping = 5f;
        }
        
        // Bật collision cho các bộ phận ragdoll
        foreach (var col in danhSachCollider)
        {
            col.enabled = true;
        }
        
        // Tắt main collider và rigidbody hoặc CharacterController
        if (colliderNhanVat != null)
            colliderNhanVat.enabled = false;
            
        if (rigidbodyNhanVat != null)
            rigidbodyNhanVat.isKinematic = true;
            
        if (characterController != null)
            characterController.enabled = false;
            
        tyLeBlendVatLy = 1f;
    }
    
    /// <summary>
    /// Vô hiệu hóa các script control khi chết
    /// </summary>
    private void VoHieuHoaCacScriptControl()
    {
        if (cacScriptCanVoHieuHoa != null)
        {
            foreach (var script in cacScriptCanVoHieuHoa)
            {
                if (script != null && script != this) // Không tắt chính RagdollController
                {
                    script.enabled = false;
                    Debug.Log($"Đã vô hiệu hóa script: {script.GetType().Name}");
                }
            }
        }
    }
    
    /// <summary>
    /// Xóa nhân vật khỏi scene (chỉ khi có thời gian tồn tại)
    /// </summary>
    private void XoaNhanVat()
    {
        Debug.Log($"Xóa nhân vật {gameObject.name} sau {thoiGianTonTaiSauKhiChet} giây");
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Kiểm tra xem nhân vật có thể hồi sinh không
    /// </summary>
    public bool CoTheHoiSinh()
    {
        return daChet && thoiGianTonTaiSauKhiChet == 0; // Chỉ hồi sinh nếu không có thời gian tồn tại
    }
    
    /// <summary>
    /// Hồi sinh nhân vật về trạng thái sống
    /// </summary>
    public void HoiSinh()
    {
        if (!CoTheHoiSinh()) return;
        
        daChet = false;
        thoiGianChet = 0f;
        
        // Khôi phục animation
        ChuyenSangTrangThaiAnimation();
        
        // Bật lại các script control
        if (cacScriptCanVoHieuHoa != null)
        {
            foreach (var script in cacScriptCanVoHieuHoa)
            {
                if (script != null)
                {
                    script.enabled = true;
                }
            }
        }
        
        // Bật lại CharacterController nếu có
        if (characterController != null)
            characterController.enabled = true;
            
        Debug.Log($"Nhân vật {gameObject.name} đã hồi sinh!");
    }
    
    // ========== GIZMOS & DEBUG ==========
    
    void OnDrawGizmosSelected()
    {
        if (danhSachRigidbody != null)
        {
            Gizmos.color = Color.yellow;
            foreach (var rb in danhSachRigidbody)
            {
                if (rb != null)
                {
                    Gizmos.DrawWireSphere(rb.transform.position, 0.1f);
                }
            }
        }
    }
}

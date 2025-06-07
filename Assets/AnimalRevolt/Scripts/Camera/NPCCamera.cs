using UnityEngine;
using UnityEngine.InputSystem;
using AnimalRevolt.UI;

namespace AnimalRevolt.Camera
{
    /// <summary>
    /// Camera để quan sát NPC từ xa với tính năng zoom và xoay
    /// Sử dụng chung parameters từ DieuChinhThongSoCamera
    /// </summary>
    public class NPCCamera : MonoBehaviour
    {
        [Header("🎯 Target Settings")]
        [SerializeField, Tooltip("NPC cần quan sát")]
        private Transform mucTieuNPC;
        
        [SerializeField, Tooltip("Tự động tìm NPC gần nhất")]
        private bool tuDongTimNPC = true;

        [Header("📏 Distance & Position")]
        [SerializeField, Tooltip("Khoảng cách từ NPC")]
        private float khoangCach = 5f;
        
        [SerializeField, Tooltip("Độ cao so với NPC")]
        private float doCao = 2f;
        
        [SerializeField, Tooltip("Offset vị trí camera")]
        private Vector3 offsetViTri = Vector3.zero;

        [Header("🎮 Controls")]
        [SerializeField, Tooltip("Tốc độ xoay camera")]
        private float tocDoXoay = 120f;
        
        [SerializeField, Tooltip("Độ nhạy chuột")]
        private float doNhayChuot = 2f;
        
        [SerializeField, Tooltip("Nhân tốc độ khi giữ Shift")]
        private float nhanTocDoNhanh = 2f;
        
        [SerializeField, Tooltip("Tốc độ zoom")]
        private float tocDoZoom = 2f;
        
        [SerializeField, Tooltip("Giới hạn zoom (min, max)")]
        private Vector2 gioiHanZoom = new Vector2(2f, 15f);

        [Header("⚙️ Advanced Settings")]
        [SerializeField, Tooltip("Làm mềm chuyển động")]
        private float doDaiLamMem = 0.1f;
        
        [SerializeField, Tooltip("Tự động focus vào NPC")]
        private bool tuDongFocus = true;
        
        [SerializeField, Tooltip("Sử dụng shared parameters")]
        private bool suDungSharedParameters = true;

        // Biến quản lý
        private UnityEngine.Camera cameraNPC;
        private float gocXoayX = 0f;
        private float gocXoayY = 0f;
        private float khoangCachHienTai;
        private Vector3 vanTocLamMem;

        // Shared parameters reference
        private CameraSettingsUI cameraSettingsUI;

        private void Start()
        {
            KhoiTaoCamera();
        }

        private void Update()
        {
            if (cameraNPC != null && cameraNPC.enabled)
            {
                CapNhatSharedParameters();
                XuLyDieuKhienCamera();
                CapNhatViTriCamera();
            }
        }

        /// <summary>
        /// Khởi tạo camera và các thông số
        /// </summary>
        private void KhoiTaoCamera()
        {
            // Lấy camera component
            cameraNPC = GetComponent<UnityEngine.Camera>();
            if (cameraNPC == null)
            {
                cameraNPC = gameObject.AddComponent<UnityEngine.Camera>();
            }

            // Tắt camera ban đầu
            cameraNPC.enabled = false;

            // Khởi tạo khoảng cách
            khoangCachHienTai = khoangCach;

            // Tự động tìm NPC nếu cần
            if (tuDongTimNPC && mucTieuNPC == null)
            {
                TimNPCGanNhat();
            }

            // Tìm CameraSettingsUI để sử dụng shared parameters
            if (suDungSharedParameters)
            {
                cameraSettingsUI = FindFirstObjectByType<CameraSettingsUI>();
            }

            // Khởi tạo góc xoay dựa trên vị trí hiện tại
            if (mucTieuNPC != null)
            {
                Vector3 huong = transform.position - mucTieuNPC.position;
                
                // Kiểm tra hướng hợp lệ trước khi tính toán
                if (IsValidVector3(huong) && huong.magnitude > 0.001f)
                {
                    gocXoayY = Mathf.Atan2(huong.x, huong.z) * Mathf.Rad2Deg;
                    
                    // Tránh Asin với giá trị ngoài phạm vi [-1, 1]
                    float sinValue = huong.y / huong.magnitude;
                    sinValue = Mathf.Clamp(sinValue, -1f, 1f);
                    gocXoayX = Mathf.Asin(sinValue) * Mathf.Rad2Deg;
                    
                    // Validation kết quả
                    if (!IsValidFloat(gocXoayX) || !IsValidFloat(gocXoayY))
                    {
                        Debug.LogWarning("🚨 NPCCamera: Góc xoay khởi tạo không hợp lệ, sử dụng giá trị mặc định");
                        gocXoayX = 0f;
                        gocXoayY = 0f;
                    }
                }
                else
                {
                    Debug.LogWarning("🚨 NPCCamera: Hướng khởi tạo không hợp lệ, sử dụng góc mặc định");
                    gocXoayX = 0f;
                    gocXoayY = 0f;
                }
            }

            Debug.Log($"🎯 NPCCamera khởi tạo: {name} | Target: {(mucTieuNPC ? mucTieuNPC.name : "None")}");
        }

        /// <summary>
        /// Cập nhật shared parameters từ CameraSettingsUI
        /// </summary>
        private void CapNhatSharedParameters()
        {
            if (!suDungSharedParameters || cameraSettingsUI == null) return;

            // Lấy parameters từ shared UI
            tocDoXoay = cameraSettingsUI.LayTocDoXoayNPC();
            doNhayChuot = cameraSettingsUI.LayDoNhayChuotNPC();
            nhanTocDoNhanh = cameraSettingsUI.LayNhanTocDoXoayNhanhNPC();
            khoangCach = cameraSettingsUI.LayKhoangCachNPC();
        }

        /// <summary>
        /// Xử lý điều khiển camera
        /// </summary>
        private void XuLyDieuKhienCamera()
        {
            if (Mouse.current == null || Keyboard.current == null) return;

            // Xoay camera bằng chuột phải
            if (Mouse.current.rightButton.isPressed)
            {
                Vector2 deltaXoay = Mouse.current.delta.ReadValue();
                
                // Validation delta input
                if (!IsValidFloat(deltaXoay.x) || !IsValidFloat(deltaXoay.y))
                {
                    Debug.LogWarning("🚨 NPCCamera: Delta xoay chuột không hợp lệ");
                    return;
                }
                
                float tocDoXoayHienTai = tocDoXoay;
                if (Keyboard.current.leftShiftKey.isPressed)
                {
                    tocDoXoayHienTai *= nhanTocDoNhanh;
                }

                // Tính toán góc xoay mới với validation
                float deltaY = deltaXoay.x * doNhayChuot * tocDoXoayHienTai * Time.deltaTime * 0.02f;
                float deltaX = deltaXoay.y * doNhayChuot * tocDoXoayHienTai * Time.deltaTime * 0.02f;
                
                if (IsValidFloat(deltaY) && IsValidFloat(deltaX))
                {
                    gocXoayY += deltaY;
                    gocXoayX -= deltaX;
                    
                    // Giới hạn góc xoay và validation
                    gocXoayX = Mathf.Clamp(gocXoayX, -89f, 89f);
                    gocXoayY = gocXoayY % 360f; // Normalize góc Y
                    
                    // Double check validation
                    if (!IsValidFloat(gocXoayX) || !IsValidFloat(gocXoayY))
                    {
                        Debug.LogWarning("🚨 NPCCamera: Góc xoay trở thành không hợp lệ, reset về 0");
                        gocXoayX = 0f;
                        gocXoayY = 0f;
                    }
                }
            }

            // Zoom bằng scroll wheel
            float scrollInput = Mouse.current.scroll.ReadValue().y;
            if (scrollInput != 0f && IsValidFloat(scrollInput))
            {
                float zoomDelta = scrollInput * tocDoZoom * Time.deltaTime;
                if (IsValidFloat(zoomDelta))
                {
                    khoangCachHienTai -= zoomDelta;
                    khoangCachHienTai = Mathf.Clamp(khoangCachHienTai, gioiHanZoom.x, gioiHanZoom.y);
                    
                    // Validation khoảng cách
                    if (!IsValidFloat(khoangCachHienTai) || khoangCachHienTai <= 0f)
                    {
                        khoangCachHienTai = Mathf.Max(gioiHanZoom.x, 1f);
                        Debug.LogWarning("🚨 NPCCamera: Khoảng cách zoom không hợp lệ, reset về giá trị tối thiểu");
                    }
                }
            }
        }

        /// <summary>
        /// Cập nhật vị trí camera
        /// </summary>
        private void CapNhatViTriCamera()
        {
            if (mucTieuNPC == null) return;

            // Validation: Kiểm tra các giá trị không hợp lệ
            if (!IsValidFloat(gocXoayX) || !IsValidFloat(gocXoayY) || !IsValidFloat(khoangCachHienTai))
            {
                Debug.LogWarning($"🚨 NPCCamera: Phát hiện giá trị không hợp lệ - gocXoayX: {gocXoayX}, gocXoayY: {gocXoayY}, khoangCach: {khoangCachHienTai}");
                ResetCameraValues();
                return;
            }

            // Validation: Kiểm tra vị trí NPC
            if (!IsValidVector3(mucTieuNPC.position))
            {
                Debug.LogWarning($"🚨 NPCCamera: Vị trí NPC không hợp lệ: {mucTieuNPC.position}");
                return;
            }

            // Đảm bảo khoảng cách tối thiểu
            if (khoangCachHienTai <= 0f)
            {
                khoangCachHienTai = 1f;
                Debug.LogWarning("🚨 NPCCamera: Khoảng cách <= 0, đặt lại về 1f");
            }

            // Tính vị trí camera dựa trên góc xoay và khoảng cách
            Vector3 huongCamera = new Vector3(
                Mathf.Sin(gocXoayY * Mathf.Deg2Rad) * Mathf.Cos(gocXoayX * Mathf.Deg2Rad),
                Mathf.Sin(gocXoayX * Mathf.Deg2Rad),
                Mathf.Cos(gocXoayY * Mathf.Deg2Rad) * Mathf.Cos(gocXoayX * Mathf.Deg2Rad)
            );

            // Validation: Kiểm tra hướng camera
            if (!IsValidVector3(huongCamera))
            {
                Debug.LogWarning($"🚨 NPCCamera: Hướng camera không hợp lệ: {huongCamera}");
                ResetCameraValues();
                return;
            }

            // Vị trí mục tiêu với offset
            Vector3 viTriMucTieu = mucTieuNPC.position + Vector3.up * doCao + offsetViTri;
            
            // Validation: Kiểm tra vị trí mục tiêu
            if (!IsValidVector3(viTriMucTieu))
            {
                Debug.LogWarning($"🚨 NPCCamera: Vị trí mục tiêu không hợp lệ: {viTriMucTieu}");
                return;
            }
            
            // Vị trí camera mới
            Vector3 viTriMoi = viTriMucTieu + huongCamera * khoangCachHienTai;

            // Validation: Kiểm tra vị trí mới
            if (!IsValidVector3(viTriMoi))
            {
                Debug.LogWarning($"🚨 NPCCamera: Vị trí camera mới không hợp lệ: {viTriMoi}");
                return;
            }

            // Làm mềm chuyển động với validation bổ sung
            if (IsValidVector3(transform.position))
            {
                Vector3 viTriMuon = Vector3.SmoothDamp(transform.position, viTriMoi, ref vanTocLamMem, doDaiLamMem);
                
                // Kiểm tra kết quả SmoothDamp
                if (IsValidVector3(viTriMuon))
                {
                    transform.position = viTriMuon;
                }
                else
                {
                    Debug.LogWarning("🚨 NPCCamera: SmoothDamp trả về giá trị không hợp lệ, sử dụng vị trí trực tiếp");
                    transform.position = viTriMoi;
                }
            }
            else
            {
                // Nếu vị trí hiện tại không hợp lệ, đặt trực tiếp
                transform.position = viTriMoi;
            }

            // Xoay camera nhìn về mục tiêu
            if (tuDongFocus && IsValidVector3(viTriMucTieu) && IsValidVector3(transform.position))
            {
                Vector3 huongNhin = (viTriMucTieu - transform.position).normalized;
                
                if (IsValidVector3(huongNhin) && huongNhin.magnitude > 0.001f)
                {
                    Quaternion xoayMoi = Quaternion.LookRotation(huongNhin);
                    transform.rotation = Quaternion.Lerp(transform.rotation, xoayMoi, Time.deltaTime * 5f);
                }
            }
        }

        /// <summary>
        /// Kiểm tra một số float có hợp lệ không (không phải NaN hoặc Infinity)
        /// </summary>
        private bool IsValidFloat(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }

        /// <summary>
        /// Kiểm tra một Vector3 có hợp lệ không
        /// </summary>
        private bool IsValidVector3(Vector3 vector)
        {
            return IsValidFloat(vector.x) && IsValidFloat(vector.y) && IsValidFloat(vector.z);
        }

        /// <summary>
        /// Reset các giá trị camera về trạng thái an toàn
        /// </summary>
        private void ResetCameraValues()
        {
            gocXoayX = 0f;
            gocXoayY = 0f;
            khoangCachHienTai = Mathf.Max(khoangCach, 1f);
            vanTocLamMem = Vector3.zero;
            
            Debug.Log("🔄 NPCCamera: Đã reset các giá trị về trạng thái an toàn");
        }

        /// <summary>
        /// Tìm NPC gần nhất
        /// </summary>
        private void TimNPCGanNhat()
        {
            GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
            if (npcs.Length == 0)
            {
                Debug.LogWarning("🎯 Không tìm thấy NPC nào trong scene");
                return;
            }

            float khoangCachGanNhat = float.MaxValue;
            GameObject npcGanNhat = null;

            foreach (GameObject npc in npcs)
            {
                float khoangCachDenNPC = Vector3.Distance(transform.position, npc.transform.position);
                if (khoangCachDenNPC < khoangCachGanNhat)
                {
                    khoangCachGanNhat = khoangCachDenNPC;
                    npcGanNhat = npc;
                }
            }

            if (npcGanNhat != null)
            {
                mucTieuNPC = npcGanNhat.transform;
                Debug.Log($"🎯 NPCCamera tự động chọn target: {npcGanNhat.name}");
            }
        }

        #region Public API

        /// <summary>
        /// Bật camera
        /// </summary>
        public void BatCamera()
        {
            if (cameraNPC != null)
            {
                cameraNPC.enabled = true;
                Debug.Log($"📹 NPCCamera bật: {name}");
            }
        }

        /// <summary>
        /// Tắt camera
        /// </summary>
        public void TatCamera()
        {
            if (cameraNPC != null)
            {
                cameraNPC.enabled = false;
            }
        }

        /// <summary>
        /// Lấy camera component
        /// </summary>
        public UnityEngine.Camera GetCamera()
        {
            return cameraNPC;
        }

        /// <summary>
        /// Đặt mục tiêu NPC
        /// </summary>
        public void DatMucTieuNPC(Transform target)
        {
            mucTieuNPC = target;
            if (target != null)
            {
                Debug.Log($"🎯 NPCCamera đặt target mới: {target.name}");
            }
        }

        /// <summary>
        /// Lấy mục tiêu NPC hiện tại
        /// </summary>
        public Transform LayMucTieuNPC()
        {
            return mucTieuNPC;
        }

        /// <summary>
        /// Reset camera về vị trí mặc định
        /// </summary>
        public void ResetCamera()
        {
            khoangCachHienTai = khoangCach;
            gocXoayX = 0f;
            gocXoayY = 0f;
            
            if (mucTieuNPC != null)
            {
                Vector3 viTriMacDinh = mucTieuNPC.position + Vector3.back * khoangCach + Vector3.up * doCao;
                transform.position = viTriMacDinh;
                transform.LookAt(mucTieuNPC.position + Vector3.up * doCao);
            }
            
            Debug.Log($"🏠 NPCCamera reset: {name}");
        }

        /// <summary>
        /// Đặt khoảng cách
        /// </summary>
        public void DatKhoangCach(float khoangCachMoi)
        {
            khoangCach = Mathf.Max(0.5f, khoangCachMoi);
            khoangCachHienTai = khoangCach;
        }

        /// <summary>
        /// Lấy khoảng cách hiện tại
        /// </summary>
        public float LayKhoangCach()
        {
            return khoangCachHienTai;
        }

        /// <summary>
        /// Đặt độ nhạy chuột
        /// </summary>
        public void DatDoNhayChuot(float doNhayMoi)
        {
            doNhayChuot = Mathf.Max(0.1f, doNhayMoi);
        }

        /// <summary>
        /// Lấy độ nhạy chuột
        /// </summary>
        public float LayDoNhayChuot()
        {
            return doNhayChuot;
        }

        /// <summary>
        /// Kiểm tra camera có đang hoạt động không
        /// </summary>
        public bool DangHoatDong()
        {
            return cameraNPC != null && cameraNPC.enabled;
        }

        /// <summary>
        /// Bật/tắt sử dụng shared parameters
        /// </summary>
        public void BatTatSharedParameters(bool suDung)
        {
            suDungSharedParameters = suDung;
        }

        #endregion

        private void OnDrawGizmosSelected()
        {
            // Vẽ gizmo để hiển thị target và khoảng cách
            if (mucTieuNPC != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(mucTieuNPC.position + Vector3.up * doCao, 0.5f);
                
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, mucTieuNPC.position + Vector3.up * doCao);
                
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(mucTieuNPC.position, khoangCach);
            }
        }
    }
}
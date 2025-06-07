using UnityEngine;
using UnityEngine.InputSystem;

namespace AnimalRevolt.Camera
{
    /// <summary>
    /// Quản lý chuyển đổi giữa các camera trong game
    /// Bao gồm Main Camera và NPC Cameras
    /// </summary>
    public class CameraManager : MonoBehaviour
    {
        [Header("🎥 Camera Management")]
        [SerializeField, Tooltip("Camera chính")]
        private UnityEngine.Camera cameraMain;
        
        [SerializeField, Tooltip("Danh sách NPC Cameras")]
        private NPCCamera[] npcCameras;
        
        [SerializeField, Tooltip("Chỉ số camera hiện tại")]
        private int chiSoCameraHienTai = 0;

        [Header("⚙️ Settings")]
        [SerializeField, Tooltip("Cho phép chuyển camera bằng phím")]
        private bool choPhepChuyenCamera = true;
        
        [SerializeField, Tooltip("Hiển thị debug info")]
        private bool hienThiDebugInfo = true;

        // Biến quản lý
        private int tongSoCamera;
        private bool[] trangThaiAudioListener;

        private void Start()
        {
            KhoiTaoHeThhongCamera();
        }

        private void Update()
        {
            if (choPhepChuyenCamera)
            {
                XuLyInputChuyenCamera();
            }
        }

        /// <summary>
        /// Khởi tạo hệ thống camera
        /// </summary>
        private void KhoiTaoHeThhongCamera()
        {
            // Tự động tìm camera chính nếu chưa gán
            if (cameraMain == null)
            {
                cameraMain = UnityEngine.Camera.main;
            }

            // Tự động tìm tất cả NPC cameras
            if (npcCameras == null || npcCameras.Length == 0)
            {
                npcCameras = FindObjectsByType<NPCCamera>(FindObjectsSortMode.None);
            }

            // Tính tổng số camera
            tongSoCamera = 1 + (npcCameras?.Length ?? 0);

            // Lưu trạng thái AudioListener ban đầu
            LuuTrangThaiAudioListener();

            // Kích hoạt camera chính
            BatCameraChinh();

            if (hienThiDebugInfo)
            {
                Debug.Log($"🎥 CameraManager khởi tạo: {tongSoCamera} cameras");
                Debug.Log("📹 Phím 0: Camera chính, 1-9: NPC Cameras");
            }
        }

        /// <summary>
        /// Xử lý input chuyển camera
        /// </summary>
        private void XuLyInputChuyenCamera()
        {
            if (Keyboard.current == null) return;

            // Phím số để chuyển camera trực tiếp
            if (Keyboard.current.digit0Key.wasPressedThisFrame)
            {
                BatCameraChinh();
            }
            else if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                ChuyenSangNPCCamera(0);
            }
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                ChuyenSangNPCCamera(1);
            }
            else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                ChuyenSangNPCCamera(2);
            }
            else if (Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                ChuyenSangNPCCamera(3);
            }
            else if (Keyboard.current.digit5Key.wasPressedThisFrame)
            {
                ChuyenSangNPCCamera(4);
            }

            // Tab để chuyển camera kế tiếp
            if (Keyboard.current.tabKey.wasPressedThisFrame)
            {
                ChuyenCameraKeTiep();
            }
        }

        /// <summary>
        /// Bật camera chính
        /// </summary>
        public void BatCameraChinh()
        {
            if (cameraMain == null) return;

            chiSoCameraHienTai = 0;

            // Tắt tất cả NPC cameras
            TatTatCaNPCCameras();

            // Bật camera chính
            cameraMain.enabled = true;
            
            // Quản lý AudioListener
            QuanLyAudioListener(cameraMain.gameObject, true);

            if (hienThiDebugInfo)
            {
                Debug.Log("🎥 Chuyển sang Camera chính");
            }
        }

        /// <summary>
        /// Chuyển sang NPC camera theo index
        /// </summary>
        public void ChuyenSangNPCCamera(int index)
        {
            if (npcCameras == null || index < 0 || index >= npcCameras.Length)
            {
                Debug.LogWarning($"❌ NPC Camera index {index} không hợp lệ");
                return;
            }

            NPCCamera npcCamera = npcCameras[index];
            if (npcCamera == null)
            {
                Debug.LogWarning($"❌ NPC Camera {index} là null");
                return;
            }

            chiSoCameraHienTai = index + 1;

            // Tắt camera chính
            if (cameraMain != null)
            {
                cameraMain.enabled = false;
                QuanLyAudioListener(cameraMain.gameObject, false);
            }

            // Tắt tất cả NPC cameras khác
            TatTatCaNPCCameras();

            // Bật NPC camera được chọn
            npcCamera.BatCamera();
            UnityEngine.Camera cam = npcCamera.GetCamera();
            if (cam != null)
            {
                QuanLyAudioListener(cam.gameObject, true);
            }

            if (hienThiDebugInfo)
            {
                Debug.Log($"🎥 Chuyển sang NPC Camera {index}: {npcCamera.name}");
            }
        }

        /// <summary>
        /// Chuyển sang camera kế tiếp
        /// </summary>
        public void ChuyenCameraKeTiep()
        {
            int cameraTiepTheo = (chiSoCameraHienTai + 1) % tongSoCamera;

            if (cameraTiepTheo == 0)
            {
                BatCameraChinh();
            }
            else
            {
                ChuyenSangNPCCamera(cameraTiepTheo - 1);
            }
        }

        /// <summary>
        /// Tắt tất cả NPC cameras
        /// </summary>
        private void TatTatCaNPCCameras()
        {
            if (npcCameras == null) return;

            foreach (NPCCamera npcCamera in npcCameras)
            {
                if (npcCamera != null)
                {
                    npcCamera.TatCamera();
                    UnityEngine.Camera cam = npcCamera.GetCamera();
                    if (cam != null)
                    {
                        QuanLyAudioListener(cam.gameObject, false);
                    }
                }
            }
        }

        /// <summary>
        /// Quản lý AudioListener để tránh conflict
        /// </summary>
        private void QuanLyAudioListener(GameObject cameraObject, bool kichHoat)
        {
            AudioListener audioListener = cameraObject.GetComponent<AudioListener>();
            if (audioListener != null)
            {
                audioListener.enabled = kichHoat;
            }
        }

        /// <summary>
        /// Lưu trạng thái AudioListener ban đầu
        /// </summary>
        private void LuuTrangThaiAudioListener()
        {
            AudioListener[] allListeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
            trangThaiAudioListener = new bool[allListeners.Length];
            
            for (int i = 0; i < allListeners.Length; i++)
            {
                trangThaiAudioListener[i] = allListeners[i].enabled;
            }
        }

        /// <summary>
        /// Khôi phục trạng thái AudioListener ban đầu
        /// </summary>
        private void KhoiPhucAudioListener()
        {
            AudioListener[] allListeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
            
            for (int i = 0; i < allListeners.Length && i < trangThaiAudioListener.Length; i++)
            {
                allListeners[i].enabled = trangThaiAudioListener[i];
            }
        }

        #region Public API

        /// <summary>
        /// Lấy camera hiện tại đang active
        /// </summary>
        public UnityEngine.Camera LayCameraHienTai()
        {
            if (chiSoCameraHienTai == 0)
            {
                return cameraMain;
            }
            else if (npcCameras != null && chiSoCameraHienTai - 1 < npcCameras.Length)
            {
                NPCCamera npcCamera = npcCameras[chiSoCameraHienTai - 1];
                return npcCamera?.GetCamera();
            }
            return null;
        }

        /// <summary>
        /// Lấy index camera hiện tại
        /// </summary>
        public int LayChiSoCameraHienTai()
        {
            return chiSoCameraHienTai;
        }

        /// <summary>
        /// Đặt camera chính
        /// </summary>
        public void DatCameraChinh(UnityEngine.Camera camera)
        {
            cameraMain = camera;
        }

        /// <summary>
        /// Thêm NPC camera
        /// </summary>
        public void ThemNPCCamera(NPCCamera npcCamera)
        {
            var newArray = new NPCCamera[npcCameras.Length + 1];
            npcCameras.CopyTo(newArray, 0);
            newArray[npcCameras.Length] = npcCamera;
            npcCameras = newArray;
            tongSoCamera = 1 + npcCameras.Length;
        }

        /// <summary>
        /// Bật/tắt chuyển camera
        /// </summary>
        public void BatTatChuyenCamera(bool choPhep)
        {
            choPhepChuyenCamera = choPhep;
        }

        /// <summary>
        /// Bật/tắt debug info
        /// </summary>
        public void BatTatDebugInfo(bool hienThi)
        {
            hienThiDebugInfo = hienThi;
        }

        #endregion

        private void OnDestroy()
        {
            // Khôi phục AudioListener khi destroy
            if (trangThaiAudioListener != null)
            {
                KhoiPhucAudioListener();
            }
        }
    }
}
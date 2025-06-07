using UnityEngine;
using UnityEngine.InputSystem;

namespace AnimalRevolt.Camera
{
    /// <summary>
    /// Controller cho camera chính có thể di chuyển tự do trong không gian 3D
    /// Hỗ trợ di chuyển bằng WASD + QE và xoay bằng chuột
    /// Bao gồm 4 camera modes: FreeCam, Follow, Overview, Orbital
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        // Camera modes enum
        public enum CameraMode { FreeCam, Follow, Overview, Orbital }
        
        [Header("🎥 Camera Modes")]
        [SerializeField, Tooltip("Chế độ camera hiện tại")]
        private CameraMode cheDoCamerHienTai = CameraMode.FreeCam;
        
        [SerializeField, Tooltip("Cho phép chuyển đổi mode bằng phím")]
        private bool choPhepChuyenMode = true;

        [Header("Cấu hình di chuyển")]
        [SerializeField, Tooltip("Tốc độ di chuyển camera")]
        private float tocDoChuyenDong = 10f;

        [SerializeField, Tooltip("Tốc độ di chuyển nhanh (khi giữ Shift)")]
        private float tocDoChuyenDongNhanh = 20f;
        
        [SerializeField, Tooltip("Tốc độ lên xuống (Q/E)")]
        private float tocDoLenXuong = 5f;

        [Header("Cấu hình xoay")]
        [SerializeField, Tooltip("Tốc độ xoay camera chính (độ/giây)")]
        private float tocDoXoayCamera = 150f;

        [SerializeField, Tooltip("Nhân tốc độ xoay nhanh khi giữ Shift")]
        private float nhanTocDoXoayNhanh = 2.5f;

        [SerializeField, Tooltip("Độ nhạy xoay chuột")]
        private float doNhayChuot = 3f;

        [SerializeField, Tooltip("Giới hạn góc xoay lên xuống")]
        private float gioiHanGocXoay = 90f;

        [SerializeField, Tooltip("Làm mềm chuyển động")]
        private float doDaiLamMem = 0.1f;

        // Biến quản lý input
        private Vector2 inputDiChuyen;
        private Vector2 inputXoayChuot;
        private bool inputLenCao;
        private bool inputXuongThap;
        private bool inputChuyenDongNhanh;

        // Biến quản lý xoay
        private float gocXoayX = 0f;
        private float gocXoayY = 0f;

        // Biến làm mềm chuyển động
        private Vector3 vanTocHienTai;
        private Vector3 vanTocLamMem;

        [Header("🎯 Follow & Orbital Mode")]
        [SerializeField, Tooltip("Mục tiêu follow (tự động tìm nếu null)")]
        private Transform mucTieuFollow;
        
        [SerializeField, Tooltip("Khoảng cách follow")]
        private float khoangCachFollow = 5f;
        
        [SerializeField, Tooltip("Độ cao follow")]
        private float doCaoFollow = 2f;
        
        [SerializeField, Tooltip("Tốc độ lerp khi follow")]
        private float tocDoLerp = 2f;
        
        [SerializeField, Tooltip("Tự động focus khi orbital")]
        private bool tuDongFocusOrbital = true;

        [Header("🌍 Overview Mode")]
        [SerializeField, Tooltip("Độ cao overview")]
        private float doCaoOverview = 15f;
        
        [SerializeField, Tooltip("Khoảng cách overview")]
        private float khoangCachOverview = 10f;

        // Biến quản lý camera modes
        private Vector3 viTriBanDau;
        private Quaternion gocXoayBanDau;
        private Vector3 viTriFocusOrbital;
        private bool dangOrbital = false;
        private float khoangCachOrbitalHienTai = 5f;

        private void Start()
        {
            viTriBanDau = transform.position;
            gocXoayBanDau = transform.rotation;
            
            Vector3 gocXoayHienTai = transform.eulerAngles;
            gocXoayY = gocXoayHienTai.y;
            gocXoayX = gocXoayHienTai.x;

            if (gocXoayX > 180f)
                gocXoayX -= 360f;
                
            khoangCachOrbitalHienTai = khoangCachFollow;
            
            Debug.Log($"🎥 CameraController khởi tạo - Mode: {cheDoCamerHienTai}");
        }

        private void Update()
        {
            if (!enabled) return;

            XuLyInputCameraMode();

            switch (cheDoCamerHienTai)
            {
                case CameraMode.FreeCam:
                    XuLyXoayCamera();
                    XuLyDiChuyenCamera();
                    break;
                case CameraMode.Follow:
                    XuLyFollowMode();
                    break;
                case CameraMode.Overview:
                    XuLyOverviewMode();
                    break;
                case CameraMode.Orbital:
                    XuLyOrbitalMode();
                    break;
            }
        }

        private void XuLyXoayCamera()
        {
            if (Mouse.current == null) return;
            if (Mouse.current.rightButton.isPressed)
            {
                Vector2 deltaXoayChuot = Mouse.current.delta.ReadValue();

                float tocDoXoayHienTai = tocDoXoayCamera;
                if (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed)
                {
                    tocDoXoayHienTai *= nhanTocDoXoayNhanh;
                }

                gocXoayX -= deltaXoayChuot.y * doNhayChuot * tocDoXoayHienTai * Time.deltaTime * 0.015f;
                gocXoayY += deltaXoayChuot.x * doNhayChuot * tocDoXoayHienTai * Time.deltaTime * 0.015f;

                gocXoayX = Mathf.Clamp(gocXoayX, -gioiHanGocXoay, gioiHanGocXoay);
                transform.rotation = Quaternion.Euler(gocXoayX, gocXoayY, 0f);
            }
        }

        private void XuLyDiChuyenCamera()
        {
            if (Keyboard.current == null) return;

            Vector2 inputWASD = Vector2.zero;
            if (Keyboard.current.wKey.isPressed) inputWASD.y += 1f;
            if (Keyboard.current.sKey.isPressed) inputWASD.y -= 1f;
            if (Keyboard.current.aKey.isPressed) inputWASD.x -= 1f;
            if (Keyboard.current.dKey.isPressed) inputWASD.x += 1f;

            float inputLenXuong = 0f;
            if (Keyboard.current.qKey.isPressed) inputLenXuong += 1f;
            if (Keyboard.current.eKey.isPressed) inputLenXuong -= 1f;

            bool chuyenDongNhanh = Keyboard.current.leftShiftKey.isPressed;

            Vector3 huongDiChuyen = Vector3.zero;
            Vector3 huongTruoc = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
            Vector3 huongPhai = transform.right;

            huongDiChuyen += huongTruoc * inputWASD.y;
            huongDiChuyen += huongPhai * inputWASD.x;
            huongDiChuyen += Vector3.up * inputLenXuong;

            float tocDoHienTai = chuyenDongNhanh ? tocDoChuyenDongNhanh : tocDoChuyenDong;
            if (inputLenXuong != 0f)
            {
                Vector3 vanTocLenXuong = Vector3.up * inputLenXuong * tocDoLenXuong;
                Vector3 vanTocNgang = new Vector3(huongDiChuyen.x, 0, huongDiChuyen.z) * tocDoHienTai;
                huongDiChuyen = vanTocNgang + vanTocLenXuong;
            }
            else
            {
                huongDiChuyen *= tocDoHienTai;
            }

            vanTocHienTai = Vector3.SmoothDamp(vanTocHienTai, huongDiChuyen, ref vanTocLamMem, doDaiLamMem);
            transform.position += vanTocHienTai * Time.deltaTime;
        }

        private void XuLyInputCameraMode()
        {
            if (!choPhepChuyenMode || Keyboard.current == null) return;

            if (Keyboard.current.cKey.wasPressedThisFrame)
                ChuyenCameraMode();

            if (Keyboard.current.homeKey.wasPressedThisFrame)
                ResetCamera();

            if (Keyboard.current.fKey.wasPressedThisFrame && cheDoCamerHienTai == CameraMode.Follow)
                TimMucTieuFollowTuDong();
        }

        private void XuLyFollowMode()
        {
            if (mucTieuFollow == null)
            {
                cheDoCamerHienTai = CameraMode.FreeCam;
                return;
            }

            Vector3 viTriMongMuon = mucTieuFollow.position - transform.forward * khoangCachFollow + Vector3.up * doCaoFollow;
            transform.position = Vector3.Lerp(transform.position, viTriMongMuon, tocDoLerp * Time.deltaTime);
        }

        private void XuLyOverviewMode()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
            
            var allTargets = new System.Collections.Generic.List<GameObject>();
            allTargets.AddRange(players);
            allTargets.AddRange(npcs);

            if (allTargets.Count > 0)
            {
                Vector3 center = Vector3.zero;
                foreach (var target in allTargets)
                {
                    if (target != null)
                        center += target.transform.position;
                }
                center /= allTargets.Count;
                
                Vector3 viTriOverview = center + Vector3.up * doCaoOverview + Vector3.back * khoangCachOverview;
                transform.position = Vector3.Lerp(transform.position, viTriOverview, tocDoLerp * Time.deltaTime);
            }
        }

        private void XuLyOrbitalMode()
        {
            if (mucTieuFollow == null)
            {
                cheDoCamerHienTai = CameraMode.FreeCam;
                return;
            }

            if (Mouse.current != null && Mouse.current.rightButton.isPressed)
            {
                Vector2 deltaXoayChuot = Mouse.current.delta.ReadValue();
                float tocDoXoayHienTai = tocDoXoayCamera;
                
                if (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed)
                    tocDoXoayHienTai *= nhanTocDoXoayNhanh;

                gocXoayY += deltaXoayChuot.x * doNhayChuot * tocDoXoayHienTai * Time.deltaTime * 0.02f;
                gocXoayX -= deltaXoayChuot.y * doNhayChuot * tocDoXoayHienTai * Time.deltaTime * 0.02f;
                gocXoayX = Mathf.Clamp(gocXoayX, -89f, 89f);
            }

            Vector3 huongCamera = new Vector3(
                Mathf.Sin(gocXoayY * Mathf.Deg2Rad) * Mathf.Cos(gocXoayX * Mathf.Deg2Rad),
                Mathf.Sin(gocXoayX * Mathf.Deg2Rad),
                Mathf.Cos(gocXoayY * Mathf.Deg2Rad) * Mathf.Cos(gocXoayX * Mathf.Deg2Rad)
            );

            Vector3 diemFocus = mucTieuFollow.position + Vector3.up * doCaoFollow;
            Vector3 viTriMoi = diemFocus + huongCamera * khoangCachOrbitalHienTai;
            transform.position = Vector3.Lerp(transform.position, viTriMoi, Time.deltaTime * tocDoLerp);
        }

        private void ChuyenCameraMode()
        {
            cheDoCamerHienTai = (CameraMode)(((int)cheDoCamerHienTai + 1) % System.Enum.GetValues(typeof(CameraMode)).Length);
            Debug.Log($"🎥 Camera Mode: {cheDoCamerHienTai}");
            
            if (cheDoCamerHienTai == CameraMode.Follow || cheDoCamerHienTai == CameraMode.Orbital)
                TimMucTieuFollowTuDong();
        }

        public void ResetCamera()
        {
            transform.position = viTriBanDau;
            transform.rotation = gocXoayBanDau;
            cheDoCamerHienTai = CameraMode.FreeCam;
            Debug.Log("🏠 Camera reset về vị trí ban đầu");
        }

        private void TimMucTieuFollowTuDong()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                mucTieuFollow = player.transform;
                return;
            }

            GameObject npc = GameObject.FindGameObjectWithTag("NPC");
            if (npc != null)
            {
                mucTieuFollow = npc.transform;
            }
        }

        #region Public API
        public CameraMode LayCameraMode() => cheDoCamerHienTai;
        public void DatCameraMode(CameraMode mode) => cheDoCamerHienTai = mode;
        public float LayTocDoXoay() => tocDoXoayCamera;
        public void DatTocDoXoay(float tocDoMoi) => tocDoXoayCamera = Mathf.Max(0f, tocDoMoi);
        public float LayDoNhayChuot() => doNhayChuot;
        public void DatDoNhayChuot(float doNhayMoi) => doNhayChuot = Mathf.Max(0f, doNhayMoi);
        public float LayTocDoChuyenDong() => tocDoChuyenDong;
        public void DatTocDoChuyenDong(float tocDoMoi) => tocDoChuyenDong = Mathf.Max(0f, tocDoMoi);
        public float LayNhanTocDoXoayNhanh() => nhanTocDoXoayNhanh;
        #endregion
    }
}
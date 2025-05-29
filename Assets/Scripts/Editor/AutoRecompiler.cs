using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Script tự động theo dõi và compile lại code khi có thay đổi từ VSCode
/// Đặt script này trong thư mục Editor để chạy trong Unity Editor
/// </summary>
[InitializeOnLoad]
public class AutoRecompiler
{
    private static FileSystemWatcher? watcher;
    private static float lastRefreshTime;
    private const float REFRESH_DELAY = 0.5f; // Delay để tránh refresh quá nhiều
    
    static AutoRecompiler()
    {
        // Khởi tạo khi Unity Editor start
        InitializeFileWatcher();
        EditorApplication.update += CheckForDelayedRefresh;
        
        Debug.Log("[AUTO RECOMPILER] Đã khởi tạo tự động compile cho VSCode");
    }
    
    static void InitializeFileWatcher()
    {
        try
        {
            // Đường dẫn đến thư mục Scripts
            string scriptsPath = Path.Combine(Application.dataPath, "Scripts");
            
            if (!Directory.Exists(scriptsPath))
            {
                Debug.LogWarning("[AUTO RECOMPILER] Không tìm thấy thư mục Assets/Scripts");
                return;
            }
            
            // Tạo FileSystemWatcher để theo dõi thay đổi file .cs
            watcher = new FileSystemWatcher(scriptsPath, "*.cs");
            watcher.Changed += OnScriptChanged;
            watcher.Created += OnScriptChanged;
            watcher.Deleted += OnScriptChanged;
            watcher.Renamed += OnScriptRenamed;
            
            // Bật theo dõi subdirectories
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
            
            Debug.Log($"[AUTO RECOMPILER] Đang theo dõi thay đổi trong: {scriptsPath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[AUTO RECOMPILER] Lỗi khởi tạo file watcher: {ex.Message}");
        }
    }
    
    static void OnScriptChanged(object sender, FileSystemEventArgs e)
    {
        // Bỏ qua file .meta và file tạm
        if (e.Name?.EndsWith(".meta") == true || 
            e.Name?.EndsWith(".tmp") == true ||
            e.Name?.Contains("~") == true)
        {
            return;
        }
        
        Debug.Log($"[AUTO RECOMPILER] Phát hiện thay đổi: {e.Name} ({e.ChangeType})");
        
        // Đánh dấu cần refresh sau một khoảng delay
        lastRefreshTime = Time.realtimeSinceStartup + REFRESH_DELAY;
    }
    
    static void OnScriptRenamed(object sender, RenamedEventArgs e)
    {
        Debug.Log($"[AUTO RECOMPILER] File đổi tên: {e.OldName} → {e.Name}");
        lastRefreshTime = Time.realtimeSinceStartup + REFRESH_DELAY;
    }
    
    static void CheckForDelayedRefresh()
    {
        // Kiểm tra xem có cần refresh không và đã đủ delay chưa
        if (lastRefreshTime > 0 && Time.realtimeSinceStartup >= lastRefreshTime)
        {
            lastRefreshTime = 0; // Reset
            
            // Thực hiện refresh AssetDatabase để Unity nhận diện thay đổi
            Debug.Log("[AUTO RECOMPILER] Đang refresh AssetDatabase...");
            AssetDatabase.Refresh();
            
            // Force recompile nếu cần
            if (!EditorApplication.isCompiling)
            {
                UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
                Debug.Log("[AUTO RECOMPILER] Đã yêu cầu compile lại scripts");
            }
        }
    }
    
    // Menu item để test thủ công
    [MenuItem("Tools/🔄 Force Recompile Scripts")]
    static void ForceRecompileManual()
    {
        Debug.Log("[AUTO RECOMPILER] Force recompile thủ công...");
        AssetDatabase.Refresh();
        UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
    }
    
    [MenuItem("Tools/🔄 Toggle Auto Recompiler")]
    static void ToggleAutoRecompiler()
    {
        if (watcher != null && watcher.EnableRaisingEvents)
        {
            watcher.EnableRaisingEvents = false;
            Debug.Log("[AUTO RECOMPILER] Đã TẮT auto recompiler");
        }
        else
        {
            if (watcher != null)
            {
                watcher.EnableRaisingEvents = true;
                Debug.Log("[AUTO RECOMPILER] Đã BẬT auto recompiler");
            }
            else
            {
                InitializeFileWatcher();
            }
        }
    }
    
    // Cleanup khi Unity Editor đóng
    static void OnEditorApplicationQuitting()
    {
        if (watcher != null)
        {
            watcher.Dispose();
            Debug.Log("[AUTO RECOMPILER] Đã cleanup file watcher");
        }
    }
}

/// <summary>
/// Script bổ sung để tự động refresh khi focus Unity window
/// </summary>
[InitializeOnLoad]
public class FocusRefresh
{
    static FocusRefresh()
    {
        EditorApplication.focusChanged += OnFocusChanged;
    }
    
    static void OnFocusChanged(bool hasFocus)
    {
        if (hasFocus)
        {
            // Khi Unity được focus lại, refresh để đảm bảo có thay đổi mới nhất
            AssetDatabase.Refresh();
            Debug.Log("[FOCUS REFRESH] Unity được focus - đã refresh AssetDatabase");
        }
    }
}

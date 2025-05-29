# Script PowerShell để copy cấu hình GitHub Copilot sang dự án mới
# Chạy từ thư mục gốc của dự án nguồn

param(
    [Parameter(Mandatory=$true)]
    [string]$TargetProjectPath
)

Write-Host "🚀 Đang copy cấu hình GitHub Copilot..." -ForegroundColor Green

# Kiểm tra đường dẫn đích
if (-not (Test-Path $TargetProjectPath)) {
    Write-Error "❌ Đường dẫn dự án đích không tồn tại: $TargetProjectPath"
    exit 1
}

# Tạo thư mục .github nếu chưa có
$githubDir = Join-Path $TargetProjectPath ".github"
if (-not (Test-Path $githubDir)) {
    New-Item -ItemType Directory -Path $githubDir -Force
    Write-Host "✅ Đã tạo thư mục .github" -ForegroundColor Yellow
}

# Copy file copilot-instructions.md
$sourceInstructions = ".github\copilot-instructions.md"
$targetInstructions = Join-Path $githubDir "copilot-instructions.md"

if (Test-Path $sourceInstructions) {
    Copy-Item $sourceInstructions $targetInstructions -Force
    Write-Host "✅ Đã copy copilot-instructions.md" -ForegroundColor Green
} else {
    Write-Warning "⚠️ Không tìm thấy file copilot-instructions.md"
}

# Tạo thư mục .vscode nếu chưa có
$vscodeDir = Join-Path $TargetProjectPath ".vscode"
if (-not (Test-Path $vscodeDir)) {
    New-Item -ItemType Directory -Path $vscodeDir -Force
    Write-Host "✅ Đã tạo thư mục .vscode" -ForegroundColor Yellow
}

# Xử lý file settings.json (merge thay vì overwrite)
$sourceSettings = ".vscode\settings.json"
$targetSettings = Join-Path $vscodeDir "settings.json"

if (Test-Path $sourceSettings) {
    if (Test-Path $targetSettings) {
        Write-Host "⚠️ File settings.json đã tồn tại. Cần merge thủ công." -ForegroundColor Yellow
        Write-Host "   Nguồn: $sourceSettings"
        Write-Host "   Đích: $targetSettings"
        
        # Tạo backup
        $backupSettings = Join-Path $vscodeDir "settings.json.backup"
        Copy-Item $targetSettings $backupSettings -Force
        Write-Host "✅ Đã backup settings.json cũ" -ForegroundColor Cyan
    } else {
        Copy-Item $sourceSettings $targetSettings -Force
        Write-Host "✅ Đã copy settings.json" -ForegroundColor Green
    }
} else {
    Write-Warning "⚠️ Không tìm thấy file settings.json"
}

# Copy file .editorconfig
$sourceEditorConfig = ".editorconfig"
$targetEditorConfig = Join-Path $TargetProjectPath ".editorconfig"

if (Test-Path $sourceEditorConfig) {
    if (Test-Path $targetEditorConfig) {
        Write-Host "⚠️ File .editorconfig đã tồn tại. Cần merge thủ công." -ForegroundColor Yellow
        
        # Tạo backup
        $backupEditorConfig = Join-Path $TargetProjectPath ".editorconfig.backup"
        Copy-Item $targetEditorConfig $backupEditorConfig -Force
        Write-Host "✅ Đã backup .editorconfig cũ" -ForegroundColor Cyan
    } else {
        Copy-Item $sourceEditorConfig $targetEditorConfig -Force
        Write-Host "✅ Đã copy .editorconfig" -ForegroundColor Green
    }
} else {
    Write-Warning "⚠️ Không tìm thấy file .editorconfig"
}

# Copy file README (tùy chọn)
$sourceReadme = "COPILOT_SETUP_README.md"
$targetReadme = Join-Path $TargetProjectPath "COPILOT_SETUP_README.md"

if (Test-Path $sourceReadme) {
    Copy-Item $sourceReadme $targetReadme -Force
    Write-Host "✅ Đã copy COPILOT_SETUP_README.md" -ForegroundColor Green
} else {
    Write-Warning "⚠️ Không tìm thấy file COPILOT_SETUP_README.md"
}

Write-Host ""
Write-Host "🎉 HOÀN THÀNH! Cấu hình GitHub Copilot đã được copy." -ForegroundColor Green
Write-Host ""
Write-Host "📋 BƯỚC TIẾP THEO:" -ForegroundColor Cyan
Write-Host "1. Mở VS Code trong dự án mới"
Write-Host "2. Restart VS Code để áp dụng settings"
Write-Host "3. Kiểm tra GitHub Copilot hoạt động"
Write-Host "4. Merge các file settings nếu có conflict"
Write-Host ""
Write-Host "💡 TIP: Nếu có file backup (.backup), hãy merge thủ công!" -ForegroundColor Yellow

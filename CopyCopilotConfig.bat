@echo off
echo 🚀 Copy cấu hình GitHub Copilot sang dự án mới
echo.

if "%1"=="" (
    echo ❌ Cách sử dụng: CopyCopilotConfig.bat "C:\path\to\new\project"
    echo.
    echo Ví dụ: CopyCopilotConfig.bat "C:\Unity\MyNewProject"
    pause
    exit /b 1
)

set TARGET_PATH=%1

echo 📁 Đích: %TARGET_PATH%
echo.

if not exist "%TARGET_PATH%" (
    echo ❌ Thư mục đích không tồn tại!
    pause
    exit /b 1
)

echo ✅ Tạo thư mục .github...
if not exist "%TARGET_PATH%\.github" mkdir "%TARGET_PATH%\.github"

echo ✅ Copy copilot-instructions.md...
if exist ".github\copilot-instructions.md" (
    copy ".github\copilot-instructions.md" "%TARGET_PATH%\.github\copilot-instructions.md" >nul
    echo    📄 Đã copy copilot-instructions.md
) else (
    echo    ⚠️ Không tìm thấy copilot-instructions.md
)

echo ✅ Tạo thư mục .vscode...
if not exist "%TARGET_PATH%\.vscode" mkdir "%TARGET_PATH%\.vscode"

echo ✅ Copy .editorconfig...
if exist ".editorconfig" (
    if exist "%TARGET_PATH%\.editorconfig" (
        echo    ⚠️ .editorconfig đã tồn tại - cần merge thủ công
        copy ".editorconfig" "%TARGET_PATH%\.editorconfig.new" >nul
        echo    📄 Đã tạo .editorconfig.new
    ) else (
        copy ".editorconfig" "%TARGET_PATH%\.editorconfig" >nul
        echo    📄 Đã copy .editorconfig
    )
) else (
    echo    ⚠️ Không tìm thấy .editorconfig
)

echo ✅ Copy settings.json...
if exist ".vscode\settings.json" (
    if exist "%TARGET_PATH%\.vscode\settings.json" (
        echo    ⚠️ settings.json đã tồn tại - cần merge thủ công
        copy ".vscode\settings.json" "%TARGET_PATH%\.vscode\settings.json.new" >nul
        echo    📄 Đã tạo settings.json.new
    ) else (
        copy ".vscode\settings.json" "%TARGET_PATH%\.vscode\settings.json" >nul
        echo    📄 Đã copy settings.json
    )
) else (
    echo    ⚠️ Không tìm thấy settings.json
)

echo ✅ Copy README...
if exist "COPILOT_SETUP_README.md" (
    copy "COPILOT_SETUP_README.md" "%TARGET_PATH%\COPILOT_SETUP_README.md" >nul
    echo    📄 Đã copy COPILOT_SETUP_README.md
) else (
    echo    ⚠️ Không tìm thấy COPILOT_SETUP_README.md
)

echo.
echo 🎉 HOÀN THÀNH!
echo.
echo 📋 BƯỚC TIẾP THEO:
echo 1. Mở VS Code trong dự án mới
echo 2. Restart VS Code để áp dụng cấu hình
echo 3. Test GitHub Copilot bằng cách tạo comment tiếng Việt
echo 4. Merge các file .new nếu có conflict
echo.
echo 💡 Nếu có file .new, hãy merge thủ công vào file gốc!
echo.
pause

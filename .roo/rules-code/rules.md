# ROO CODING RULES

## Unity Project Structure
- luôn luôn kiểm tra script trong thư mục assets/animalrevolt đầu tiên
- chỉ sử dụng prefab trong assets/prefabs
- tạo các script mới trong thư mục assets/animalrevolt

## Script Development Guidelines
- KHÔNG tự động tạo các script test, demo, hoặc scene manager
- KHÔNG tạo script AITestSceneManager, TestManager, DemoScript hay tương tự
- CHỈ tạo script khi user YÊU CẦU CỤ THỂ
- Ưu tiên tạo hướng dẫn manual và documentation thay vì test scripts
- Khi user cần test → hướng dẫn setup manual qua Inspector
- Focus vào core functionality scripts, không tạo helper/utility không cần thiết

## Code Quality
- Luôn validate components trước khi sử dụng
- Sử dụng reflection cẩn thận, có error handling
- Debug logs rõ ràng với emoji để dễ đọc
- Documentation đầy đủ cho public methods

## User Experience
- Tạo hướng dẫn step-by-step chi tiết
- Cung cấp troubleshooting guide
- Alternative methods khi features không hoạt động
- UI-friendly với Inspector buttons thay vì context menu phức tạp

## MCP Unity Commands
### execute_menu_item
Executes a Unity menu item by path
- menuPath*: The path to the menu item to execute (e.g. "GameObject/Create Empty")

### select_gameobject
Sets the selected GameObject in the Unity editor by path or instance ID
- objectPath: The path or name of the GameObject to select (e.g. "Main Camera")
- instanceId: The instance ID of the GameObject to select

### add_package
Adds packages into the Unity Package Manager
- source*: The source to use (registry, github, or disk) to add the package
- packageName: The package name to add from Unity registry (e.g. com.unity.textmeshpro)
- version: The version to use for registry packages (optional)
- repositoryUrl: The GitHub repository URL (e.g. https://github.com/username/repo.git)
- branch: The branch to use for GitHub packages (optional)
- path: The path to use (folder path for disk method or subfolder for GitHub)

### run_tests
Runs Unity's Test Runner tests
- testMode: The test mode to run (EditMode or PlayMode) - defaults to EditMode (optional)
- testFilter: The specific test filter to run (e.g. specific test name or namespace) (optional)
- returnOnlyFailures: Whether to show only failed tests in the results (optional)

### send_console_log
Sends console log messages to the Unity console
- message*: The message to display in the Unity console
- type: The type of message (info, warning, error) - defaults to info (optional)

### get_console_logs
Retrieves logs from the Unity console with pagination support to avoid token limits
- logType: The type of logs to retrieve (info, warning, error) - defaults to all logs if not specified
- offset: Starting index for pagination (0-based, defaults to 0)
- limit: Maximum number of logs to return (defaults to 50, max 500 to avoid token limits)
- includeStackTrace: Whether to include stack trace in logs. ⚠️ ALWAYS SET TO FALSE to save 80-90% tokens, unless you specifically need stack traces for debugging. Default: true (except info logs in resource)

### update_component
Updates component fields on a GameObject or adds it to the GameObject if it does not contain the component
- instanceId: The instance ID of the GameObject to update
- objectPath: The path of the GameObject in the hierarchy to update (alternative to instanceId)
- componentName*: The name of the component to update or add
- componentData: An object containing the fields to update on the component (optional)

### add_asset_to_scene
Adds an asset from the AssetDatabase to the Unity scene
- assetPath: The path of the asset in the AssetDatabase
- guid: The GUID of the asset
- position: Position in the scene (defaults to Vector3.zero)
- parentPath: The path of the parent GameObject in the hierarchy
- parentId: The instance ID of the parent GameObject

### update_gameobject
Updates properties of a GameObject in the Unity scene by its instance ID or path. If the GameObject does not exist at the specified path, it will be created.
- instanceId: The instance ID of the GameObject to update
- objectPath: The path of the GameObject in the hierarchy to update (alternative to instanceId)
- gameObjectData*: An object containing the fields to update on the GameObject. If the GameObject does not exist at objectPath, it will be created.

## MCP Unity Resources
### unity://tests/{testMode}
get_tests: Retrieve tests from Unity's Test Runner
Returns application/json

### unity://gameobject/{id}
get_gameobject: Retrieve a GameObject by ID or path
Returns application/json

### unity://logs/{logType}?offset={offset}&limit={limit}&includeStackTrace={includeStackTrace}
get_console_logs: Retrieve Unity console logs by type with pagination support
Returns application/json

### unity://menu-items
get_menu_items: List of available menu items in Unity to execute
Returns application/json

### unity://scenes_hierarchy
get_scenes_hierarchy: Retrieve all GameObjects in the Unity loaded scenes with their active state
Returns application/json

### unity://packages
get_packages: Retrieve all packages from the Unity Package Manager
Returns application/json

### unity://assets
get_assets: Retrieve assets from the Unity Asset Database
Returns application/json

### Unity Resource Examples
- unity://tests/EditMode - List only 'EditMode' tests
- unity://tests/PlayMode - List only 'PlayMode' tests  
- unity://tests/ - List all tests
- unity://logs/?offset=0&limit=50&includeStackTrace=true - All logs
- unity://logs/error?offset=0&limit=20&includeStackTrace=true - Error logs
- unity://logs/warning?offset=0&limit=30&includeStackTrace=true - Warning logs
- unity://logs/info?offset=0&limit=25&includeStackTrace=false - Info logs
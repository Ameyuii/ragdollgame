ưu tiên kết nối mcp
MCP Server Tools
The following tools are available for manipulating and querying Unity scenes and GameObjects via MCP:

execute_menu_item: Executes Unity menu items (functions tagged with the MenuItem attribute)

Example prompt: "Execute the menu item 'GameObject/Create Empty' to create a new empty GameObject"

select_gameobject: Selects game objects in the Unity hierarchy by path or instance ID

Example prompt: "Select the Main Camera object in my scene"

update_gameobject: Updates a GameObject's core properties (name, tag, layer, active/static state), or creates the GameObject if it does not exist

Example prompt: "Set the Player object's tag to 'Enemy' and make it inactive"

update_component: Updates component fields on a GameObject or adds it to the GameObject if it does not contain the component

Example prompt: "Add a Rigidbody component to the Player object and set its mass to 5"

add_package: Installs new packages in the Unity Package Manager

Example prompt: "Add the TextMeshPro package to my project"

run_tests: Runs tests using the Unity Test Runner

Example prompt: "Run all the EditMode tests in my project"

send_console_log: Send a console log to Unity

Example prompt: "Send a console log to Unity Editor"

add_asset_to_scene: Adds an asset from the AssetDatabase to the Unity scene

Example prompt: "Add the Player prefab from my project to the current scene"

MCP Server Resources
unity://menu-items: Retrieves a list of all available menu items in the Unity Editor to facilitate execute_menu_item tool

Example prompt: "Show me all available menu items related to GameObject creation"

unity://scenes-hierarchy: Retrieves a list of all game objects in the current Unity scene hierarchy

Example prompt: "Show me the current scenes hierarchy structure"

unity://gameobject/{id}: Retrieves detailed information about a specific GameObject by instance ID or object path in the scene hierarchy, including all GameObject components with it's serialized properties and fields

Example prompt: "Get me detailed information about the Player GameObject"

unity://logs: Retrieves a list of all logs from the Unity console

Example prompt: "Show me the recent error messages from the Unity console"

unity://packages: Retrieves information about installed and available packages from the Unity Package Manager

Example prompt: "List all the packages currently installed in my Unity project"

unity://assets: Retrieves information about assets in the Unity Asset Database

Example prompt: "Find all texture assets in my project"

unity://tests/{testMode}: Retrieves information about tests in the Unity Test Runner

Example prompt: "List all available tests in my Unity project"
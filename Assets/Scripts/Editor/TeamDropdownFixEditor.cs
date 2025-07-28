using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class TeamDropdownFixEditor : EditorWindow
{
    [MenuItem("Tools/Fix Team Dropdown")]
    public static void ShowWindow()
    {
        FixTeamDropdown();
    }
    
    public static void FixTeamDropdown()
    {
        Debug.Log("🔧 TEAM DROPDOWN FIX - STARTING");
        
        // Find TeamDropdownController
        TeamDropdownController controller = Object.FindObjectOfType<TeamDropdownController>();
        if (controller == null)
        {
            Debug.LogError("❌ TeamDropdownController not found in scene!");
            return;
        }
        
        Debug.Log("✅ Found TeamDropdownController component");
        
        // Check current references
        Debug.Log("🔍 CURRENT REFERENCES CHECK:");
        Debug.Log($"teamDropdownButton: {(controller.teamDropdownButton != null ? "ASSIGNED" : "NULL")}");
        Debug.Log($"teamDropdownPanel: {(controller.teamDropdownPanel != null ? "ASSIGNED" : "NULL")}");
        Debug.Log($"team1Option: {(controller.team1Option != null ? "ASSIGNED" : "NULL")}");
        Debug.Log($"team2Option: {(controller.team2Option != null ? "ASSIGNED" : "NULL")}");
        Debug.Log($"teamDropdownButtonText: {(controller.teamDropdownButtonText != null ? "ASSIGNED" : "NULL")}");
        
        // Find the GameObjects manually and assign
        Debug.Log("🔧 FIXING REFERENCES:");
        
        // 1. TeamDropdownButton
        if (controller.teamDropdownButton == null)
        {
            GameObject buttonObj = GameObject.Find("TeamDropdownButton");
            if (buttonObj != null)
            {
                controller.teamDropdownButton = buttonObj.GetComponent<Button>();
                Debug.Log("✅ Fixed teamDropdownButton reference");
                
                // Also ensure target graphic is set
                Image buttonImage = buttonObj.GetComponent<Image>();
                if (buttonImage != null && controller.teamDropdownButton.targetGraphic == null)
                {
                    controller.teamDropdownButton.targetGraphic = buttonImage;
                }
            }
            else
            {
                Debug.LogError("❌ TeamDropdownButton GameObject not found!");
            }
        }
        
        // 2. TeamDropdownPanel
        if (controller.teamDropdownPanel == null)
        {
            GameObject panelObj = GameObject.Find("DropdownPanel");
            if (panelObj != null)
            {
                controller.teamDropdownPanel = panelObj;
                panelObj.SetActive(false); // Ensure it's hidden
                Debug.Log("✅ Fixed teamDropdownPanel reference");
            }
            else
            {
                Debug.LogError("❌ DropdownPanel GameObject not found!");
            }
        }
        
        // 3. Team1Option
        if (controller.team1Option == null)
        {
            GameObject team1Obj = GameObject.Find("TeamButton_1");
            if (team1Obj != null)
            {
                controller.team1Option = team1Obj.GetComponent<Button>();
                Debug.Log("✅ Fixed team1Option reference");
            }
            else
            {
                Debug.LogError("❌ TeamButton_1 GameObject not found!");
            }
        }
        
        // 4. Team2Option  
        if (controller.team2Option == null)
        {
            GameObject team2Obj = GameObject.Find("TeamButton_2");
            if (team2Obj != null)
            {
                controller.team2Option = team2Obj.GetComponent<Button>();
                Debug.Log("✅ Fixed team2Option reference");
            }
            else
            {
                Debug.LogError("❌ TeamButton_2 GameObject not found!");
            }
        }
        
        // 5. Button Text
        if (controller.teamDropdownButtonText == null && controller.teamDropdownButton != null)
        {
            Text buttonText = controller.teamDropdownButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                controller.teamDropdownButtonText = buttonText;
                Debug.Log("✅ Fixed teamDropdownButtonText reference");
            }
            else
            {
                Debug.LogWarning("⚠️ Text component not found in TeamDropdownButton");
                
                // Create text if missing
                GameObject textChild = new GameObject("Text");
                textChild.transform.SetParent(controller.teamDropdownButton.transform);
                
                RectTransform textRect = textChild.AddComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.sizeDelta = Vector2.zero;
                textRect.anchoredPosition = Vector2.zero;
                
                Text newText = textChild.AddComponent<Text>();
                newText.text = "TEAM 1";
                newText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                newText.fontSize = 14;
                newText.color = Color.white;
                newText.alignment = TextAnchor.MiddleCenter;
                
                controller.teamDropdownButtonText = newText;
                Debug.Log("✅ Created new Text component for TeamDropdownButton");
            }
        }
        
        // Now setup button listeners
        Debug.Log("🔗 SETTING UP BUTTON LISTENERS:");
        
        if (controller.teamDropdownButton != null)
        {
            controller.teamDropdownButton.onClick.RemoveAllListeners();
            controller.teamDropdownButton.onClick.AddListener(() => {
                Debug.Log("🖱️ TeamDropdownButton clicked - calling ToggleDropdown");
                controller.ToggleDropdown();
            });
            Debug.Log("✅ Main button onClick listener added");
        }
        
        if (controller.team1Option != null)
        {
            controller.team1Option.onClick.RemoveAllListeners();
            controller.team1Option.onClick.AddListener(() => {
                Debug.Log("🖱️ Team 1 option clicked");
                controller.SelectTeam(1);
            });
            Debug.Log("✅ Team 1 onClick listener added");
        }
        
        if (controller.team2Option != null)
        {
            controller.team2Option.onClick.RemoveAllListeners();
            controller.team2Option.onClick.AddListener(() => {
                Debug.Log("🖱️ Team 2 option clicked");
                controller.SelectTeam(2);
            });
            Debug.Log("✅ Team 2 onClick listener added");
        }
        
        // Force update button appearance
        if (controller.teamDropdownButtonText != null)
        {
            controller.teamDropdownButtonText.text = "TEAM 1";
        }
        
        if (controller.teamDropdownButton != null)
        {
            Image buttonImage = controller.teamDropdownButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = new Color(0.2f, 0.4f, 0.8f, 1f); // Team 1 blue
            }
        }
        
        // Mark scene as dirty so changes are saved
        if (!Application.isPlaying)
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }
        
        Debug.Log("🎉 TEAM DROPDOWN FIX COMPLETED!");
        Debug.Log("🧪 TEST: Try clicking the TeamDropdownButton now!");
    }
}
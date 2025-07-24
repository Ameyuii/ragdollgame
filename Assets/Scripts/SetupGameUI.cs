using UnityEngine;
using UnityEngine.UI;

public class SetupGameUI : MonoBehaviour
{
    public static void Execute()
    {
        // Find or create Canvas
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("UI Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            
            canvasGO.AddComponent<GraphicRaycaster>();
        }
        
        // Clear existing UI
        foreach (Transform child in canvas.transform)
        {
            DestroyImmediate(child.gameObject);
        }
        
        // Create UI Panel
        GameObject panel = new GameObject("GameUI Panel");
        panel.transform.SetParent(canvas.transform, false);
        
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.3f); // Semi-transparent background
        
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        // Create Title
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(panel.transform, false);
        
        Text titleText = titleGO.AddComponent<Text>();
        titleText.text = "RAGDOLL BATTLE SIMULATOR";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 48;
        titleText.color = Color.yellow;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.fontStyle = FontStyle.Bold;
        
        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.9f);
        titleRect.anchorMax = new Vector2(0.5f, 0.9f);
        titleRect.anchoredPosition = Vector2.zero;
        titleRect.sizeDelta = new Vector2(800, 60);
        
        // Create Team 1 Counter
        GameObject team1GO = new GameObject("Team1Counter");
        team1GO.transform.SetParent(panel.transform, false);
        
        Text team1Text = team1GO.AddComponent<Text>();
        team1Text.text = "TEAM 1 (BLUE): 0";
        team1Text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        team1Text.fontSize = 32;
        team1Text.color = Color.cyan;
        team1Text.alignment = TextAnchor.MiddleLeft;
        team1Text.fontStyle = FontStyle.Bold;
        
        RectTransform team1Rect = team1GO.GetComponent<RectTransform>();
        team1Rect.anchorMin = new Vector2(0.05f, 0.8f);
        team1Rect.anchorMax = new Vector2(0.05f, 0.8f);
        team1Rect.anchoredPosition = Vector2.zero;
        team1Rect.sizeDelta = new Vector2(400, 40);
        
        // Create Team 2 Counter
        GameObject team2GO = new GameObject("Team2Counter");
        team2GO.transform.SetParent(panel.transform, false);
        
        Text team2Text = team2GO.AddComponent<Text>();
        team2Text.text = "TEAM 2 (RED): 0";
        team2Text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        team2Text.fontSize = 32;
        team2Text.color = Color.red;
        team2Text.alignment = TextAnchor.MiddleRight;
        team2Text.fontStyle = FontStyle.Bold;
        
        RectTransform team2Rect = team2GO.GetComponent<RectTransform>();
        team2Rect.anchorMin = new Vector2(0.95f, 0.8f);
        team2Rect.anchorMax = new Vector2(0.95f, 0.8f);
        team2Rect.anchoredPosition = Vector2.zero;
        team2Rect.sizeDelta = new Vector2(400, 40);
        
        // Create Status Text
        GameObject statusGO = new GameObject("StatusText");
        statusGO.transform.SetParent(panel.transform, false);
        
        Text statusText = statusGO.AddComponent<Text>();
        statusText.text = "Press START BATTLE to begin the epic ragdoll fight!";
        statusText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        statusText.fontSize = 28;
        statusText.color = Color.white;
        statusText.alignment = TextAnchor.MiddleCenter;
        statusText.fontStyle = FontStyle.Italic;
        
        RectTransform statusRect = statusGO.GetComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0.5f, 0.7f);
        statusRect.anchorMax = new Vector2(0.5f, 0.7f);
        statusRect.anchoredPosition = Vector2.zero;
        statusRect.sizeDelta = new Vector2(800, 40);
        
        // Create Start Button
        GameObject startButtonGO = new GameObject("StartButton");
        startButtonGO.transform.SetParent(panel.transform, false);
        
        Button startButton = startButtonGO.AddComponent<Button>();
        Image startButtonImage = startButtonGO.AddComponent<Image>();
        startButtonImage.color = new Color(0.2f, 0.8f, 0.2f, 0.8f); // Green
        
        RectTransform startButtonRect = startButtonGO.GetComponent<RectTransform>();
        startButtonRect.anchorMin = new Vector2(0.3f, 0.1f);
        startButtonRect.anchorMax = new Vector2(0.3f, 0.1f);
        startButtonRect.anchoredPosition = Vector2.zero;
        startButtonRect.sizeDelta = new Vector2(200, 60);
        
        // Start Button Text
        GameObject startTextGO = new GameObject("Text");
        startTextGO.transform.SetParent(startButtonGO.transform, false);
        
        Text startButtonText = startTextGO.AddComponent<Text>();
        startButtonText.text = "START BATTLE";
        startButtonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        startButtonText.fontSize = 24;
        startButtonText.color = Color.white;
        startButtonText.alignment = TextAnchor.MiddleCenter;
        startButtonText.fontStyle = FontStyle.Bold;
        
        RectTransform startTextRect = startTextGO.GetComponent<RectTransform>();
        startTextRect.anchorMin = Vector2.zero;
        startTextRect.anchorMax = Vector2.one;
        startTextRect.offsetMin = Vector2.zero;
        startTextRect.offsetMax = Vector2.zero;
        
        // Create Reset Button
        GameObject resetButtonGO = new GameObject("ResetButton");
        resetButtonGO.transform.SetParent(panel.transform, false);
        
        Button resetButton = resetButtonGO.AddComponent<Button>();
        Image resetButtonImage = resetButtonGO.AddComponent<Image>();
        resetButtonImage.color = new Color(0.8f, 0.2f, 0.2f, 0.8f); // Red
        
        RectTransform resetButtonRect = resetButtonGO.GetComponent<RectTransform>();
        resetButtonRect.anchorMin = new Vector2(0.7f, 0.1f);
        resetButtonRect.anchorMax = new Vector2(0.7f, 0.1f);
        resetButtonRect.anchoredPosition = Vector2.zero;
        resetButtonRect.sizeDelta = new Vector2(200, 60);
        
        // Reset Button Text
        GameObject resetTextGO = new GameObject("Text");
        resetTextGO.transform.SetParent(resetButtonGO.transform, false);
        
        Text resetButtonText = resetTextGO.AddComponent<Text>();
        resetButtonText.text = "RESET BATTLE";
        resetButtonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        resetButtonText.fontSize = 24;
        resetButtonText.color = Color.white;
        resetButtonText.alignment = TextAnchor.MiddleCenter;
        resetButtonText.fontStyle = FontStyle.Bold;
        
        RectTransform resetTextRect = resetTextGO.GetComponent<RectTransform>();
        resetTextRect.anchorMin = Vector2.zero;
        resetTextRect.anchorMax = Vector2.one;
        resetTextRect.offsetMin = Vector2.zero;
        resetTextRect.offsetMax = Vector2.zero;
        
        // Update GameManager references
        GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.team1CountText = team1Text;
            gameManager.team2CountText = team2Text;
            gameManager.gameStatusText = statusText;
            gameManager.startBattleButton = startButton;
            gameManager.resetBattleButton = resetButton;
            
            // Add button listeners
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(gameManager.StartBattle);
            
            resetButton.onClick.RemoveAllListeners();
            resetButton.onClick.AddListener(gameManager.ResetBattle);
        }
        
        Debug.Log("Game UI setup completed!");
    }
}
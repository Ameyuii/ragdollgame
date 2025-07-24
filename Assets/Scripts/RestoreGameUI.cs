using UnityEngine;
using UnityEngine.UI;

public class RestoreGameUI : MonoBehaviour
{
    public static void Execute()
    {
        Debug.Log("Restoring Game UI...");
        
        // Find UI Canvas
        GameObject uiCanvas = GameObject.Find("UI Canvas");
        if (uiCanvas == null)
        {
            Debug.LogError("UI Canvas not found!");
            return;
        }
        
        // Create GameUI Panel
        GameObject gameUIPanel = new GameObject("GameUI Panel");
        gameUIPanel.transform.SetParent(uiCanvas.transform, false);
        
        // Add Image component for background
        Image panelImage = gameUIPanel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.5f); // Semi-transparent black
        
        // Set RectTransform to fill screen
        RectTransform panelRect = gameUIPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        // Create Title
        GameObject title = new GameObject("Title");
        title.transform.SetParent(gameUIPanel.transform, false);
        Text titleText = title.AddComponent<Text>();
        titleText.text = "Battle Arena";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 36;
        titleText.color = Color.white;
        titleText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.8f);
        titleRect.anchorMax = new Vector2(0.5f, 0.9f);
        titleRect.sizeDelta = new Vector2(300, 50);
        
        // Create Team1 Counter
        GameObject team1Counter = new GameObject("Team1Counter");
        team1Counter.transform.SetParent(gameUIPanel.transform, false);
        Text team1Text = team1Counter.AddComponent<Text>();
        team1Text.text = "Team 1: 0";
        team1Text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        team1Text.fontSize = 24;
        team1Text.color = Color.blue;
        team1Text.alignment = TextAnchor.MiddleLeft;
        
        RectTransform team1Rect = team1Counter.GetComponent<RectTransform>();
        team1Rect.anchorMin = new Vector2(0.1f, 0.85f);
        team1Rect.anchorMax = new Vector2(0.4f, 0.95f);
        team1Rect.sizeDelta = new Vector2(200, 30);
        
        // Create Team2 Counter
        GameObject team2Counter = new GameObject("Team2Counter");
        team2Counter.transform.SetParent(gameUIPanel.transform, false);
        Text team2Text = team2Counter.AddComponent<Text>();
        team2Text.text = "Team 2: 0";
        team2Text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        team2Text.fontSize = 24;
        team2Text.color = Color.red;
        team2Text.alignment = TextAnchor.MiddleRight;
        
        RectTransform team2Rect = team2Counter.GetComponent<RectTransform>();
        team2Rect.anchorMin = new Vector2(0.6f, 0.85f);
        team2Rect.anchorMax = new Vector2(0.9f, 0.95f);
        team2Rect.sizeDelta = new Vector2(200, 30);
        
        // Create Status Text
        GameObject statusText = new GameObject("StatusText");
        statusText.transform.SetParent(gameUIPanel.transform, false);
        Text statusTextComp = statusText.AddComponent<Text>();
        statusTextComp.text = "Ready to battle!";
        statusTextComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        statusTextComp.fontSize = 20;
        statusTextComp.color = Color.yellow;
        statusTextComp.alignment = TextAnchor.MiddleCenter;
        
        RectTransform statusRect = statusText.GetComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0.2f, 0.7f);
        statusRect.anchorMax = new Vector2(0.8f, 0.8f);
        statusRect.sizeDelta = new Vector2(400, 30);
        
        // Create Start Button
        GameObject startButton = new GameObject("StartButton");
        startButton.transform.SetParent(gameUIPanel.transform, false);
        
        Image startButtonImage = startButton.AddComponent<Image>();
        startButtonImage.color = Color.green;
        
        Button startButtonComp = startButton.AddComponent<Button>();
        
        RectTransform startButtonRect = startButton.GetComponent<RectTransform>();
        startButtonRect.anchorMin = new Vector2(0.3f, 0.1f);
        startButtonRect.anchorMax = new Vector2(0.45f, 0.2f);
        startButtonRect.sizeDelta = new Vector2(120, 40);
        
        // Start Button Text
        GameObject startButtonText = new GameObject("Text");
        startButtonText.transform.SetParent(startButton.transform, false);
        Text startBtnText = startButtonText.AddComponent<Text>();
        startBtnText.text = "Start Battle";
        startBtnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        startBtnText.fontSize = 16;
        startBtnText.color = Color.white;
        startBtnText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform startBtnTextRect = startButtonText.GetComponent<RectTransform>();
        startBtnTextRect.anchorMin = Vector2.zero;
        startBtnTextRect.anchorMax = Vector2.one;
        startBtnTextRect.offsetMin = Vector2.zero;
        startBtnTextRect.offsetMax = Vector2.zero;
        
        // Create Reset Button
        GameObject resetButton = new GameObject("ResetButton");
        resetButton.transform.SetParent(gameUIPanel.transform, false);
        
        Image resetButtonImage = resetButton.AddComponent<Image>();
        resetButtonImage.color = Color.red;
        
        Button resetButtonComp = resetButton.AddComponent<Button>();
        
        RectTransform resetButtonRect = resetButton.GetComponent<RectTransform>();
        resetButtonRect.anchorMin = new Vector2(0.55f, 0.1f);
        resetButtonRect.anchorMax = new Vector2(0.7f, 0.2f);
        resetButtonRect.sizeDelta = new Vector2(120, 40);
        
        // Reset Button Text
        GameObject resetButtonText = new GameObject("Text");
        resetButtonText.transform.SetParent(resetButton.transform, false);
        Text resetBtnText = resetButtonText.AddComponent<Text>();
        resetBtnText.text = "Reset";
        resetBtnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        resetBtnText.fontSize = 16;
        resetBtnText.color = Color.white;
        resetBtnText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform resetBtnTextRect = resetButtonText.GetComponent<RectTransform>();
        resetBtnTextRect.anchorMin = Vector2.zero;
        resetBtnTextRect.anchorMax = Vector2.one;
        resetBtnTextRect.offsetMin = Vector2.zero;
        resetBtnTextRect.offsetMax = Vector2.zero;
        
        Debug.Log("Game UI restored successfully!");
    }
}
using UnityEngine;

public class EnableStartButton : MonoBehaviour
{
    public static void Execute()
    {
        GameObject startButton = GameObject.Find("StartButton");
        if (startButton != null)
        {
            startButton.SetActive(true);
            Debug.Log("Start button enabled");
        }
    }
}
using GameAnalyticsSDK;
using UnityEngine;

public class GameAnalyticsManager : MonoBehaviour
{
    public bool isEnable;

    public static GameAnalyticsManager Instance { get; private set; }

    private void Awake()
    {
        if (isEnable == false) return;

        Instance = this;
        GameAnalytics.Initialize();
    }

    public void Log_StartLevel()
    {
        if (isEnable == false) return;

        GameAnalytics.NewProgressionEvent(
            GAProgressionStatus.Start,
            Application.version,
            GameController.instance.currentLevel.ToString("00000"));
    }

    public void Log_EndLevel()
    {
        if (isEnable == false) return;

        GameAnalytics.NewProgressionEvent(
            GAProgressionStatus.Complete,
            Application.version,
            GameController.instance.currentLevel.ToString("00000"));
    }
}
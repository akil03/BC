using GameAnalyticsSDK;
using UnityEngine;

public class GameAnalyticsManager : MonoBehaviour
{

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    private void Awake()
    {
        GameAnalytics.Initialize();
    }

    void GameStarted()
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "game");
    }

    void GameCompleted(int score)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "game", score);
    }
}
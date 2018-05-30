using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class SocialManager : MonoBehaviour
{
    private void OnEnable()
    {
        GameUtility.Instance.OnSocialLoginSuccess += LoadAchievements;
        GameUtility.Instance.OnUpdateAchievement += UpdateAchievement;
        GameUtility.Instance.OnDisplayAchievements += DisplayAchievements;
        GameUtility.Instance.OnLeaderboardDisplay += DisplayLeaderboard;
        GameUtility.Instance.OnUpdateLeaderboard += UpdateLeaderboard;
        GameUtility.Instance.OnSignOut += SignOut;
    }

    private void OnDisable()
    {
        GameUtility.Instance.OnSocialLoginSuccess -= LoadAchievements;
        GameUtility.Instance.OnUpdateAchievement -= UpdateAchievement;
        GameUtility.Instance.OnDisplayAchievements -= DisplayAchievements;
        GameUtility.Instance.OnLeaderboardDisplay -= DisplayLeaderboard;
        GameUtility.Instance.OnUpdateLeaderboard -= UpdateLeaderboard;
        GameUtility.Instance.OnSignOut -= SignOut;
    }

    private void Awake()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.Activate();
        }
        Login();
    }

    void Login()
    {
        Social.Active.Authenticate(Social.localUser, response =>
         {
             if (response)
             {
                 GameUtility.Instance.SocialLoginSuccess();
                 GameUtility.Instance.UpdateAchievement(GPGSIds.achievement_welcome, 100);
             }
             else
             {
                 DebugText.instance.Print("Login failed.");
             }
         });
    }

    private void LoadAchievements()
    {
        Social.LoadAchievements(success =>
        {
            if (success != null)
            {
                GameUtility.Instance.GotAchievements(success);
            }
        });
    }

    private void UpdateAchievement(string id, int value)
    {
        if (Application.platform == RuntimePlatform.Android && value != 100)
        {
            PlayGamesPlatform.Instance.IncrementAchievement(id, value, (bool success) =>
            {
                if (success)
                {
                    DebugText.instance.Print(id + " has incremented successfully!");
                }
                else
                {
                    DebugText.instance.Print(id + " has failed to increment!");
                }
            });
        }
        else
        {
            Social.ReportProgress(id, double.Parse(value.ToString()), success =>
            {
                if (success)
                {
                    DebugText.instance.Print(id + " has progressed successfully!");
                }
                else
                {
                    DebugText.instance.Print(id + " has failed to progress!");
                }
            });
        }
    }

    private void DisplayAchievements()
    {
        Social.ShowAchievementsUI();
    }

    private void DisplayLeaderboard()
    {
        Social.ShowLeaderboardUI();
    }

    private void UpdateLeaderboard(string id, int score)
    {
        Social.ReportScore(score, id, (bool success) =>
        {
            if (success)
            {
                DebugText.instance.Print("Score successfully updated!");
            }
        });
    }

    private void SignOut()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            PlayGamesPlatform.Instance.SignOut();
        }
    }
}

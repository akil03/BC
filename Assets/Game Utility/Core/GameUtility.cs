using UnityEngine.Purchasing;
using UnityEngine.SocialPlatforms;

public class GameUtility : Singleton<GameUtility>
{
    public ParameterlessDelegate OnSocialLoginSuccess;
    public ParameterlessDelegate OnDisplayAchievements;
    public ParameterlessDelegate OnLeaderboardDisplay;
    public IAchievementDelegate OnGotAchievements;
    public AchievementDelegate OnUpdateAchievement;
    public AchievementDelegate OnUpdateLeaderboard;
    public ChallengeDelegate OnStartChallenge;
    public ParameterlessDelegate OnSignOut;
    public StringDelegate OnPurchaseProduct;
    public PurchaseArgsDelegate OnProductPurchased;
    public ParameterlessDelegate OnGameStart;
    public IntDelegate OnGameComplete;

    public void SocialLoginSuccess()
    {
        if (OnSocialLoginSuccess != null)
        {
            OnSocialLoginSuccess();
        }
    }

    public void DisplayAchievements()
    {
        if (OnDisplayAchievements != null)
        {
            OnDisplayAchievements();
        }
    }

    public void DisplayLeaderboard()
    {
        if (OnLeaderboardDisplay != null)
        {
            OnLeaderboardDisplay();
        }
    }

    public void GotAchievements(IAchievement[] achievements)
    {
        if (OnGotAchievements != null)
        {
            OnGotAchievements(achievements);
        }
    }

    public void UpdateAchievement(string id, int value)
    {
        if (OnUpdateAchievement != null)
        {
            OnUpdateAchievement(id, value);
        }
    }

    public void UpdateLeaderboard(string id, int value)
    {
        if (OnUpdateLeaderboard != null)
        {
            OnUpdateLeaderboard(id, value);
        }
    }

    public void StartChallenge(Challenge challenge)
    {
        if (OnStartChallenge != null)
        {
            OnStartChallenge(challenge);
        }
    }

    public void SignOut()
    {
        if (OnSignOut != null)
        {
            OnSignOut();
        }
    }

    public void PurchaseProduct(string id)
    {
        if (OnPurchaseProduct != null)
        {
            OnPurchaseProduct(id);
        }
    }

    public void ProductPurchased(PurchaseEventArgs args)
    {
        if (OnProductPurchased != null)
        {
            OnProductPurchased(args);
        }
    }

    public void GameStarted()
    {
        if (OnGameStart != null)
        {
            OnGameStart();
        }
    }

    public void GameCompleted(int score)
    {
        if (OnGameComplete != null)
        {
            OnGameComplete(score);
        }
    }
}

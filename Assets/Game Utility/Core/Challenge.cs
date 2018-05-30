using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;

public abstract class Challenge : MonoBehaviour, IPointerClickHandler
{
    public string achievementId, Description;
    IAchievement achievement;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!achievement.completed)
        {
            GameUtility.Instance.StartChallenge(this);
            ListenToEvents();
        }
    }

    void Awake()
    {
        GameUtility.Instance.OnGotAchievements += Init;
    }

    public void Init(IAchievement[] achievements)
    {
        foreach (var item in achievements)
        {
            if (item.id == achievementId)
            {
                if (item.completed)
                {
                    achievement = item;
                    UIChanges();
                }
            }
        }
    }

    public void OnCompleted()
    {
        GameUtility.Instance.UpdateAchievement(achievementId, 100);
        Completed();
        UIChanges();
    }

    void UIChanges()
    {

    }

    public abstract void Completed();

    public abstract void ListenToEvents();

    public abstract event ChallengeDelegate Complete;
}
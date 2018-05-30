using UnityEngine;
using UnityEngine.SocialPlatforms;

public abstract class Unlockable : MonoBehaviour
{
    public string id;
    public string Description;
    int completed;
    public int target;

    private void Start()
    {
        GameUtility.Instance.OnGotAchievements += Init;
        ListenToEvents();
    }

    public void Init(IAchievement[] achievement)
    {
        foreach (var item in achievement)
        {
            if (item.id == id)
            {
                completed = int.Parse((item.percentCompleted / 10).ToString());
            }
        }
    }

    public abstract void ListenToEvents();

    public abstract event UnlockableDelegate Complete;
}
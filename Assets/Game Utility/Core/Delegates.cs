using UnityEngine.Purchasing;
using UnityEngine.SocialPlatforms;

public delegate void ParameterlessDelegate();
public delegate void IAchievementDelegate(IAchievement[] achievements);
public delegate void AchievementDelegate(string id, int value);
public delegate void ChallengeDelegate(Challenge challenge);
public delegate void UnlockableDelegate(Unlockable unlockable);
public delegate void StringDelegate(string value);
public delegate void PurchaseArgsDelegate(PurchaseEventArgs args);
public delegate void IntDelegate(int value);
public class PickupChallenge : Challenge
{
    public Powerup targetPower;

    public int target, current;

    public override event ChallengeDelegate Complete;

    public override void Completed()
    {
        Complete(this);
    }

    public override void ListenToEvents()
    {
        throw new System.NotImplementedException();
    }

    void PickedUp(Powerup powerup)
    {
        if (powerup == targetPower || targetPower == Powerup.any)
        {
            current++;
            if (current >= target)
            {
                OnCompleted();
            }
        }
    }
}

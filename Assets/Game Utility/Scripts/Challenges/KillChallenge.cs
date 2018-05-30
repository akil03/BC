public class KillChallenge : Challenge
{
    public KillReason targetKillReason;
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

    void Kill(Snake snake, KillReason reason)
    {
        current++;
        if (reason == targetKillReason || targetKillReason == KillReason.any)
        {
            if (current >= target)
            {
                OnCompleted();
            }
        }
    }
}

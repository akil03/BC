public class AreaCoverChallenge : Challenge
{
    public float target, covered;

    public override event ChallengeDelegate Complete;

    public override void Completed()
    {
        Complete(this);
    }

    public override void ListenToEvents()
    {
        throw new System.NotImplementedException();
    }

    void AreaCovered(float areaCovered)
    {
        covered += areaCovered;
        if (covered >= target)
        {
            OnCompleted();
        }
    }
}

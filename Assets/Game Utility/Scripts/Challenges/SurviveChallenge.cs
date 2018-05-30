using System.Collections;
using UnityEngine;

public class SurviveChallenge : Challenge
{
    public int target, current;
    bool isCounting;

    public override event ChallengeDelegate Complete;

    void Start()
    {
        StartCoroutine(StartCounting());
    }

    public override void ListenToEvents()
    {
        throw new System.NotImplementedException();
    }

    void KillPlayer(Snake snake, KillReason reason)
    {
        if (!snake.isBot)
        {
            isCounting = false;
            StopCoroutine(StartCounting());
        }
    }

    IEnumerator StartCounting()
    {
        current = 0;
        while (isCounting)
        {
            yield return new WaitForSeconds(1);
            current++;
            if (current >= target)
            {
                OnCompleted();
            }
        }
    }

    public override void Completed()
    {
        Complete(this);
    }
}

using System.Collections;
using UnityEngine;

public class NumberOneChallenge : Challenge
{
    public override event ChallengeDelegate Complete;

    public int target, current;
    bool alreadyCounting;

    public override void ListenToEvents()
    {
        throw new System.NotImplementedException();
    }

    void BecameNumberOne(Snake snake)
    {
        if (!snake.isBot)
        {
            if (!alreadyCounting)
            {
                alreadyCounting = true;
                StartCoroutine(StartCounting());
            }
        }
        else
        {
            alreadyCounting = false;
            StopCoroutine(StartCounting());
        }
    }

    IEnumerator StartCounting()
    {
        while (alreadyCounting)
        {
            current = 0;
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

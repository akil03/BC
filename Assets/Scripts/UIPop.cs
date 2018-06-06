using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIPop : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnSmash()
	{
        GetComponent<SpriteRenderer>().DOFade(1, 0);
        transform.DOLocalMoveY(-400, 0);

        transform.DOLocalMoveY(0, 1f);

        GetComponent<SpriteRenderer>().DOFade(0, 1f);


    }


    public void OnScore(int score)
    {
        GetComponent<Text>().DOFade(1, 0);
        transform.DOLocalMoveY(-400, 0);
        GetComponent<Text>().text = "+" + score.ToString();
        transform.DOLocalMoveY(0, 1f);
        GetComponent<Text>().DOFade(0, 1f);
    }

    public void OnTextSmash()
    {
        GetComponent<Text>().DOFade(1, 0);
        transform.DOLocalMoveY(-400, 0);
        GetComponent<Text>().text = "Smash !!";
        transform.DOLocalMoveY(0, 1f);
        GetComponent<Text>().DOFade(0, 1f);
    }


}

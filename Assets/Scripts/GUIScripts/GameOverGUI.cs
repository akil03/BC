using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using GameAnalyticsSDK;
#if UNITY_MOBILE
using UnityEngine.Advertisements;
#endif

public class GameOverGUI : MonoBehaviour {

	public Text scoreText;
	public Text highScoreText;
	public Text diamondText;


	public Text coinText;

	public Button GetCoinButton;

    public Image slider;
    public Transform Lost, score, highscore,continuebtn, playbtn;

	// Use this for initialization
	void Start () {
	
	}


	private void OnEnable()
	{

        OnDisable();
        Lost.DOScale(Vector3.one, 0.5f);
        score.DOScale(Vector3.one, 0.75f);
        highscore.DOScale(Vector3.one, 0.75f);
        continuebtn.DOScale(Vector3.one * 1.3f, 1f).OnComplete(() =>
          {
              slider.DOFillAmount(1, 5);
              playbtn.DOScale(Vector3.one, .5f);
          });

        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "game", int.Parse(scoreText.text));

    }

    private void OnDisable()
    {
        Lost.DOScale(Vector3.zero, 0);
        score.DOScale(Vector3.zero, 0);
        highscore.DOScale(Vector3.zero, 0);
        continuebtn.DOScale(Vector3.zero, 0);
        playbtn.DOScale(Vector3.zero, 0);
        slider.DOFillAmount(0, 0);
    }

    // Update is called once per frame
    void Update () {

		//string scoreString = ObliusGameManager.instance.TrimPercentage(((100 * (float)ScoreHandler.instance.score) / (float)GroundSpawner.instance.spawnedGroundPieces.Count).ToString());
		//string highscoreString = ObliusGameManager.instance.TrimPercentage(((100 * (float)ScoreHandler.instance.highScore) / (float)GroundSpawner.instance.spawnedGroundPieces.Count).ToString());

		//scoreText.text = "" + scoreString + "%";
		//highScoreText.text = "" + highscoreString + "%";


		scoreText.text = ScoreHandler.instance.score.ToString();
		highScoreText.text = ScoreHandler.instance.highScore.ToString();

		//diamondText.text = "" + ScoreHandler.instance.specialPoints;

	}


	public void OnGetCoinButtonClick()
	{
		SoundsManager.instance.PlayMenuButtonSound();

		#if UNITY_MOBILE

		UnityRewardAds.instance.ShowRewardedAd(HandleShowResult);

		#endif

		GetCoinButton.interactable = false;
	}

	public void OnBallShopClick()
	{
		SoundsManager.instance.PlayMenuButtonSound();

		Deactivate();
		GUIManager.instance.ShowShopGUI();
	}

	public void OnRemoveAdsButtonClick()
	{
		SoundsManager.instance.PlayMenuButtonSound();

//        AdRemover.instance.BuyNonConsumable();
	}

	public void OnRestorePurchaseButtonClick()
	{
		SoundsManager.instance.PlayMenuButtonSound();

	  //  AdRemover.instance.RestorePurchases();
	}

	public void OnLeaderboardButtonClick()
	{
		SoundsManager.instance.PlayMenuButtonSound();

		//Leaderboard.instance.showLeaderboard();
	}

	public void OnShareButtonClick()
	{
		SoundsManager.instance.PlayMenuButtonSound();

	 //   ShareManager.instance.share();
	}

	public void OnPlayButtonClick()
	{
		Deactivate ();
		GUIManager.instance.ShowMainMenuGUI ();			
	}

	public void Deactivate()
	{
		gameObject.SetActive(false);
	}

	#if UNITY_MOBILE
	private void HandleShowResult(ShowResult result)
	{
		switch (result)
		{
			case ShowResult.Finished:
				ScoreHandler.instance.increaseSpecialPoints(UnityRewardAds.instance.GetCoinsToRewardOnVideoWatched());
				//
				// YOUR CODE TO REWARD THE GAMER
				// Give coins etc.
				break;
			case ShowResult.Skipped:
				Debug.Log("The ad was skipped before reaching the end.");
				break;
			case ShowResult.Failed:
				Debug.LogError("The ad failed to be shown.");
				break;
		}
	}
	#endif


}

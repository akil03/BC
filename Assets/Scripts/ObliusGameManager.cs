using UnityEngine;
using System.Collections;
using DG.Tweening;
public class ObliusGameManager : MonoBehaviour
{
    public Color[] floorColour, skyColour;
    public int startColor, currentColor;
    public static ObliusGameManager instance;

	public int percentageCharLimit = 4;
    public SpriteRenderer floor;
	public enum GameState
	{
		menu,
		game,
		gameover,
		shop

	}

	public GameState gameState;
	public bool oneMoreChanceUsed = false;


    public GameObject VehiclAnimator, CameraContainer;

    public AudioClip gamestartClip, cardropClip;

	void Awake ()
	{
		Application.targetFrameRate = 60;
		instance = this;
	}
	// Use this for initialization
	void Start ()
	{
       
	}

	// Update is called once per frame
	void Update ()
	{

	}

	public IEnumerator GameOverCoroutine (float delay)
	{

		yield return new WaitForSeconds (delay);
		SoundsManager.instance.PlayGameOverSound ();
		//AdNetworks.instance.HideBanner ();
		gameState = GameState.gameover;

	//	Leaderboard.instance.reportScore (ScoreHandler.instance.score);
		GUIManager.instance.ShowGameOverGUI ();
		InGameGUI.instance.gameObject.SetActive (false);
		//AdNetworks.instance.ShowInterstitial ();


	}





	public void GameOver (float delay)
	{
		StartCoroutine (GameOverCoroutine (delay));
	}

	public void StartGame ()
	{
        StartCoroutine(FadeGame());
        return;

		ResetGame ();
		ScoreHandler.instance.incrementNumberOfGames ();
		GUIManager.instance.ShowInGameGUI ();
        //GUIManager.instance.tutorialGUI.ShowIfNeverAppeared();
        //AdNetworks.instance.ShowBanner ();


        //GroundSpawner.instance.ClearGround();
        //SnakesSpawner.instance.KillAllSnakes();

		SnakesSpawner.instance.SpawnPlayer ();
        //SnakesSpawner.instance.SpawnEnemies();

		gameState = GameState.game;



	}


    public void SwitchColors()
    {
        floor.DOColor(floorColour[currentColor], 1);
        GUIManager.instance.gameCam.DOColor(floorColour[currentColor], 1);
        currentColor++;
        if (currentColor > floorColour.Length - 1)
            currentColor = 0;

        Invoke("SwitchColors",8);

    }

    public void StartActualGame()
    {
        ResetGame();
        ScoreHandler.instance.incrementNumberOfGames();
        GUIManager.instance.ShowInGameGUI();
        //GUIManager.instance.tutorialGUI.ShowIfNeverAppeared();
        //AdNetworks.instance.ShowBanner ();


        //GroundSpawner.instance.ClearGround();
        //SnakesSpawner.instance.KillAllSnakes();

        SnakesSpawner.instance.SpawnPlayer();
        //SnakesSpawner.instance.SpawnEnemies();

        gameState = GameState.game;

        currentColor = startColor;
        CancelInvoke();
        SwitchColors();

    }


    IEnumerator FadeGame()
    {
        SoundsManager.instance.Play(gamestartClip);
        GUIManager.instance.FadeBlack.DOFade(1, 0.3f);
        
        VehiclAnimator.transform.DOMoveZ(-20, 0, true);
        yield return new WaitForSeconds(0.3f);

        GUIManager.instance.mainMenuGUI.gameObject.SetActive(false);
        CameraContainer.SetActive(false);
        VehiclAnimator.SetActive(true);
        VehiclAnimator.transform.DOMoveZ(-10, 5, false);

        GUIManager.instance.FadeBlack.DOFade(0, 1.3f);
        yield return new WaitForSeconds(2.5f);
        GUIManager.instance.FadeBlack.DOFade(1, 1f);
        yield return new WaitForSeconds(1.3f);
        GUIManager.instance.FadeBlack.DOFade(0, 0.1f);
        VehiclAnimator.SetActive(false);
        CameraContainer.SetActive(true);
        SoundsManager.instance.Play(cardropClip);
        StartActualGame();
        
    }


	public void ResetGame (bool resetScore = true, bool resetOneMoreChance = true)
	{
		if (resetOneMoreChance) {
			oneMoreChanceUsed = false;
		}

		if (resetScore) {
			ScoreHandler.instance.reset ();
		}
	}

	public string TrimPercentage(string str)
	{
		if (str.Length >= 8) {
			return str.Remove (percentageCharLimit);
		} else {
			return str;
		}
	}

}

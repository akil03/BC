using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuGUI : MonoBehaviour
{

    public static MainMenuGUI instance;

    public Text highscoreText;
    public Text gamesPlayedText;
    public Text specialPointsText;
	public InputField playerNameField;

    string originalHighScoreText;
    string originalGamesPlayedText;

	public GameObject loadingScreen;

    void Awake()
    {

        instance = this;
        originalHighScoreText = highscoreText.text;
        originalGamesPlayedText = gamesPlayedText.text;
    }

    // Use this for initialization
    void Start()
    {
        specialPointsText.text = ScoreHandler.instance.specialPoints.ToString();
    }

    // Update is called once per frame
    void Update()
    {
		string highscoreString = ObliusGameManager.instance.TrimPercentage(((100 * (float)ScoreHandler.instance.highScore) / (float)GroundSpawner.instance.spawnedGroundPieces.Count).ToString());
		highscoreText.text = originalHighScoreText + highscoreString + "%";
        gamesPlayedText.text = originalGamesPlayedText + ScoreHandler.instance.numberOfGames;
    }

    public void OnShopButtonClick()
    {
        SoundsManager.instance.PlayMenuButtonSound();

        gameObject.SetActive(false);
        GUIManager.instance.ShowShopGUI();
    }

    public void OnPlayButtonClick()
    {

        SoundsManager.instance.PlayMenuButtonSound();

        ObliusGameManager.instance.StartGame();
        //gameObject.SetActive(false);

    }

    public void OnRateButtonClick()
    {
        SoundsManager.instance.PlayMenuButtonSound();

        RateManager.instance.rateGame();
    }



}

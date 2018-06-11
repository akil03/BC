using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
public class InGameGUI : MonoBehaviour
{

    public static InGameGUI instance;
    public Text scoreText;
    public Text specialPointsText;
	public Text highscoreText;
	public Image specialPointsImage;
    public Transform fillTransform;

	public GameObject takenGUIDiamond;


    void Awake()
    {
        instance = this;
        
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
		UpdateScoreText ();
		UpdateSpecialPointsText();
		UpdateHighScoreText();
    }

    float currentFillScale;
	public void UpdateScoreText(){
        float coveredArea = 100*(float)SnakesSpawner.instance.playerSnake.ownedGroundPieces.Count / (float)GroundSpawner.instance.spawnedGroundPieces.Count;

        float fillScale = coveredArea / ObliusGameManager.instance.LevelTargets[ObliusGameManager.instance.currentLevel];

        if (currentFillScale != fillScale)
        {
            currentFillScale = fillScale;

            fillTransform.DOScaleX(currentFillScale, 0.5f);

            if (currentFillScale > 1)
                ObliusGameManager.instance.SwitchColors();
        }

        string scoreString = coveredArea.ToString("0.00");

        scoreText.text = scoreString +  " / "+ObliusGameManager.instance.LevelTargets[ObliusGameManager.instance.currentLevel] + " %";
        
    }

	public void UpdateSpecialPointsText(){
		specialPointsText.text = ScoreHandler.instance.specialPoints.ToString();
	}

	public void UpdateHighScoreText(){
		string highscoreString =  ObliusGameManager.instance.TrimPercentage (((100 * (float)ScoreHandler.instance.highScore) / (float)GroundSpawner.instance.spawnedGroundPieces.Count).ToString());
		highscoreText.text = "BEST SCORE: " + highscoreString + "%";
	}

    public void OnTutorialButtonClick()
    {
        SoundsManager.instance.PlayMenuButtonSound();
        GUIManager.instance.ShowTutorialGUI();
    }

    public void OnPauseButtonClick()
    {
        SoundsManager.instance.PlayMenuButtonSound();
        GUIManager.instance.ShowPauseGUI();
    }


	public void InstantiateTakeGUIDiamond(Vector3 pos){
		GameObject newObj = (GameObject)Instantiate (takenGUIDiamond);
		newObj.transform.SetParent(transform,false);
		TakenGUIDiamond newDiamond = newObj.GetComponent<TakenGUIDiamond> ();
		newDiamond.rect.position = pos;	
	}

}

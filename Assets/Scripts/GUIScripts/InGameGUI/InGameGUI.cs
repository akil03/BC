﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InGameGUI : MonoBehaviour
{

    public static InGameGUI instance;
    public Text scoreText;
    public Text specialPointsText;
	public Text highscoreText;
	public Image specialPointsImage;
    

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

	public void UpdateScoreText(){
		string scoreString = ObliusGameManager.instance.TrimPercentage (((100 * (float)ScoreHandler.instance.score) / (float)GroundSpawner.instance.spawnedGroundPieces.Count).ToString ());
		scoreText.text = "" + scoreString +  "%";
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

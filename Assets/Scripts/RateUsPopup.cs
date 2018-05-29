using UnityEngine;
using System.Collections;

public class RateUsPopup : MonoBehaviour
{

	public static RateUsPopup instance;


	public string appleID;
	public string androidAppURL;

	string totalSecondsElapsedPrefString = "POPUPSECONDS";


	string playerPrefsString = "RATEPOPUPPREF";
	string ratedValueString = "RATED";
	string remindValueString = "REMIND";
	string declinedValueString = "DECLINE";

	public int showEveryXSeconds;
	public int totalSecondsElapsed;


	bool showing;
	bool shown;


	void Awake ()
	{
		if (instance != null) {
			Destroy (gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad (gameObject);
	}



	// Use this for initialization
	void Start ()
	{
		StartCoroutine (PopUpShowRoutine ());
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}


	public IEnumerator PopUpShowRoutine ()
	{
		int cyclePauseSeconds = 1;
		totalSecondsElapsed = GetSecondsElapsed ();

		while (GetRateStatusFromPrefs () == remindValueString) {

			while (totalSecondsElapsed < showEveryXSeconds) {
				yield return new WaitForSeconds (cyclePauseSeconds);
				totalSecondsElapsed += cyclePauseSeconds;
				SaveSecondsElapsed (totalSecondsElapsed);
			}

			if (GetRateStatusFromPrefs () == remindValueString && !showing) {

				while (ObliusGameManager.instance.gameState == ObliusGameManager.GameState.game) {
					yield return new WaitForEndOfFrame ();
				}
				Show ();
				ResetTotalSecondsElapsed ();
			}
			yield return new WaitForEndOfFrame ();
		}
	}


	void Show ()
	{
		MobileNativeRateUs ratePopUp = new MobileNativeRateUs ("Like this game?", "Please rate to support future updates!");
		ratePopUp.SetAppleId (appleID);
		ratePopUp.SetAndroidAppUrl (androidAppURL);
		ratePopUp.OnComplete += OnRatePopUpClose;
		ratePopUp.Start ();
		showing = true;
	}


	private void OnRatePopUpClose (MNDialogResult result)
	{
		//parsing result
		switch (result) {
		case MNDialogResult.RATED:
			PlayerPrefs.SetString (playerPrefsString, ratedValueString);
			showing = false;
			break;
		case MNDialogResult.REMIND:
			PlayerPrefs.SetString (playerPrefsString, remindValueString);
			showing = false;
			break;
		case MNDialogResult.DECLINED:
			PlayerPrefs.SetString (playerPrefsString, declinedValueString);
			showing = false;
			break;
		}
	}

	private string GetRateStatusFromPrefs ()
	{
		return PlayerPrefs.GetString (playerPrefsString, remindValueString);
	}



	public int GetSecondsElapsed ()
	{
		return PlayerPrefs.GetInt (totalSecondsElapsedPrefString, 0);
	}

	public void SaveSecondsElapsed (int value)
	{
		PlayerPrefs.SetInt (totalSecondsElapsedPrefString, value);
	}

	public void ResetTotalSecondsElapsed ()
	{
		totalSecondsElapsed = 0;
		SaveSecondsElapsed (totalSecondsElapsed);
	}



}

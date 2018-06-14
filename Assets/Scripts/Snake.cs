using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Snake : MonoBehaviour
{

	public float speed = 1f;
	public Vector3 originalPos;

	[HideInInspector]
	public Vector3 nextMoveDirection;
	public Vector3 currentMoveDirection;
	public Vector3 previousMoveDirection;

	GroundPiece groundPieceToReach;
	public GroundPiece lastReachedGroundPiece;

	BoxCollider2D boxCollider;

	public List<GroundPiece> ownedGroundPieces;

	[HideInInspector]
	public bool isCollectingNewGroundPieces;
	public List<GroundPiece> tailGroundPieces;

	public bool isBot,isDead;
	public SnakeAI AI;

	public bool haveToDie;

	public SnakeMeshContainer snakeMeshContainer;

	public SnakeMeshProprietes snakeMeshProprietes;
	public Sprite tailPieceSprite;
	public Sprite collectedPieceSprite;
	public SpriteRenderer FollowSprite;

	public SnakeNameTextMesh snakeNameTextMesh;
	public string name;

	public Color spriteColor;
	public int scoreMultiplier = 1;
	public int invertControls = 1;
	public bool isShielded;

	public GameObject destroyParticle;

	public AudioSource movementSound;

	public AudioClip deadClip, powerupClip, gemsClip, speedClip, shieldClip, scoreClip, blackoutClip, slowClip, fillClip;

	void Awake ()
	{
		AI = GetComponent<SnakeAI> ();
		boxCollider = GetComponent<BoxCollider2D> ();
		snakeMeshContainer = GetComponentInChildren<SnakeMeshContainer> ();
		ownedGroundPieces = new List<GroundPiece> ();

				
	}


	public IEnumerator StartMove ()
	{
		nextMoveDirection = transform.up;
		currentMoveDirection = nextMoveDirection;
		previousMoveDirection = currentMoveDirection;
		yield return snakeMeshContainer.DOSpawnAnimation ();
		StartCoroutine (MoveToTurningPoint ());
	}

	public void Initialize ()
	{
		
		snakeMeshProprietes = GetComponentInChildren<SnakeMeshProprietes> ();
		//collectedPieceSprite = snakeMeshProprietes.collectedPiece;
		//tailPieceSprite = snakeMeshProprietes.tailPiece;

		snakeMeshProprietes.collectedPiece = collectedPieceSprite;
		snakeMeshProprietes.tailPiece = tailPieceSprite;

	   


		snakeMeshProprietes.snakeColor = spriteColor;
		FollowSprite.color = spriteColor;
		FollowSprite.DOFade(0.5f, 0);

		SetName ();
		if (!isBot) {
			movementSound.Play();
			GUIManager.instance.gameCam.fieldOfView = 60;
			GUIManager.instance.scoreText.text = "0";
		}
		
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
		if (!isBot) {	

			if(Input.GetKeyDown(KeyCode.DownArrow)){
				SwipeHandler.instance.lastSwipeDirection = SwipeHandler.SwipeDirection.down;
			
			}

			if(Input.GetKeyDown(KeyCode.UpArrow)){
				SwipeHandler.instance.lastSwipeDirection = SwipeHandler.SwipeDirection.up;
			}

			if(Input.GetKeyDown(KeyCode.LeftArrow)){
				SwipeHandler.instance.lastSwipeDirection = SwipeHandler.SwipeDirection.left;
			}

			if(Input.GetKeyDown(KeyCode.RightArrow)){
				SwipeHandler.instance.lastSwipeDirection = SwipeHandler.SwipeDirection.right;
			}


			if (SwipeHandler.instance.lastSwipeDirection == SwipeHandler.SwipeDirection.up) {
				//if (nextMoveDirection != -transform.up)
					MoveToDirection (transform.up* invertControls);
			}

			if (SwipeHandler.instance.lastSwipeDirection == SwipeHandler.SwipeDirection.down) {
				//if (nextMoveDirection != transform.up)
					MoveToDirection (-transform.up * invertControls);
			}

			if (SwipeHandler.instance.lastSwipeDirection == SwipeHandler.SwipeDirection.left) {
			//	if (nextMoveDirection != transform.right)
					MoveToDirection (-transform.right * invertControls);
			}

			if (SwipeHandler.instance.lastSwipeDirection == SwipeHandler.SwipeDirection.right) {
			//	if (nextMoveDirection != -transform.right)
					MoveToDirection (transform.right * invertControls);
			}
		} 
	}


	public void MoveToDirection (Vector3 vector)
	{
		nextMoveDirection = vector;
	}


	public IEnumerator MoveToTurningPoint ()
	{
		while (true) {

			if (lastReachedGroundPiece.IsBoundPiece ()) {
				haveToDie = true;
			}

			if (haveToDie) {
				StartCoroutine (Die ());
				break;
			}

			if (currentMoveDirection == -nextMoveDirection) {
				nextMoveDirection = currentMoveDirection;
			}

			groundPieceToReach = GetNewGroundPieceToReach ();

			previousMoveDirection = currentMoveDirection;
			currentMoveDirection = nextMoveDirection;

			Vector3 targetPos = groundPieceToReach.transform.position;
			targetPos.z = transform.position.z;

			while (transform.position != targetPos) {	
				transform.position = Vector3.MoveTowards (transform.position, targetPos, speed * Time.deltaTime);
				yield return new WaitForEndOfFrame ();
			}

			CheckReachedGroundPiece (groundPieceToReach);

			if (isBot) {			
				AI.Notify (groundPieceToReach, isCollectingNewGroundPieces);
			}

			lastReachedGroundPiece = groundPieceToReach;

			yield return new WaitForEndOfFrame ();
		}
	}
	int totalScore = 0;
	int scoreCount;
	float fov;
	public void CheckReachedGroundPiece (GroundPiece pieceToCheck)
	{
		if (pieceToCheck.collectingSnake != null) {		
			KillSnake (pieceToCheck.collectingSnake);
		}

		if (pieceToCheck.snakeOwener != this) {			

			if (isCollectingNewGroundPieces == false) {
				isCollectingNewGroundPieces = true;
				AI.Reset ();
			} 
			pieceToCheck.SetCollectingSnake (this);

		} else {

			if (isCollectingNewGroundPieces) {

				List<GroundPiece> newOwnedGroundPieces = new List<GroundPiece> ();

				foreach (GroundPiece groundPiece in tailGroundPieces) {
					newOwnedGroundPieces.Add (groundPiece);
					groundPiece.SetSnakeOwner (this);
				}

				GroundPiece[] groundPiecesToCheck = Poly.GetGroundPiecesToCheck (this);
			
				foreach (GroundPiece piece in groundPiecesToCheck) {				
					piece.tempHasToBeChecked = true;

					if (piece.snakeOwener != this) {
						piece.ownerIDForCheck = 1;
					} else {
						piece.ownerIDForCheck = 0;
					}
				}

				Poly.FloodFill (groundPiecesToCheck [0], 1, 2);

				if(!isBot)
					SoundsManager.instance.Play(fillClip);
				

				foreach (GroundPiece piece in groundPiecesToCheck) {

					if (piece.ownerIDForCheck == 1) {
						newOwnedGroundPieces.Add (piece);
						piece.SetSnakeOwner (this);
					}

					piece.tempHasToBeChecked = false;
				}
				scoreCount = 0;

				foreach (GroundPiece piece in newOwnedGroundPieces) {
					piece.pieceWhenCollected.sr.color = spriteColor;
					piece.ShowCollectedPiece (collectedPieceSprite);
					scoreCount++;

				}
				scoreCount *= 50*scoreMultiplier;

				if (!isBot)
				{
					totalScore += scoreCount;
					fov = 60 + (0.02f * ownedGroundPieces.Count);
					fov = Mathf.Clamp(fov, 60, 80);
					GUIManager.instance.ScorePop.OnScore(scoreCount);
					GUIManager.instance.scoreText.text = totalScore.ToString();
					GUIManager.instance.gameCam.DOFieldOfView(fov, 1);
					ScoreHandler.instance.SetScore(totalScore);
				}
					

				if (!isBot&& !isDead) {
					//ScoreHandler.instance.SetScore (ownedGroundPieces.Count);
				}

				isCollectingNewGroundPieces = false;
				tailGroundPieces = new List<GroundPiece> ();
			}
		}



	}


	public GroundPiece GetNewGroundPieceToReach ()
	{

		GroundPiece piece = groundPieceToReach;

		try {

			if (nextMoveDirection == transform.up) {
				piece = lastReachedGroundPiece.column.groundPieces [lastReachedGroundPiece.indexInColumn - 1];
			}

			if (nextMoveDirection == -transform.up) {
				piece = lastReachedGroundPiece.column.groundPieces [lastReachedGroundPiece.indexInColumn + 1];
			}

			if (nextMoveDirection == transform.right) {
				piece = lastReachedGroundPiece.row.groundPieces [lastReachedGroundPiece.indexInRow + 1];
			}

			if (nextMoveDirection == -transform.right) {
				piece = lastReachedGroundPiece.row.groundPieces [lastReachedGroundPiece.indexInRow - 1];
			}

			return piece;

		} catch {
			return groundPieceToReach;
		}		
	}





	public void SetFirstOwnedGroundPieces (GroundPiece spawnPoint)
	{

		List<GroundPiece> newOwnedGroundPieces = new List<GroundPiece> ();


		newOwnedGroundPieces.Add (GroundSpawner.instance.columns [spawnPoint.column.indexInColumnsList].groundPieces [spawnPoint.indexInColumn]);
		newOwnedGroundPieces.Add (GroundSpawner.instance.columns [spawnPoint.column.indexInColumnsList].groundPieces [spawnPoint.indexInColumn - 1]);
		newOwnedGroundPieces.Add (GroundSpawner.instance.columns [spawnPoint.column.indexInColumnsList].groundPieces [spawnPoint.indexInColumn + 1]);
		newOwnedGroundPieces.Add(GroundSpawner.instance.columns[spawnPoint.column.indexInColumnsList].groundPieces[spawnPoint.indexInColumn - 2]);   //extra
		newOwnedGroundPieces.Add(GroundSpawner.instance.columns[spawnPoint.column.indexInColumnsList].groundPieces[spawnPoint.indexInColumn + 2]);   //extra


		newOwnedGroundPieces.Add (GroundSpawner.instance.columns [spawnPoint.column.indexInColumnsList - 1].groundPieces [spawnPoint.indexInColumn]);
		newOwnedGroundPieces.Add (GroundSpawner.instance.columns [spawnPoint.column.indexInColumnsList - 1].groundPieces [spawnPoint.indexInColumn - 1]);
		newOwnedGroundPieces.Add (GroundSpawner.instance.columns [spawnPoint.column.indexInColumnsList - 1].groundPieces [spawnPoint.indexInColumn + 1]);
		newOwnedGroundPieces.Add(GroundSpawner.instance.columns[spawnPoint.column.indexInColumnsList - 1].groundPieces[spawnPoint.indexInColumn - 2]);   //extra
		newOwnedGroundPieces.Add(GroundSpawner.instance.columns[spawnPoint.column.indexInColumnsList - 1].groundPieces[spawnPoint.indexInColumn + 2]);   //extra

		newOwnedGroundPieces.Add (GroundSpawner.instance.columns [spawnPoint.column.indexInColumnsList + 1].groundPieces [spawnPoint.indexInColumn]);
		newOwnedGroundPieces.Add (GroundSpawner.instance.columns [spawnPoint.column.indexInColumnsList + 1].groundPieces [spawnPoint.indexInColumn - 1]);
		newOwnedGroundPieces.Add (GroundSpawner.instance.columns [spawnPoint.column.indexInColumnsList + 1].groundPieces [spawnPoint.indexInColumn + 1]);
		newOwnedGroundPieces.Add(GroundSpawner.instance.columns[spawnPoint.column.indexInColumnsList + 1].groundPieces[spawnPoint.indexInColumn - 2]);   //extra
		newOwnedGroundPieces.Add(GroundSpawner.instance.columns[spawnPoint.column.indexInColumnsList + 1].groundPieces[spawnPoint.indexInColumn + 2]);   //extra

		newOwnedGroundPieces.Add(GroundSpawner.instance.columns[spawnPoint.column.indexInColumnsList - 2].groundPieces[spawnPoint.indexInColumn]);   //extra
		newOwnedGroundPieces.Add(GroundSpawner.instance.columns[spawnPoint.column.indexInColumnsList - 2].groundPieces[spawnPoint.indexInColumn - 1]);   //extra
		newOwnedGroundPieces.Add(GroundSpawner.instance.columns[spawnPoint.column.indexInColumnsList - 2].groundPieces[spawnPoint.indexInColumn + 1]);   //extra
		newOwnedGroundPieces.Add(GroundSpawner.instance.columns[spawnPoint.column.indexInColumnsList - 2].groundPieces[spawnPoint.indexInColumn - 2]);   //extra
		newOwnedGroundPieces.Add(GroundSpawner.instance.columns[spawnPoint.column.indexInColumnsList - 2].groundPieces[spawnPoint.indexInColumn + 2]);   //extra

		newOwnedGroundPieces.Add(GroundSpawner.instance.columns[spawnPoint.column.indexInColumnsList + 2].groundPieces[spawnPoint.indexInColumn]);   //extra
		newOwnedGroundPieces.Add(GroundSpawner.instance.columns[spawnPoint.column.indexInColumnsList + 2].groundPieces[spawnPoint.indexInColumn - 1]);   //extra
		newOwnedGroundPieces.Add(GroundSpawner.instance.columns[spawnPoint.column.indexInColumnsList + 2].groundPieces[spawnPoint.indexInColumn + 1]);   //extra
		newOwnedGroundPieces.Add(GroundSpawner.instance.columns[spawnPoint.column.indexInColumnsList + 2].groundPieces[spawnPoint.indexInColumn - 2]);   //extra
		newOwnedGroundPieces.Add(GroundSpawner.instance.columns[spawnPoint.column.indexInColumnsList + 2].groundPieces[spawnPoint.indexInColumn + 2]);   //extra


		foreach (GroundPiece piece in newOwnedGroundPieces) {
			piece.SetSnakeOwner (this);
		} 

		foreach (GroundPiece piece in newOwnedGroundPieces) {
			piece.pieceWhenCollected.sr.color = spriteColor;
			piece.ShowCollectedPiece (collectedPieceSprite);
		}
	}


	public Snake lastKill;
	public void KillSnake (Snake targetSnake)
	{
		if(!targetSnake.isShielded)
		   targetSnake.haveToDie = true;


		if (!isBot&&targetSnake!=this){
			if (lastKill != null)
				return;

			lastKill = targetSnake;
			
			GUIManager.instance.ScorePop.OnTextSmash();
			SoundsManager.instance.Play(deadClip);
			Handheld.Vibrate();
		}
	}


	public IEnumerator Die ()
	{

        //if (isShielded)
        //    yield break;
        isDead = true;

        GameObject DP = Instantiate(destroyParticle, transform);

        if (!isBot)
		{
			DisableBlackOut();
			DisableTimeSlow();
			GUIManager.instance.HidePowerText();
			SoundsManager.instance.Play(deadClip);
			Handheld.Vibrate();
		}

        if (!SnakesSpawner.instance.playerSnake)
        {
            DP.GetComponent<AudioSource>().volume = 0;
        }
        else if (Vector3.Distance(transform.position, SnakesSpawner.instance.playerSnake.transform.position) > 8)
            DP.GetComponent<AudioSource>().volume = 0;





        yield return  StartCoroutine (FadeOutTailPieces ());	
		yield return StartCoroutine (FadeOutCollectedGroundPieces ());

		foreach (GroundPiece piece in ownedGroundPieces) {
			piece.RemoveSnakeOwner (this);
		}

		foreach (GroundPiece piece in tailGroundPieces) {
			piece.RemoveCollectingSnake (this);
		}
					
		SnakesSpawner.instance.GetNotifiedSnakeDeath (this);

		if (!isBot) {
			ObliusGameManager.instance.GameOver (0);
		}

		SpawnDiamonds ();
		DestroyImmediate (gameObject);
		yield return null;
	}


	IEnumerator FadeOutTailPieces ()
	{

		tailGroundPieces.Reverse ();
		GroundPiece[] piecesToFade = tailGroundPieces.ToArray ();

		foreach (GroundPiece piece in piecesToFade) {
			if (piece.collectingSnake == this) {
				yield return StartCoroutine (piece.tailPiece.FadeOut ());
			}
		}

	}

	IEnumerator FadeOutCollectedGroundPieces ()
	{
		GroundPiece[] piecesToFade = ownedGroundPieces.ToArray ();

		foreach (GroundPiece piece in piecesToFade) {
			if (piece.snakeOwener == this) {
				StartCoroutine (piece.pieceWhenCollected.FadeOut ());
			}
		}
		yield return new WaitForSeconds (0.3f);
	}

	void OnTriggerEnter2D(Collider2D coll){


		if (coll.tag == "Diamond") {
			if (!isBot) {	
				GUIManager.instance.inGameGUI.InstantiateTakeGUIDiamond (Camera.main.WorldToScreenPoint (coll.gameObject.transform.position));		
				ScoreHandler.instance.increaseSpecialPoints (1);
				SoundsManager.instance.Play(gemsClip);
			}
				Destroy (coll.gameObject);

		}

		if (coll.tag == "Enemy")
		{
			if(!isShielded)
				StartCoroutine(Die());
			

		}


		if (coll.tag == "Random")
		{
			Destroy(coll.gameObject);
			SnakesSpawner.instance.spawnedPowerCount--;
			ActivateRandom();
		}

		if (coll.tag == "Speed")
		{
			Destroy(coll.gameObject);
			SnakesSpawner.instance.spawnedPowerCount--;
			ActivateSpeed();
		}

		if (coll.tag == "Shield")
		{
			Destroy(coll.gameObject);
			SnakesSpawner.instance.spawnedPowerCount--;
			ActivateShields();
		}

		if (coll.tag == "Score")
		{
			Destroy(coll.gameObject);
			SnakesSpawner.instance.spawnedPowerCount--;
			ActivateMultiplier();
		}

		if (coll.tag == "Slow")
		{
			Destroy(coll.gameObject);
			SnakesSpawner.instance.spawnedPowerCount--;
			ActivateTimeSlow();
		}

	}

	public void SpawnDiamonds(){

		int numberOfDiamonds = Random.Range (2, 4);
		GameObject diamondPrefab = (GameObject)Resources.Load ("Diamond");


		for (int i = 0; i <= numberOfDiamonds; i++) {
			
			GameObject diamond = (GameObject) Instantiate (diamondPrefab);
			diamond.transform.position = transform.position;
			diamond.transform.position +=(Vector3) Random.insideUnitCircle;

		}
	}

	public void SetName(){
		name = GetName ();
		snakeNameTextMesh = GetComponentInChildren<SnakeNameTextMesh> ();
		snakeNameTextMesh.SetColor (snakeMeshProprietes.snakeColor);
		snakeNameTextMesh.SetText (name);
	}


	public string GetName(){


		if (isBot) {
		return	NamesManager.instance.GetRandomName ();
		} else {

			if (GUIManager.instance.mainMenuGUI.playerNameField.text != "") {
				return GUIManager.instance.mainMenuGUI.playerNameField.text;
			} else {
				return "";
				return "guest" + Random.Range (0, 999999);
			}
		}

	}


	public void ActivateRandom()
	{
	   
		int rand = Random.Range(0, 7);

		switch (rand)
		{
			case 0:
				ActivateSpeed();
				break;
			case 1:
				ActivateMultiplier();
				break;
			case 2:
				ActivateShields();
				break;
			case 3:
				ActivateTimeSlow();
				break;
			case 4:
				OnCameraTilt();
				break;
			case 5:
				ActivateInvertControls();
				break;
			case 6:
				ActivateBlackOut();
				break;

		}
	}




	public int speedTime=4;
	public void ActivateSpeed()
	{

		if (!isBot)
		{
			GUIManager.instance.ShowPowerText("Accelerate !!");
			SoundsManager.instance.Play(powerupClip);
			SoundsManager.instance.Play(speedClip);
		}
			

		speed = 12;
		CancelInvoke("DisableSpeed");
		Invoke("DisableSpeed", speedTime);

	}

	public void DisableSpeed()
	{
		speed = 4.5f;
		GUIManager.instance.HidePowerText();
	}


	public int multiplierTime = 10;
	public void ActivateMultiplier()
	{
		if (isBot)
			return;

		SoundsManager.instance.Play(powerupClip);
		SoundsManager.instance.Play(scoreClip);

		GUIManager.instance.ShowPowerText("2x Score Multiplier !!");
				
		scoreMultiplier = scoreMultiplier * 2;
		CancelInvoke("DisableMultiplier");
		Invoke("DisableMultiplier", multiplierTime);

	}

	public void DisableMultiplier()
	{
		scoreMultiplier = scoreMultiplier / 2;
		GUIManager.instance.HidePowerText();
	}



	public int shieldTime = 5;
	public void ActivateShields()
	{
		if (!isBot)
		{
			SoundsManager.instance.Play(powerupClip);
			SoundsManager.instance.Play(shieldClip);
			GUIManager.instance.ShowPowerText("Shields !!");
		}
			
		
		isShielded = true;
		//snakeMeshProprietes.Shield.transform.DOScale(new Vector3(1.5f,1.5f,1.5f), 0.3f).SetEase(Ease.OutBack);
		snakeMeshProprietes.Shield.SetActive(true);

		CancelInvoke("DisableShields");
		Invoke("DisableShields", shieldTime);

	}

	public void DisableShields()
	{
		isShielded = false;
		//snakeMeshProprietes.Shield.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack);
		snakeMeshProprietes.Shield.SetActive(false);
		GUIManager.instance.HidePowerText();

	}

	public int timeSlow = 2;
	public void ActivateTimeSlow()
	{
		if (isBot)
			return;

		SoundsManager.instance.Play(powerupClip);
		SoundsManager.instance.Play(slowClip);

		GUIManager.instance.ShowPowerText("Time freeze !!");

		Time.timeScale = 0.5f;
		speed = speed*2;
		GUIManager.instance.FrozenVignette.DOFade(1, 0.7f);
		CancelInvoke("DisableTimeSlow");
		Invoke("DisableTimeSlow", timeSlow);

	}

	public void DisableTimeSlow()
	{
		Time.timeScale = 1f;
		speed = speed / 2;
		GUIManager.instance.HidePowerText();
		GUIManager.instance.FrozenVignette.DOFade(0, 0.6f);
	}



	public int InvertControlTime = 4;
	public void ActivateInvertControls()
	{
		if (isBot)
			return;


		GUIManager.instance.ShowPowerText("Invert controls !!");
		invertControls = -1;
		CancelInvoke("DisableInvertControls");
		Invoke("DisableInvertControls", InvertControlTime);

	}

	public void DisableInvertControls()
	{
		invertControls = 1;
		GUIManager.instance.HidePowerText();
	}

   
	public int BlackOutTime = 2;
	public void ActivateBlackOut()
	{
		if (isBot)
			return;


		SoundsManager.instance.Play(powerupClip);
		SoundsManager.instance.Play(blackoutClip);

		GUIManager.instance.ShowPowerText("Black out !!");
		GUIManager.instance.FadeBlack.DOFade(1, 0.3f);
		CancelInvoke("DisableBlackOut");
		Invoke("DisableBlackOut", BlackOutTime);

	}

	public void DisableBlackOut()
	{
		GUIManager.instance.FadeBlack.DOFade(0, 1.3f);
		GUIManager.instance.HidePowerText();
	}



	public void OnCameraTilt()
	{

		if (isBot)
			return;

		GUIManager.instance.ShowPowerText("Camera Tilt !!");
		GUIManager.instance.gameCam.transform.DOLocalRotate(new Vector3(-52, 0, -40), 1f, RotateMode.Fast).OnComplete(() =>
		{
			GUIManager.instance.gameCam.transform.DOLocalRotate(new Vector3(-52, 0, 40), 2f, RotateMode.Fast).OnComplete(() =>
			{
				GUIManager.instance.gameCam.transform.DOLocalRotate(new Vector3(-52, 0, 0), 1f, RotateMode.Fast);
				GUIManager.instance.HidePowerText();
			});
		});
	}

}

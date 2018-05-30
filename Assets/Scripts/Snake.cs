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

	public bool isBot;
	public SnakeAI AI;

	public bool haveToDie;

	public SnakeMeshContainer snakeMeshContainer;

	public SnakeMeshProprietes snakeMeshProprietes;
	public Sprite tailPieceSprite;
	public Sprite collectedPieceSprite;

	public SnakeNameTextMesh snakeNameTextMesh;
	public string name;

	public Color spriteColor;
	public int scoreMultiplier = 1;
	public int invertControls = 1;
	public bool isShielded;

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

		SetName ();
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

				foreach (GroundPiece piece in groundPiecesToCheck) {

					if (piece.ownerIDForCheck == 1) {
						newOwnedGroundPieces.Add (piece);
						piece.SetSnakeOwner (this);
					}

					piece.tempHasToBeChecked = false;
				}

				foreach (GroundPiece piece in newOwnedGroundPieces) {
					piece.pieceWhenCollected.sr.color = spriteColor;
					piece.ShowCollectedPiece (collectedPieceSprite);
				}	

				if (!isBot) {
					ScoreHandler.instance.SetScore (ownedGroundPieces.Count);
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



	public void KillSnake (Snake targetSnake)
	{
        if (!isShielded)
           targetSnake.haveToDie = true;
	}


	public IEnumerator Die ()
	{

		if (!isBot)
		{
			DisableBlackOut();
			DisableTimeSlow();
            GUIManager.instance.HidePowerText();
        }



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
				ActivateShields();
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
            GUIManager.instance.ShowPowerText("Accelerate !!");

        speed = 12;
		Invoke("DisableSpeed", speedTime);

	}

	public void DisableSpeed()
	{
        speed = 4;
        GUIManager.instance.HidePowerText();
	}


	public int multiplierTime = 10;
	public void ActivateMultiplier()
	{
		if (isBot)
			return;


        GUIManager.instance.ShowPowerText("2x Score Multiplier !!");
                
		scoreMultiplier = scoreMultiplier * 2;
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
            GUIManager.instance.ShowPowerText("Shields !!");
        
		isShielded = true;
		snakeMeshProprietes.Shield.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
		CancelInvoke("DisableShields");
		Invoke("DisableShields", shieldTime);

	}

	public void DisableShields()
	{
		isShielded = false;
		snakeMeshProprietes.Shield.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack);
        GUIManager.instance.HidePowerText();

    }

	public int timeSlow = 2;
	public void ActivateTimeSlow()
	{
		if (isBot)
			return;


        GUIManager.instance.ShowPowerText("Time freeze !!");

        Time.timeScale = 0.5f;
		CancelInvoke("DisableTimeSlow");
		Invoke("DisableTimeSlow", timeSlow);

	}

	public void DisableTimeSlow()
	{
		Time.timeScale = 1f;
        GUIManager.instance.HidePowerText();
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

}

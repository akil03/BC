using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakesSpawner : MonoBehaviour
{


	public static SnakesSpawner instance;

	public int maxEnemiesOnGround = 10;

    public int maxPowersOnGround = 10;

    public List<GameObject> randomSnakeMeshes;
	public List<GameObject> shoppableSnakeMeshes;
	public List<GameObject> usableSnakeMeshes;
    public List<Color> usableColors;

    public GameObject snakeMeshAssignedToPlayer;
    public List<GameObject> PowerupsList;

	public Snake playerSnake;

	public int spawnedEnemiesCount,spawnedPowerCount;
	public List<Snake> spawnedSnakes;




	void Awake ()
	{
		instance = this;	
		spawnedSnakes = new List<Snake> ();
	}

	// Use this for initialization
	void Start ()
	{
		LoadUsableMeshesFromResources ();
		//snakeMeshAssignedToPlayer = GetMeshUsedByPlayer ();
		StartCoroutine (SpawnRoutine ());
        StartCoroutine (SpawnPowers ());
        
	}

	public void SpawnPlayer ()
	{
		StartCoroutine (SpawnNewSnake (true));
	}

    public void KillAllSnakes()
    {
        //StopCoroutine(SpawnRoutine());
        foreach(Snake S in spawnedSnakes)
        {
            try
            {
                spawnedSnakes.Remove(S);
            }
            catch { }
            
            Destroy(S.gameObject);
        }
            
    }

    public void SpawnEnemies()
    {

        while (spawnedEnemiesCount < maxEnemiesOnGround)
        {
           StartCoroutine(SpawnNewSnake(false));
        }
        //StartCoroutine(SpawnRoutine());
    }

    public IEnumerator SpawnPowers()
    {
        while (true)
        {

            if (spawnedPowerCount < maxPowersOnGround)
            {
                yield return StartCoroutine(SpawnNewPowerup());
            }
            yield return new WaitForEndOfFrame();
        }
     
    }



	public IEnumerator SpawnRoutine ()
	{

		while (true) {

			if (spawnedEnemiesCount < maxEnemiesOnGround) {
				yield return StartCoroutine (SpawnNewSnake (false));					
			}
			yield return new WaitForEndOfFrame ();
		}

	}

	
	// Update is called once per frame
	void Update ()
	{
	


	}


	public IEnumerator SpawnNewSnake (bool isPlayer = false)
	{
		/*
		if (!isPlayer) {
			spawnedEnemiesCount++;
			yield return StartCoroutine (GetValidSpawnPoint ());
		} else {
			spawnPoint = GetRandomSpawnPoint ();
		}
		*/

		if (!isPlayer) {
			spawnedEnemiesCount++;
		}

		SpawnPointFinder spawnPointFinder = new SpawnPointFinder ();
		yield return StartCoroutine (spawnPointFinder.GetValidSpawnPoint ());

		GameObject go = (GameObject)Resources.Load ("Snake");
		go = GameObject.Instantiate (go);

		Snake newSnake = go.GetComponent<Snake> ();

		if (isPlayer) {
			newSnake.isBot = false;
			playerSnake = newSnake;
		} else {
			newSnake.isBot = true;
		}

		SetSnakeMesh (newSnake);


		newSnake.Initialize ();

		newSnake.lastReachedGroundPiece = spawnPointFinder.spawnPoint;
		newSnake.SetFirstOwnedGroundPieces (spawnPointFinder.spawnPoint);

		Vector3 newPos = spawnPointFinder.spawnPoint.transform.position;
		newPos.z = newSnake.transform.position.z;
		newSnake.transform.position = newPos;
		newSnake.originalPos = transform.position;

		newSnake.gameObject.name = newSnake.name;
		spawnedSnakes.Add (newSnake);
		StartCoroutine (newSnake.StartMove ());
	}

    public IEnumerator SpawnNewPowerup()
    {
        yield return null;

        int rand = Random.Range(0, PowerupsList.Count);

        GameObject go = PowerupsList[rand];

        go = GameObject.Instantiate(go);

        Vector3 newPos = new Vector3(Random.Range(1,32),Random.Range(-1,-32),-1);
        
        go.transform.position = newPos;

        spawnedPowerCount++;
        
    }



    public void SetPlayerSnakeColor ()
	{
		Snake snake = FindObjectOfType<Snake> ();
		SetSnakeMesh (snake);
	}

	public void  SetSnakeMesh (Snake snake)
	{	GameObject meshToUse = null;
		if (snake.isBot) {
			int random = Random.Range (0, usableSnakeMeshes.Count);
			meshToUse = usableSnakeMeshes [random];

		} else {
			meshToUse = snakeMeshAssignedToPlayer;
		}

        int rand = Random.Range(0, usableColors.Count);
        snake.spriteColor = usableColors[rand];
        snake.spriteColor = usableColors[rand];
        usableColors.RemoveAt(rand);


        snake.snakeMeshContainer.SetSnakeMesh (meshToUse);
		usableSnakeMeshes.Remove (meshToUse);
	}

	public void GetNotifiedSnakeDeath (Snake snake)
	{
		usableSnakeMeshes.Add (snake.snakeMeshContainer.snakeMesh);

        usableColors.Add(snake.spriteColor);


        spawnedSnakes.Remove (snake);
		if (snake.isBot) {
			spawnedEnemiesCount--;
		}
	}

	public void LoadUsableMeshesFromResources(){

		Object[] meshesInResources = Resources.LoadAll("SnakesModels/Random");

		foreach (Object obj in meshesInResources) {
			GameObject mesh = obj as GameObject;
			randomSnakeMeshes.Add (mesh);
			//usableSnakeMeshes.Add (mesh);
		}

		meshesInResources = Resources.LoadAll("SnakesModels/Shoppable");

		foreach (Object obj in meshesInResources) {
			GameObject mesh = obj as GameObject;
			shoppableSnakeMeshes.Add (mesh);
			//usableSnakeMeshes.Add (mesh);
		}

	}

	public GameObject GetMeshUsedByPlayer(){
		ShopItem shopItemToUse = ShopHandler.instance.shopItemToUse;
	
		string shopItemName = shopItemToUse.name;

		if (shopItemName == "Random Color") {
			int rand = Random.Range (0, randomSnakeMeshes.Count);
			shopItemName = randomSnakeMeshes [rand].name;
		}

		GameObject meshToRemove = null;
		foreach (GameObject mesh in usableSnakeMeshes) {			
			if (mesh.name == shopItemName) {
				meshToRemove = mesh;
				break;
			}
		}

		Debug.Log ("USABLE SNAKE MESH SIZE BEFORE = " + usableSnakeMeshes.Count);

		usableSnakeMeshes.Remove (meshToRemove);

		Debug.Log ("USABLE SNAKE MESH SIZE AFTER = " + usableSnakeMeshes.Count);

		return meshToRemove;
	}


}

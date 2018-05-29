using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
	public GameObject objectToFollow;
	public float followSpeed = 10f;


	void Awake ()
	{

	}


	// Use this for initialization
	void Start ()
	{
		



	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
		if (objectToFollow == null) {
			
			if (SnakesSpawner.instance.playerSnake != null) {
				objectToFollow = SnakesSpawner.instance.playerSnake.gameObject;

				Vector3 newCameraPos = objectToFollow.transform.position;
				newCameraPos.z = transform.position.z;

				transform.position = newCameraPos;

			}
		}
	
		if (objectToFollow != null) {
			Vector3 newCameraPos = objectToFollow.transform.position;
			newCameraPos.z = transform.position.z;

			transform.position = newCameraPos;
		}
	}


}

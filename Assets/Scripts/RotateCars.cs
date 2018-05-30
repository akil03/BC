using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCars : MonoBehaviour {
	public float speed;
	public Vector3 direction = Vector3.up;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {



		transform.Rotate(direction * speed * Time.deltaTime);
	}
}

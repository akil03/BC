using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeHandler : MonoBehaviour {

	public static SwipeHandler instance;

	public float swipeSensibility;
	Vector2 lastMousePos;


	public enum SwipeDirection {up,down,left,right};

	public SwipeDirection lastSwipeDirection;

	void Awake(){
		instance = this;
		lastSwipeDirection = SwipeDirection.up;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown (0)) {
			lastMousePos = Input.mousePosition;
		}


		if (Input.GetMouseButton(0)) {
			Vector2 currentMousePos = Input.mousePosition;

			if (Vector2.Distance (lastMousePos, currentMousePos) > swipeSensibility) {
				if (Mathf.Abs (currentMousePos.x - lastMousePos.x) > Mathf.Abs (currentMousePos.y - lastMousePos.y)) {
				
				
					if (currentMousePos.x > lastMousePos.x) {
						lastSwipeDirection = SwipeDirection.right;
					} else {
						lastSwipeDirection = SwipeDirection.left;
					}


				} else {
					if (currentMousePos.y > lastMousePos.y) {
						lastSwipeDirection = SwipeDirection.up;
					} else {
						lastSwipeDirection = SwipeDirection.down;
					}
				}

			}

			lastMousePos = currentMousePos;
		}

	}



}

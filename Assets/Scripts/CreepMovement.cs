using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CreepMovement : MonoBehaviour {
    public float speed=2.5f;
	// Use this for initialization
	void Start () {
        StartMove();

    }
    bool isFirst=true;
	// Update is called once per frame
	void Update () {
		
	}



    void StartMove()
    {
        StartCoroutine(FindNewPoint());
    }

    public IEnumerator FindNewPoint()
    {
        yield return null;


        Vector3 newPos = new Vector3(Random.Range(1, 32), Random.Range(-1, -32), -1);
        //transform.GetChild(0).LookAt(newPos);
        //transform.GetChild(0).localRotation = Quaternion.Euler(0, transform.GetChild(0).localRotation.y, 0);


        float time = Vector3.Distance(transform.position, newPos) / speed;

        if (isFirst)
        {
            transform.position = newPos;
            isFirst = false;
            StartMove();
        }
        else
            transform.DOMove(newPos, time).SetEase(Ease.Linear).OnComplete(() => {
                StartMove();
            });

    }
}

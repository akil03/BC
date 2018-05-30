﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class RandomSpin : MonoBehaviour
{
    public Ease EaseType;
    public Vector3 targetRotation;
    public float spinTime;

    public float minSpin = -60, MaxSpin = 60;


    // Use this for initialization
    void Start()
    {
        Spin();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Spin()
    {
        float rand = Random.Range(minSpin, MaxSpin);
        spinTime = Random.Range(0.85f, 2f);
        targetRotation = new Vector3(0, 0, rand);
        transform.DOLocalRotate(targetRotation, spinTime, RotateMode.Fast).SetEase(EaseType);
        Invoke("Spin", spinTime + Random.Range(0.0f, 1.0f));
    }

    public void Stop()
    {

        CancelInvoke();
        Invoke("Normalize", 1.5f);
    }

    void Normalize()
    {
        transform.DOLocalRotate(Vector3.zero, spinTime, RotateMode.Fast).SetEase(EaseType);

    }
}
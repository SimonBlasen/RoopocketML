﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineSimpleRotator : MonoBehaviour {

    public float speed = 0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


        transform.Rotate(0f, speed * Time.deltaTime, 0f);

	}
}

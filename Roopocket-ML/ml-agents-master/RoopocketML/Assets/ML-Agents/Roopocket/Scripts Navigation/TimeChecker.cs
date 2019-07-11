using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeChecker : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log("Time.fixedDeltaTime: " + Time.fixedDeltaTime);
        Debug.Log("Time.maximumDeltaTime: " + Time.maximumDeltaTime);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

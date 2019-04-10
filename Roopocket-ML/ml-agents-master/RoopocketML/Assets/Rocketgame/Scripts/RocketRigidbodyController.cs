using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketRigidbodyController : MonoBehaviour {

    public Rigidbody rocketRigidbody;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "TurnPoint")
        {
            rocketRigidbody.constraints = RigidbodyConstraints.None;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "TurnPoint")
        {
            rocketRigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform rocket;
    [SerializeField]
    private Rigidbody rocketRigidbody;
    [SerializeField]
    private Vector3 offsetVector = new Vector3(0f, 0f, -1f);
    [SerializeField]
    private float minDistance = 10f;
    [SerializeField]
    private float distanceFactor = 1f;
    [SerializeField]
    private float lerpSpeed = 0.1f;


    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        transform.position = Vector3.Lerp(transform.position, rocket.position + offsetVector.normalized * (minDistance + rocketRigidbody.velocity.magnitude * distanceFactor), lerpSpeed);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemPositioning : MonoBehaviour {

    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private Transform[] rockets;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 mid = Vector3.zero;
        

        for (int i = 0; i < rockets.Length; i++)
        {
            mid += rockets[i].position;
        }
        mid /= rockets.Length;

        transform.position = mid + offset;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionForce : MonoBehaviour
{
    public float explisionForce = 500f;

    private List<int> affected = new List<int>();

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {

	}

    private void OnTriggerEnter(Collider other)
    {
        if (affected.Contains(other.transform.GetInstanceID()) == false)
        {
            if (other.transform.GetComponent<Rigidbody>() != null)
            {
                other.transform.GetComponent<Rigidbody>().AddExplosionForce(explisionForce, transform.position, GetComponent<SphereCollider>().radius);
            }

            affected.Add(other.transform.GetInstanceID());
        }
    }
}

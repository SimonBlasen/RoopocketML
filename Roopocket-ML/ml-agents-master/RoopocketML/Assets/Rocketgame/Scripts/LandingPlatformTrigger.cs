using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingPlatformTrigger : MonoBehaviour {

    public LandingPlatform landingPlatform;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {

    }

    private void OnTriggerExit(Collider other)
    {
        Transform topParent = other.transform;
        while (topParent.parent != null)
        {
            topParent = topParent.parent;
        }
        landingPlatform.TriggerExit(topParent);
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform topParent = other.transform;
        while (topParent.parent != null)
        {
            topParent = topParent.parent;
        }
        landingPlatform.TriggerEnter(topParent);
    }
}

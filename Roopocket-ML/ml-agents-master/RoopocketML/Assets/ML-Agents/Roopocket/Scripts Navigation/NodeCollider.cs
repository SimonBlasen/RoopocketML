using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeCollider : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    public RPNavNode NavNode
    {
        get;set;
    }


    private void OnTriggerEnter(Collider other)
    {
        Transform parent = other.transform;
        while (parent.parent != null)
        {
            parent = parent.parent;
        }

        if (parent.GetComponent<RoopocketAI>() != null)
        {
            parent.GetComponent<RoopocketAI>().ReachedNode(NavNode);
        }
    }
}

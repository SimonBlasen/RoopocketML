using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    [SerializeField]
    private Transform hookedRocket;
    [SerializeField]
    private TurnPoints turnPoints;

    private bool turning = false;

    private int turningPointIndex = -1;

    // Use this for initialization
    void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
        if (turning)
        {
            transform.position = new Vector3(transform.position.x, hookedRocket.position.y, transform.position.z);
            if (Vector3.Angle(turnPoints.borderVecs[turningPointIndex], (hookedRocket.position - transform.position)) > 45f)
            {
                turning = false;
                GetComponent<HingeJoint>().connectedBody = null;
            }
        }
        else
        {
            Vector3 normalRocket = hookedRocket.GetComponent<RocketController>().Normal;
            if (normalRocket.z != 0f)
            {
                transform.position = new Vector3(hookedRocket.position.x, hookedRocket.position.y, hookedRocket.position.z + 5f * normalRocket.z);
            }
            else if (normalRocket.x != 0f)
            {
                transform.position = new Vector3(hookedRocket.position.x + 5f * normalRocket.x, hookedRocket.position.y, hookedRocket.position.z);
            }
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "TurnPoint")
        {
            for (int i = 0; i < turnPoints.turnPoints.Length; i++)
            {
                if (turnPoints.turnPoints[i].GetInstanceID() == other.transform.parent.GetInstanceID())
                {
                    turningPointIndex = i;
                    break;
                }
            }

            transform.position = new Vector3(other.transform.position.x, hookedRocket.position.y, other.transform.position.z);

            Vector3 normalRocket = hookedRocket.GetComponent<RocketController>().Normal;

            if (normalRocket.z != 0f)
            {
                hookedRocket.position = new Vector3(other.transform.position.x, hookedRocket.position.y, hookedRocket.position.z);
            }
            else if (normalRocket.x != 0f)
            {
                hookedRocket.position = new Vector3(hookedRocket.position.x, hookedRocket.position.y, other.transform.position.z);
            }

            GetComponent<HingeJoint>().connectedBody = hookedRocket.GetComponent<Rigidbody>();
            hookedRocket.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

            turning = true;
        }
    }
}

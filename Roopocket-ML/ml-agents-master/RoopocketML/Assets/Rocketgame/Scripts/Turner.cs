using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turner : MonoBehaviour {

    public Transform midPoint;
    public float radius = 5f;
    public Vector2 inVector;
    public bool insideCurve = false;
    public CameraMultiController cmc;

    private List<Transform> inRockets = new List<Transform>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
		for (int i = 0; i < inRockets.Count; i++)
        {
            Vector3 toRocket3 = (inRockets[i].position - midPoint.position);

            toRocket3.y = 0f;

            toRocket3.Normalize();
            toRocket3 *= radius;

            Vector2 toRocket = new Vector2(toRocket3.x, toRocket3.z);
            float curAngle = Vector2.Angle(toRocket, inVector);


            if (inRockets[i].GetComponent<RocketController>().IsKI == false)
            {
                if (insideCurve)
                {
                    cmc.OffsetVector = new Vector3(toRocket.x * -1f, 0f, toRocket.y * -1f);
                }
                else
                {
                    cmc.OffsetVector = new Vector3(toRocket.x * 1f, 0f, toRocket.y * 1f);
                }
            }


            float angleAbs = Vector2.Angle((new Vector2(0f, 1f)), toRocket);
            if (Vector2.Angle((new Vector2(1f, 0f)), toRocket) > 90f)
            {
                angleAbs = 360f - angleAbs;
            }

            float difference = Mathf.Abs(angleAbs - inRockets[i].rotation.eulerAngles.y);
            while (difference > 360f)
            {
                difference -= 360f;
            }

            if (difference > 90f && difference < 270f)
            {
                //Debug.Log("Big: " + angleAbs + ", " + inRockets[i].rotation.eulerAngles.y);
                angleAbs += 180f;
            }

            inRockets[i].position = toRocket3 + midPoint.position + new Vector3(0f, inRockets[i].position.y, 0f);
            inRockets[i].rotation = Quaternion.Euler(inRockets[i].rotation.eulerAngles.x, angleAbs, inRockets[i].rotation.eulerAngles.z);


            Vector3 vel = inRockets[i].GetComponent<Rigidbody>().velocity;
            Vector3 topVec = Vector3.Cross(vel, toRocket3);
            Vector3 newVel = Vector3.Cross(toRocket3, topVec);
            if (Vector3.Angle(newVel, vel) > 90f)
            {
                Debug.Log("Other way round");
                newVel *= -1f;
            }

            newVel = newVel.normalized * vel.magnitude;
            inRockets[i].GetComponent<Rigidbody>().velocity = newVel;
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Rocket")
        {
            Transform topParent = other.transform;
            while (topParent.parent != null)
            {
                topParent = topParent.parent;
            }

            if (topParent.tag == "RocketKI")
            {
                topParent = topParent.GetComponentInChildren<Rigidbody>().transform;
            }

            if (inRockets.Contains(topParent) == false)
            {
                topParent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

                inRockets.Add(topParent);

                topParent.GetComponent<RocketController>().Turning = true;

                Debug.Log("Rocket in");
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Rocket")
        {
            Transform topParent = other.transform;
            while (topParent.parent != null)
            {
                topParent = topParent.parent;
            }

            if (topParent.tag == "RocketKI")
            {
                topParent = topParent.GetComponentInChildren<Rigidbody>().transform;
            }

            if (inRockets.Contains(topParent))
            {
                Debug.Log(cmc.OffsetVector.ToString());
                Vector3 toRocket3 = (topParent.position - midPoint.position);
                Vector2 toRocket = new Vector2(toRocket3.x, toRocket3.z);
                toRocket.Normalize();
                float curAngle = Vector2.Angle(toRocket, inVector);

                if (Mathf.Abs(toRocket.y) > Mathf.Abs(toRocket.x))
                {
                    topParent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ;// | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

                    //cmc.OffsetVector = new Vector3((int)(toRocket.x), 0f, (int)(toRocket.y + 0.5f * Mathf.Sign(toRocket.y)));

                }
                else
                {
                    topParent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX;// | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;

                    //cmc.OffsetVector = new Vector3((int)(toRocket.x + 0.5f * Mathf.Sign(toRocket.x)), 0f, (int)(toRocket.y));
                }
                Vector3 newOffset = Vector3.zero;
                float absMax = Mathf.Max(Mathf.Abs(cmc.OffsetVector.x), Mathf.Abs(cmc.OffsetVector.z));
                if (cmc.OffsetVector.x >= absMax)
                {
                    newOffset.x = 1f;
                }
                else if (cmc.OffsetVector.x <= -absMax)
                {
                    newOffset.x = -1f;
                }
                else if (cmc.OffsetVector.z >= absMax)
                {
                    newOffset.z = 1f;
                }
                else
                {
                    newOffset.z = -1f;
                }

                cmc.OffsetVector = newOffset;

                //Debug.Log("new rot: " + ((int)((topParent.rotation.eulerAngles.y + 45f) / 90f)) * 90f);
                topParent.rotation = Quaternion.Euler(0f, ((int)((topParent.rotation.eulerAngles.y + 45f) / 90f)) * 90f, topParent.rotation.eulerAngles.z);

                Vector3 point1 = midPoint.position + (Quaternion.Euler(0f, 45f, 0f) * (new Vector3(inVector.x, 0f, inVector.y))).normalized * radius;
                Vector3 point2 = midPoint.position + (Quaternion.Euler(0f, -45f, 0f) * (new Vector3(inVector.x, 0f, inVector.y))).normalized * radius;

                float destinationX;
                float destinationZ;
                if (Vector2.Distance(new Vector2(point1.x, point1.z), new Vector2(topParent.position.x, topParent.position.z)) < Vector2.Distance(new Vector2(point2.x, point2.z), new Vector2(topParent.position.x, topParent.position.z)))
                {
                    destinationX = point1.x;
                    destinationZ = point1.z;
                }
                else
                {
                    destinationX = point2.x;
                    destinationZ = point2.z;
                }


                if (newOffset.x != 0f && topParent.position.x >= 0f)
                {
                    topParent.position = new Vector3(destinationX, topParent.position.y, topParent.position.z);
                }
                else if (newOffset.x != 0f && topParent.position.x < 0f)
                {
                    topParent.position = new Vector3(destinationX, topParent.position.y, topParent.position.z);
                }
                else
                if (newOffset.z != 0f && topParent.position.z >= 0f)
                {
                    topParent.position = new Vector3(topParent.position.x, topParent.position.y, destinationZ);
                }
                else if (newOffset.z != 0f && topParent.position.z < 0f)
                {
                    topParent.position = new Vector3(topParent.position.x, topParent.position.y, destinationZ);
                }

                topParent.GetComponent<RocketController>().Turning = false;

                inRockets.Remove(topParent);

                Debug.Log("Rocket out");
            }

            
        }
    }
}

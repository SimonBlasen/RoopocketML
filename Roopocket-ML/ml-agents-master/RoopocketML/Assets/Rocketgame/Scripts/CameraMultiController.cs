using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMultiController : MonoBehaviour
{
    [SerializeField]
    private Transform[] rockets;
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
    [SerializeField]
    private float camScaleFactor = 2f;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 mid = Vector3.zero;

        Vector3 minBB = rockets[0].position;
        Vector3 maxBB = rockets[0].position;

        for (int i = 0; i < rockets.Length; i++)
        {
            if (rockets[i].position.x < minBB.x)
            {
                minBB.x = rockets[i].position.x;
            }
            if (rockets[i].position.y < minBB.y)
            {
                minBB.y = rockets[i].position.y;
            }
            if (rockets[i].position.z < minBB.z)
            {
                minBB.z = rockets[i].position.z;
            }

            if (rockets[i].position.x > maxBB.x)
            {
                maxBB.x = rockets[i].position.x;
            }
            if (rockets[i].position.y > maxBB.y)
            {
                maxBB.y = rockets[i].position.y;
            }
            if (rockets[i].position.z > maxBB.z)
            {
                maxBB.z = rockets[i].position.z;
            }
            mid += rockets[i].position;
        }
        mid /= rockets.Length;
        float maxDist = Vector3.Distance(minBB, maxBB) * camScaleFactor;
        if (maxDist < minDistance)
        {
            maxDist = minDistance;
        }

        Vector3 vecUp = Vector3.up;
        Vector3 vecSide = Vector3.Cross(vecUp, offsetVector);


        transform.position = Vector3.Lerp(transform.position, mid + offsetVector.normalized * (maxDist + (IsRocketDead ? 20f : (rocketRigidbody != null ? rocketRigidbody.velocity.magnitude * distanceFactor : 0f))), lerpSpeed);
        transform.LookAt(transform.position + offsetVector * -1f);
    }

    public bool IsRocketDead
    {
        get;set;
    }

    public Vector3 OffsetVector
    {
        get
        {
            return offsetVector;
        }
        set
        {
            offsetVector = value;
        }
    }
}

using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoopocketAccelerationAgent : Agent
{
    public GameObject rocketObject;
    public Vector2 goalVel;
    public GameObject directionPointer;
    private RocketController rocketController;
    private RocketProps rocketProps;
    private Rigidbody rb;

    public bool reset = false;

    private float zPos = 0f;

    private float timeRun = 20f;

    private Vector2 currentVel = Vector2.zero;

    private void Update()
    {
        timeRun -= Time.deltaTime;
        directionPointer.transform.position = rocketObject.transform.position;

        if (reset)
        {
            reset = false;
            Done();
        }
    }

    public override void InitializeAgent()
    {
        zPos = rocketObject.transform.position.z;
        rocketController = rocketObject.GetComponent<RocketController>();
        rocketProps = rocketObject.GetComponent<RocketProps>();
        rb = rocketObject.GetComponent<Rigidbody>();
    }

    public override void CollectObservations()
    {
        AddVectorObs(Mathf.Cos(rocketObject.transform.rotation.eulerAngles.z * Mathf.PI / 180f));
        AddVectorObs(Mathf.Sin(rocketObject.transform.rotation.eulerAngles.z * Mathf.PI / 180f));
        AddVectorObs(goalVel.x * 0.25f);
        AddVectorObs(goalVel.y * 0.25f);
        AddVectorObs(rb.velocity.x * 0.25f);
        AddVectorObs(rb.velocity.y * 0.25f);
        AddVectorObs(rb.angularVelocity.z);
    }

    private float lastDistance = 0f;

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        for (int i = 0; i < vectorAction.Length; i++)
        {
            rocketController.SetThrust(i, Mathf.Clamp(vectorAction[i], -1f, 1f) >= 0f);
        }

        Vector2 velDelta = goalVel - (new Vector2(rb.velocity.x, rb.velocity.y));

        float deltaMag = velDelta.magnitude < 0.01f ? 0.01f : velDelta.magnitude;

        float rew = 1f / (deltaMag / goalVel.magnitude);

        rew /= 1000f;
            
        SetReward(rew);


        if (Mathf.Abs(goalVel.magnitude - (new Vector2(rb.velocity.x, rb.velocity.y)).magnitude) > 10f)
        {
            SetReward(-1f);
            Done();
        }

        if (timeRun <= 0f)
        {
            Done();
        }
    }

    public override void AgentReset()
    {
        float goalAngle = Random.Range(0f, Mathf.PI * 2f);
        float goalRadius = Random.Range(0f, 5f);

        goalVel = new Vector2(Mathf.Cos(goalAngle) * goalRadius, Mathf.Sin(goalAngle) * goalRadius);

        directionPointer.transform.rotation = Quaternion.Euler(0f, 0f, (goalAngle / Mathf.PI) * 180f - 90f);

        float startAngle = Random.Range(0f, Mathf.PI * 2f);
        float startVelRadius = Random.Range(0f, 5f);
        
        
            rocketObject.transform.position = new Vector3(0f, 0f, zPos);
        
        rocketObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        rocketObject.transform.Rotate(new Vector3(0, 0, 1), Random.Range(-180f, 180f));
        rb.velocity = new Vector3(Mathf.Cos(startAngle) * startVelRadius, Mathf.Sin(startAngle) * startVelRadius, 0f);
        rb.angularVelocity = new Vector3(0f, 0f, Random.Range(-25f, 25f) * Mathf.PI / 180f);

        Debug.Log("Goal: " + goalVel.ToString());

        timeRun = 15f;
    }

}

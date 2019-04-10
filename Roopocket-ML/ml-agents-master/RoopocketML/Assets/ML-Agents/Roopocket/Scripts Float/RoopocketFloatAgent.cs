using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoopocketFloatAgent : Agent
{
    [Header("Specific to Ball3D")]
    public GameObject rocketObject;
    public GameObject goal;
    private RocketController rocketController;
    private RocketProps rocketProps;
    private Rigidbody rb;

    public bool randomiseStart = true;

    private float zPos = 0f;

    private float timeRun = 50f;

    private void Update()
    {
        if (randomiseStart)
        {
            timeRun -= Time.deltaTime;
        }
    }

    public override void InitializeAgent()
    {
        zPos = rocketObject.transform.position.z;
        rocketController = rocketObject.GetComponent<RocketController>();
        rocketProps = rocketObject.GetComponent<RocketProps>();
        rb = rocketObject.GetComponent<Rigidbody>();
        rocketController.UseFloatThrusts = true;
    }

    public override void CollectObservations()
    {
        float xCos = Mathf.Cos(rocketController.AbsoluteYAngle * Mathf.PI / 180f + Mathf.PI);
        float zSin = Mathf.Sin(rocketController.AbsoluteYAngle * Mathf.PI / 180f);

        AddVectorObs(Mathf.Cos(rocketObject.transform.rotation.eulerAngles.z * Mathf.PI / 180f));
        AddVectorObs(Mathf.Sin(rocketObject.transform.rotation.eulerAngles.z * Mathf.PI / 180f));
        AddVectorObs((goal.transform.position.x - rocketObject.transform.position.x) * 0.1f * xCos + (goal.transform.position.z - rocketObject.transform.position.z) * 0.1f * zSin);
        AddVectorObs((goal.transform.position.y - rocketObject.transform.position.y) * 0.1f);
        AddVectorObs(rb.velocity.x * xCos + rb.velocity.z * zSin);
        AddVectorObs(rb.velocity.y);
        AddVectorObs(rb.angularVelocity.z * xCos + rb.angularVelocity.x * -zSin);
    }

    private float lastDistance = 0f;

    public override void AgentAction(float[] vectorAction, string textAction)
    {

        //if (brain.brainParameters.vectorActionSpaceType == SpaceType.discrete)
        //{
        for (int i = 0; i < vectorAction.Length; i++)
        {
            rocketController.SetThrustFloat(i, Mathf.Clamp(vectorAction[i], -1f, 1f));
        }
        //}
        if (Vector3.Distance(rocketObject.transform.position, goal.transform.position) > 50f)
        {
            Done();
            SetReward(-1f);
        }
        else
        {
            float curDistance = Vector3.Distance(goal.transform.position, rocketObject.transform.position);
            /*if (curDistance < lastDistance)
            {
                SetReward(0.1f);
            }
            else
            {
                SetReward(-0.1f);
            }*/

            //Debug.Log(lastDistance - curDistance);

            SetReward(lastDistance - curDistance);

            //Debug.Log("Distance: " + curDistance.ToString());

            lastDistance = curDistance;
        }


        if (timeRun <= 0f && randomiseStart)
        {
            Done();
        }
    }

    public override void AgentReset()
    {
        float goalX = Random.Range(-20f, 20f);
        float goalY = Random.Range(-20f, 20f);

        float startAngle = Random.Range(0f, Mathf.PI * 2f);
        float startVelRadius = Random.Range(0f, 5f);

        if (randomiseStart)
        {
            goal.transform.position = new Vector3(goalX, goalY, zPos);
        }

        if (timeRun > 0f || randomiseStart)
        {
            rocketObject.transform.position = new Vector3(0f, 0f, zPos);
        }
        rocketObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        rocketObject.transform.Rotate(new Vector3(0, 0, 1), Random.Range(-30f, 30f));
        if (randomiseStart)
        {
            rb.velocity = new Vector3(Mathf.Cos(startAngle) * startVelRadius, Mathf.Sin(startAngle) * startVelRadius, 0f);
        }
        else
        {
            rb.velocity = Vector3.zero;
        }

        rb.angularVelocity = new Vector3(0f, 0f, Random.Range(-0.5f, 0.5f));

        lastDistance = Vector3.Distance(goal.transform.position, rocketObject.transform.position);
        timeRun = 30f;
    }

}

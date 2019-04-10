using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoopocketLandingAgent : Agent
{
    [Header("Specific to Ball3D")]
    public GameObject rocketObject;
    public GameObject landingPlatform;
    private LandingPlatform landingPlatformS;
    private RocketController rocketController;
    private RocketProps rocketProps;
    private Rigidbody rb;

    public bool randomiseStart = true;

    private float zPos = 0f;

    private float timeRun = 50f;

    private void Update()
    {
        timeRun -= Time.deltaTime;
    }

    public override void InitializeAgent()
    {
        landingPlatformS = landingPlatform.GetComponent<LandingPlatform>();
        zPos = rocketObject.transform.position.z;
        rocketController = rocketObject.GetComponent<RocketController>();
        rocketProps = rocketObject.GetComponent<RocketProps>();
        rb = rocketObject.GetComponent<Rigidbody>();
    }

    public override void CollectObservations()
    {
        float xCos = Mathf.Cos(rocketController.AbsoluteYAngle * Mathf.PI / 180f + Mathf.PI);
        float zSin = Mathf.Sin(rocketController.AbsoluteYAngle * Mathf.PI / 180f);

        //Debug.Log("Vel: " + (rb.velocity.x * xCos + rb.velocity.z * zSin).ToString());

        AddVectorObs(Mathf.Cos(rocketObject.transform.rotation.eulerAngles.z * Mathf.PI / 180f));
        AddVectorObs(Mathf.Sin(rocketObject.transform.rotation.eulerAngles.z * Mathf.PI / 180f));
        AddVectorObs((landingPlatform.transform.position.x - rocketObject.transform.position.x) * 0.1f * xCos + (landingPlatform.transform.position.z - rocketObject.transform.position.z) * 0.1f * zSin);
        AddVectorObs((landingPlatform.transform.position.y - rocketObject.transform.position.y) * 0.1f);
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
            rocketController.SetThrust(i, Mathf.Clamp(vectorAction[i], -1f, 1f) >= 0f);
        }
        //}
        if (Vector3.Distance(rocketObject.transform.position, landingPlatform.transform.position + new Vector3(0f, 1.4f, 0f)) > 30f)
        {
            if (randomiseStart)
            {
                Done();
            }
            SetReward(-3f);
        }
        else if (landingPlatformS.StandsOn(rocketObject.transform) == false)
        {
            float curDistance = Vector3.Distance(landingPlatform.transform.position + new Vector3(0f, 1.4f, 0f), rocketObject.transform.position);

            float velMag = rb.velocity.magnitude;

            if (velMag < 0.05f)
            {
                velMag = 0.05f;
            }

            float angleCos = Mathf.Cos(rocketObject.transform.rotation.eulerAngles.z * Mathf.PI / 180f);
            float delta = lastDistance - curDistance;
            float rew = delta * angleCos;
            if (angleCos < 0f && delta > 0f)
            {
                delta *= -1f;
            }
            SetReward(delta/* * (curDistance / velMag)*/);

            //Debug.Log("Distance: " + curDistance.ToString());

            lastDistance = curDistance;
        }
        else if (landingPlatformS.StandsOn(rocketObject.transform))
        {

            float velMag = rb.velocity.magnitude;
            float angleCos = Mathf.Cos(rocketObject.transform.rotation.eulerAngles.z * Mathf.PI / 180f);



            if (firstTimeEnter)
            {
                firstTimeEnter = false;
                enterVelocity = velMag;
            }



            velMag = 1f - velMag;

            if (velMag < -1f)
            {
                velMag = -1f;
            }

            //angleCos = (angleCos - 0.5f);



            //Done();
            //Debug.Log("Reward: " + (angleCos + velMag).ToString());
            SetReward(velMag * angleCos);
            if (randomiseStart)
            {
                Done();
            }
        }


        if (timeRun <= 0f && randomiseStart)
        {
            float curDistance = Vector3.Distance(landingPlatform.transform.position + new Vector3(0f, 1.4f, 0f), rocketObject.transform.position);
            Done();
            if (curDistance < 1.6f)
            {
                //SetReward(0.2f);
            }
        }
    }

    private float enterVelocity = 0f;
    private bool firstTimeEnter = false;

    private float accVariance = 0f;
    private float initDistance = 0f;

    public override void AgentReset()
    {
        firstTimeEnter = true;
        //accVariance += 0.005f;

        float goalX = Random.Range(-2f - accVariance, 2f + accVariance);
        float goalY = Random.Range(-4f, 0f);

        float startAngle = Random.Range(0f, Mathf.PI * 2f);
        float startVelRadius = Random.Range(0f, 0.7f);

        if (randomiseStart)
        {
            landingPlatform.transform.position = new Vector3(goalX, goalY, zPos);
        }

        if ((timeRun > 0f && randomiseStart) || randomiseStart)
        {
            rocketObject.transform.position = new Vector3(0f, 10f, zPos);
        }
        if (randomiseStart)
        {
            rocketObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            rocketObject.transform.Rotate(new Vector3(0, 0, 1), Random.Range(-45f, 45f));
            rb.velocity = new Vector3(Mathf.Cos(startAngle) * startVelRadius, Mathf.Sin(startAngle) * startVelRadius, 0f);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        else
        {
        }

        if (randomiseStart)
        {
            landingPlatformS.KIResetStandOn();
        }

        lastDistance = Vector3.Distance(landingPlatform.transform.position + new Vector3(0f, 1.4f, 0f), rocketObject.transform.position);
        initDistance = lastDistance;
        timeRun = 30f;
    }

}

using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoopocketCollisionAgent : Agent
{
    [Header("Specific to Ball3D")]
    public GameObject rocketObject;
    public GameObject goal;
    private RocketController rocketController;
    private RocketProps rocketProps;
    private Rigidbody rb;
    public Transform[] raysStart;
    public float raysDistance = 7f;
    public float raysOffset = 1f;
    public Transform rocketAgentGoal;
    public GameObject[] obstacles;
    public Transform obstaclesParent;

    private float zPos = 0f;

    private float timeRun = 50f;

    private void Update()
    {
        timeRun -= Time.deltaTime;
    }

    public override void InitializeAgent()
    {
        zPos = rocketObject.transform.position.z;
        movingGoal = new Vector3(0f, 0f, zPos);
        rocketController = rocketObject.GetComponent<RocketController>();
        rocketProps = rocketObject.GetComponent<RocketProps>();
        rb = rocketObject.GetComponent<Rigidbody>();
    }

    public override void CollectObservations()
    {
        RaycastHit hit0;
        bool hit0B = Physics.Raycast(new Ray(rocketObject.transform.position + (new Vector3(1f, 0f, 0f)).normalized * raysOffset, new Vector3(1f, 0f, 0f)), out hit0, raysDistance);
        RaycastHit hit1;
        bool hit1B = Physics.Raycast(new Ray(rocketObject.transform.position + (new Vector3(1f, -1f, 0f)).normalized * raysOffset, new Vector3(1f, -1f, 0f)), out hit1, raysDistance);
        RaycastHit hit2;
        bool hit2B = Physics.Raycast(new Ray(rocketObject.transform.position + (new Vector3(0f, -1f, 0f)).normalized * raysOffset, new Vector3(0f, -1f, 0f)), out hit2, raysDistance);
        RaycastHit hit3;
        bool hit3B = Physics.Raycast(new Ray(rocketObject.transform.position + (new Vector3(-1f, -1f, 0f)).normalized * raysOffset, new Vector3(-1f, -1f, 0f)), out hit3, raysDistance);
        RaycastHit hit4;
        bool hit4B = Physics.Raycast(new Ray(rocketObject.transform.position + (new Vector3(-1f, 0f, 0f)).normalized * raysOffset, new Vector3(-1f, 0f, 0f)), out hit4, raysDistance);
        RaycastHit hit5;
        bool hit5B = Physics.Raycast(new Ray(rocketObject.transform.position + (new Vector3(-1f, 1f, 0f)).normalized * raysOffset, new Vector3(-1f, 1f, 0f)), out hit5, raysDistance);
        RaycastHit hit6;
        bool hit6B = Physics.Raycast(new Ray(rocketObject.transform.position + (new Vector3(0f, 1f, 0f)).normalized * raysOffset, new Vector3(0f, 1f, 0f)), out hit6, raysDistance);
        RaycastHit hit7;
        bool hit7B = Physics.Raycast(new Ray(rocketObject.transform.position + (new Vector3(1f, 1f, 0f)).normalized * raysOffset, new Vector3(1f, 1f, 0f)), out hit7, raysDistance);

        /*

        Debug.DrawLine(rocketObject.transform.position, rocketObject.transform.position + (new Vector3(1f, 0f, 0f)).normalized *   ( hit0B ? hit0.distance + raysOffset : (raysDistance + raysOffset)), Color.green, 0.1f);
        Debug.DrawLine(rocketObject.transform.position, rocketObject.transform.position + (new Vector3(1f, -1f, 0f)).normalized *  ( hit1B ? hit1.distance + raysOffset : (raysDistance + raysOffset)), Color.green, 0.1f);
        Debug.DrawLine(rocketObject.transform.position, rocketObject.transform.position + (new Vector3(0f, -1f, 0f)).normalized *  ( hit2B ? hit2.distance + raysOffset : (raysDistance + raysOffset)), Color.green, 0.1f);
        Debug.DrawLine(rocketObject.transform.position, rocketObject.transform.position + (new Vector3(-1f, -1f, 0f)).normalized * ( hit3B ? hit3.distance + raysOffset : (raysDistance + raysOffset)), Color.green, 0.1f);
        Debug.DrawLine(rocketObject.transform.position, rocketObject.transform.position + (new Vector3(-1f, 0f, 0f)).normalized *  ( hit4B ? hit4.distance + raysOffset : (raysDistance + raysOffset)), Color.green, 0.1f);
        Debug.DrawLine(rocketObject.transform.position, rocketObject.transform.position + (new Vector3(-1f, 1f, 0f)).normalized *  ( hit5B ? hit5.distance + raysOffset : (raysDistance + raysOffset)), Color.green, 0.1f);
        Debug.DrawLine(rocketObject.transform.position, rocketObject.transform.position + (new Vector3(0f, 1f, 0f)).normalized *   ( hit6B ? hit6.distance + raysOffset : (raysDistance + raysOffset)), Color.green, 0.1f);
        Debug.DrawLine(rocketObject.transform.position, rocketObject.transform.position + (new Vector3(1f, 1f, 0f)).normalized *   ( hit7B ? hit7.distance + raysOffset : (raysDistance + raysOffset)), Color.green, 0.1f);


        */

        AddVectorObs((hit0B ? hit0.distance : raysDistance) / raysDistance);
        AddVectorObs((hit1B ? hit1.distance : raysDistance) / raysDistance);
        AddVectorObs((hit2B ? hit2.distance : raysDistance) / raysDistance);
        AddVectorObs((hit3B ? hit3.distance : raysDistance) / raysDistance);
        AddVectorObs((hit4B ? hit4.distance : raysDistance) / raysDistance);
        AddVectorObs((hit5B ? hit5.distance : raysDistance) / raysDistance);
        AddVectorObs((hit6B ? hit6.distance : raysDistance) / raysDistance);
        AddVectorObs((hit7B ? hit7.distance : raysDistance) / raysDistance);


        //AddVectorObs(Mathf.Cos(rocketObject.transform.rotation.eulerAngles.z * Mathf.PI / 180f));
        //AddVectorObs(Mathf.Sin(rocketObject.transform.rotation.eulerAngles.z * Mathf.PI / 180f));
        AddVectorObs((goal.transform.position.x - rocketObject.transform.position.x) * 0.1f);
        AddVectorObs((goal.transform.position.y - rocketObject.transform.position.y) * 0.1f);
        AddVectorObs(rb.velocity.x);
        AddVectorObs(rb.velocity.y);
        //AddVectorObs(rb.angularVelocity.z);
    }

    private float lastDistance = 0f;

    private Vector3 movingGoal = new Vector3(0f, 0f, 0f);

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        movingGoal += new Vector3(vectorAction[0] * 1f, vectorAction[1] * 1f, 0f);
        rocketAgentGoal.transform.position = new Vector3(vectorAction[0] * 5f, vectorAction[1] * 5f, 0f) + rocketObject.transform.position;
        

        
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



        if (rocketProps.MLAccDamage > 0)
        {
            rocketProps.MLAccDamage = 0;
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
        float goalRadius = 15f;
        float goalAngle = Random.Range(0f, Mathf.PI * 2f);

        float startAngle = Random.Range(0f, Mathf.PI * 2f);
        float startVelRadius = Random.Range(0f, 0f);

        goal.transform.position = new Vector3(Mathf.Cos(goalAngle) * goalRadius, Mathf.Sin(goalAngle) * goalRadius, zPos);

        if (timeRun > 0f || true)
        {
            rocketObject.transform.position = new Vector3(0f, 0f, zPos);
        }
        rocketObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        rocketObject.transform.Rotate(new Vector3(0, 0, 1), Random.Range(-10f, 10f));
        rb.velocity = new Vector3(Mathf.Cos(startAngle) * startVelRadius, Mathf.Sin(startAngle) * startVelRadius, 0f);
        rb.angularVelocity = Vector3.zero;


        float obstacleParentAngle = Random.Range(0f, 360f);


        float obstacleSpawnProp = 0.5f;

        obstaclesParent.Rotate(new Vector3(0, 0, 1), obstacleParentAngle);
        for (int i = 0; i < obstacles.Length; i++)
        {
            if (Random.Range(0f, 1f) < obstacleSpawnProp)
            {
                obstacles[i].SetActive(true);
            }
            else
            {
                obstacles[i].SetActive(false);
            }
        }


        lastDistance = Vector3.Distance(goal.transform.position, rocketObject.transform.position);
        timeRun = 30f;
    }

}

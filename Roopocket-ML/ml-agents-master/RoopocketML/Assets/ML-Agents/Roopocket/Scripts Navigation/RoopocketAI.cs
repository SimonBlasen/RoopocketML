using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoopocketAI : MonoBehaviour
{
    [SerializeField]
    private bool refreshGoal = false;
    [SerializeField]
    private string currentGoal = "";
    [SerializeField]
    private string currentStation = "";
    [SerializeField]
    private float landingVelThresh = 0.2f;

    [SerializeField]
    private RPNavGraph graph;

    [SerializeField]
    private RoopocketFloatAgent flyAgent;
    [SerializeField]
    private RoopocketLandingAgent landingAgent;
    [SerializeField]
    private Rigidbody rocketRB;
    [SerializeField]
    private RocketController rocketController;

    private List<RPNavNode> currentPath = new List<RPNavNode>();
    private RPNavNode currentGoalFinal = null;

    private bool allOff = false;
    private bool waitForLowVelocity = false;

    private float standStillFor = 0f;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (refreshGoal)
        {
            refreshGoal = false;
            SetGoal(currentGoal);
        }


        if (landingAgent.enabled)
        {
            if (rocketRB.velocity.magnitude < 0.1f)
            {
                standStillFor += Time.deltaTime;
            }
            else
            {
                standStillFor = 0f;
            }
            if (standStillFor >= 0.4f)
            {
                landingAgent.enabled = false;

                allOff = true;
                rocketController.SetThrust(0, false);
                rocketController.SetThrust(1, false);
                rocketController.SetThrust(2, false);
                rocketController.SetThrust(3, false);
            }
        }

        if (allOff)
        {
            rocketController.SetThrust(0, false);
            rocketController.SetThrust(1, false);
            rocketController.SetThrust(2, false);
            rocketController.SetThrust(3, false);
        }
        
        if (waitForLowVelocity)
        {
            if (rocketRB.velocity.magnitude < landingVelThresh)
            {
                waitForLowVelocity = false;
                updateTempGoal();
            }
        }
	}

    public void ReachedNode(RPNavNode node)
    {
        if (currentPath.Count <= 1)
        {
            waitForLowVelocity = true;
            currentPath.Remove(node);
            
        }
        else
        {
            currentPath.Remove(node);

            updateTempGoal();
        }
    }

    public void SetGoal(string goal)
    {
        currentGoal = goal;

        currentPath = graph.GetPathTo(currentStation, currentGoal);
        currentGoalFinal = currentPath[currentPath.Count - 1];

        updateTempGoal();

        rocketController.UseFloatThrusts = true;
        landingAgent.enabled = false;
        flyAgent.enabled = true;
        rocketController.LandingMoversOut = false;
        allOff = false;
    }

    private void updateTempGoal()
    {
        if (currentPath.Count > 0)
        {
            flyAgent.goal.transform.position = currentPath[0].transform.position;
        }
        else
        {
            flyAgent.enabled = false;
            landingAgent.landingPlatform = currentGoalFinal.landingPlatform;
            landingAgent.enabled = true;

            standStillFor = 0f;
            allOff = false;

            currentStation = currentGoal;

            rocketController.UseFloatThrusts = false;
            rocketController.LandingMoversOut = true;
        }
    }
}

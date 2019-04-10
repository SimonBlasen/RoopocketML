using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationPlanner : MonoBehaviour
{
    public float goalDistance = 4f;
    public float curvatureImpact = 1f;
    public float sqrtFactor = 1f;

    public Transform rocket;
    private Rigidbody rocketRb;
    public SappSpline spline;
    public float distanceDelta = 0.01f;


    public RoopocketAgent agentFly;
    public RoopocketLandingAgent agentLanding;


    public Transform rocketGoal;

    [SerializeField]
    private float splineS = 0f;

    // Use this for initialization
    void Start ()
    {
        rocketRb = rocket.GetComponent<Rigidbody>();
	}

	
	// Update is called once per frame
	void Update ()
    {
        float currentDistance = Vector3.Distance(rocket.position, rocketGoal.position);

        float currentCurvature = spline.AngleBetweenSplines(splineS) / spline.LengthOfBezier(splineS);

        Debug.Log("Curv: " + currentCurvature.ToString());

        if (currentCurvature < 0.01f)
        {
            currentCurvature = 0.01f;
        }

        goalDistance = (180f * curvatureImpact) / currentCurvature;
        goalDistance = sqrtFactor * Mathf.Sqrt(goalDistance);

        if (currentDistance < goalDistance || splineS == 0f)
        {
            splineS += distanceDelta;
            if (splineS > 1f)
            {
                splineS = 1f;

                if (currentDistance < 0.3f && rocketRb.velocity.magnitude < 1f)
                {
                    agentFly.enabled = false;
                    agentLanding.enabled = true;
                }
            }
            rocketGoal.position = spline.SplineAt(splineS);
        }
	}
}

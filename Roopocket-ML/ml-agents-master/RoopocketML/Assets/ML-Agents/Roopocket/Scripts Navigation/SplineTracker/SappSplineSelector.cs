using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SappSplineSelector : MonoBehaviour
{
    public SappSpline[] splines;
    public float speed = 0.2f;

    public float spline = 0f;
    private bool moving = false;
    private bool curForward = true;
    public int curSpline = 0;


    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (moving)
        {
            float invPos = curForward ? spline : 1f - spline;
            transform.position = splines[curSpline].SplineAt(invPos);

            if (spline >= 1f)
            {
                moving = false;
            }

            spline += Time.deltaTime * speed;

            if (spline > 1f)
            {
                spline = 1f;
                //moving = false;
            }
        }
    }

    public bool RunSpline(int index, bool forward)
    {
        if (moving == false)
        {
            curForward = forward;
            curSpline = index;

            moving = true;

            spline = 0f;

            return true;
        }

        return false;
    }

	public bool RunSpline(string splineName, bool forward)
	{
		if (moving == false)
		{
			for (int i = 0; i < splines.Length; i++)
			{
				if (splines[i].splineName == splineName)
				{
					curForward = forward;
					curSpline = i;

					moving = true;

					spline = 0f;

					return true;
				}
			}
		}

		return false;
	}

    public bool Moving
    {
        get
        {
            return moving;
        }
    }

    public SappSpline GetSpline(string splineName)
    {
        for (int i = 0; i < splines.Length; i++)
        {
            if (splines[i].splineName == splineName)
            {
                return splines[i];
            }
        }

        return null;
    }
}

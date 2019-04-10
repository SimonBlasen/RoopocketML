using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SappSplineFollower : MonoBehaviour
{
    public SappSpline[] splines;

    public float speed = 0.2f;

    public float spline = 0f;

    public int curPos = 0;

    bool toRight = false;

    public bool moving = false;

    public bool loop = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (moving)
        {
            //int splineToUse = toRight ? curPos - 1 : curPos;
            transform.position = splines[curPos].SplineAt(spline);
            
            if (toRight)
            {
                spline += Time.deltaTime * speed;
            }
            else
            {
                spline -= Time.deltaTime * speed;
            }

            if (spline < 0f)
            {
                spline = 0f;
                if (toRight == false)
                {
                    if (!loop)
                        moving = false;
                    else
                    {
                        spline = 1f;
                    }
                }
            }
            if (spline > 1f)
            {
                spline = 1f;
                if (toRight)
                {
                    if (!loop)
                        moving = false;
                    else
                    {
                        spline = 0f;
                    }
                }
            }
        }
        else
        {
            bool move = false;

            bool notMoveIfEqual = false;
            int oldPos = curPos;

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (toRight == false)
                {
                    curPos--;
                    notMoveIfEqual = true;
                }
                else
                {
                    toRight = false;
                }
                move = true;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (toRight)
                {
                    curPos++;
                    notMoveIfEqual = true;
                }
                else
                {
                    toRight = true;
                }
                move = true;
            }

            curPos = curPos < 0 ? 0 : (curPos >= splines.Length ? splines.Length - 1 : curPos);

            if (notMoveIfEqual && oldPos == curPos)
            {
                move = false;
            }

            if (move)
            {
                moving = true;
                if (toRight)
                {
                    spline = 0f;
                }
                else
                {
                    spline = 1f;
                }
            }
        }
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SappSpline : MonoBehaviour
{
    public string splineName = "";

    public SappPlot plot;

    public bool refresh = false;

    public GameObject[] controlPoints;

    public Bezier[] beziers;

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < controlPoints.Length; i++)
        {
            if (Application.isPlaying)
                controlPoints[i].GetComponent<MeshRenderer>().enabled = false;
            else
            {
                controlPoints[i].GetComponent<MeshRenderer>().enabled =
                    true;
            }
        }

        beziers = new Bezier[controlPoints.Length - 1];

        Bezier first = new Bezier();
        first.b0 = controlPoints[0].transform.position;
        first.b1 = (controlPoints[1].transform.position - controlPoints[0].transform.position) * (1f / 3f) + controlPoints[0].transform.position;
        first.b2 = (controlPoints[1].transform.position - controlPoints[0].transform.position) * (2f / 3f) + controlPoints[0].transform.position;
        beziers[0] = first;

        Bezier last = new Bezier();
        last.b1 = (controlPoints[controlPoints.Length - 1].transform.position - controlPoints[controlPoints.Length - 2].transform.position) * (1f / 3f) + controlPoints[controlPoints.Length - 2].transform.position;
        last.b2 = (controlPoints[controlPoints.Length - 1].transform.position - controlPoints[controlPoints.Length - 2].transform.position) * (2f / 3f) + controlPoints[controlPoints.Length - 2].transform.position;
        last.b3 = controlPoints[controlPoints.Length - 1].transform.position;
        beziers[beziers.Length - 1] = last;

        for (int i = 1; i < beziers.Length - 1; i++)
        {
            beziers[i] = new Bezier();
            beziers[i].b1 = (controlPoints[i + 1].transform.position - controlPoints[i].transform.position) * (1f / 3f) + controlPoints[i].transform.position;
            beziers[i].b2 = (controlPoints[i + 1].transform.position - controlPoints[i].transform.position) * (2f / 3f) + controlPoints[i].transform.position;
        }

        for (int i = 0; i < beziers.Length - 1; i++)
        {
            beziers[i].b3 = (beziers[i + 1].b1 - beziers[i].b2) * 0.5f + beziers[i].b2;
            beziers[i + 1].b0 = beziers[i].b3;
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (refresh)
        {
            //refresh = false;
            Start();
        }

        if (Application.isPlaying == false && controlPoints.Length >= 4 && beziers.Length >= 3)
        {
            float pos = 0f;
            float posNext = 0f;
            for (int i = 0; i < 15 * controlPoints.Length; i++)
            {
                pos = i / ((float)15 * controlPoints.Length);
                posNext = (i + 1) / ((float)15 * controlPoints.Length);
                Debug.DrawLine(SplineAt(pos), SplineAt(posNext), Color.yellow, 0.2f);
            }
        }
    }

    public Vector3 SplineAt(float x)
    {
        int sectionsAmount = controlPoints.Length - 1;
        float xSized = x * sectionsAmount;
        int section = (int)xSized;
        section = section > sectionsAmount ? sectionsAmount : section;
        float secPart = xSized - section;

        if (section >= beziers.Length)
            return beziers[beziers.Length - 1].b3;
        return beziers[section].At(secPart);
    }

    public Vector3[] TangentAt(float x)
    {
        int sectionsAmount = controlPoints.Length;
        float xSized = x * sectionsAmount;
        int section = (int)xSized;
        section = section > sectionsAmount ? sectionsAmount : section;
        float secPart = xSized - section;

        if (section >= beziers.Length)
            return beziers[beziers.Length - 1].TangentAt(secPart);
        return beziers[section].TangentAt(secPart);
    }

    public float LengthOfBezier(float x)
    {
        int sectionsAmount = controlPoints.Length - 1;
        float xSized = x * sectionsAmount;
        int section = (int)xSized;
        section = section > sectionsAmount ? sectionsAmount : section;
        float secPart = xSized - section;

        if (section >= beziers.Length)
            return Vector3.Distance(beziers[beziers.Length - 1].b3, beziers[beziers.Length - 1].b0);
        return Vector3.Distance(beziers[section].b3, beziers[section].b0);
    }

    public float AngleBetweenSplines(float x)
    {
        int sectionsAmount = controlPoints.Length;
        float xSized = x * sectionsAmount;
        int section = (int)xSized;
        section = section > sectionsAmount ? sectionsAmount : section;
        float secPart = xSized - section;


        Vector3[] tangent0;
        Vector3[] tangent1;



        if (section >= beziers.Length)
        {
            tangent0 = beziers[beziers.Length - 1].TangentAt(0f);
            tangent1 = beziers[beziers.Length - 1].TangentAt(1f);
        }
        else
        {
            tangent0 = beziers[section].TangentAt(0f);
            tangent1 = beziers[section].TangentAt(1f);
        }

        Vector3 vec0 = tangent0[1] - tangent0[0];
        Vector3 vec1 = tangent1[1] - tangent1[0];

        return Vector3.Angle(vec0, vec1);
    }

    public GameObject[] ControlPoints
    {
        get
        {
            return controlPoints;
        }
        set
        {
            controlPoints = value;
        }
    }

    public void RefreshSpline()
    {
        Start();
    }
}


public class Bezier
{
    public Vector3 b0;
    public Vector3 b1;
    public Vector3 b2;
    public Vector3 b3;

    public Vector3 At(float val)
    {
        val = val > 1f ? 1f : (val < 0f ? 0f : val);

        Vector3 c0 = (b1 - b0) * val + b0;
        Vector3 c1 = (b2 - b1) * val + b1;
        Vector3 c2 = (b3 - b2) * val + b2;
        Vector3 d0 = (c1 - c0) * val + c0;
        Vector3 d1 = (c2 - c1) * val + c1;

        Vector3 e = (d1 - d0) * val + d0;

        return e;
    }

    public Vector3[] TangentAt(float val)
    {
        val = val > 1f ? 1f : (val < 0f ? 0f : val);


        Vector3 c0 = (b1 - b0) * val + b0;
        Vector3 c1 = (b2 - b1) * val + b1;
        Vector3 c2 = (b3 - b2) * val + b2;
        Vector3 d0 = (c1 - c0) * val + c0;
        Vector3 d1 = (c2 - c1) * val + c1;

        return new Vector3[] { d0, d1 };
    }
}

public class Bezier<T>
{
    public T b0;
    public T b1;
    public T b2;
    public T b3;

    public T At(float val)
    {
        val = val > 1f ? 1f : (val < 0f ? 0f : val);
        
        T c0 = TUtils<T>.Additate(TUtils<T>.Scalar(TUtils<T>.Subtract(b1, b0), val), b0);
        T c1 = TUtils<T>.Additate(TUtils<T>.Scalar(TUtils<T>.Subtract(b2, b1), val), b1);
        T c2 = TUtils<T>.Additate(TUtils<T>.Scalar(TUtils<T>.Subtract(b3, b2), val), b2);
        T d0 = TUtils<T>.Additate(TUtils<T>.Scalar(TUtils<T>.Subtract(c1, c0), val), c0);
        T d1 = TUtils<T>.Additate(TUtils<T>.Scalar(TUtils<T>.Subtract(c2, c1), val), c1);

        T e = TUtils<T>.Additate(TUtils<T>.Scalar(TUtils<T>.Subtract(d1, d0), val), d0);

        return e;
    }

    public T[] TangentAt(float val)
    {
        val = val > 1f ? 1f : (val < 0f ? 0f : val);

        if (val == 0f)
        {
            return new T[] { b0, b1 };
        }
        else if (val == 1f)
        {
            return new T[] { b2, b3 };
        }


        T c0 = TUtils<T>.Additate(TUtils<T>.Scalar(TUtils<T>.Subtract(b1, b0), val), b0);
        T c1 = TUtils<T>.Additate(TUtils<T>.Scalar(TUtils<T>.Subtract(b2, b1), val), b1);
        T c2 = TUtils<T>.Additate(TUtils<T>.Scalar(TUtils<T>.Subtract(b3, b2), val), b2);
        T d0 = TUtils<T>.Additate(TUtils<T>.Scalar(TUtils<T>.Subtract(c1, c0), val), c0);
        T d1 = TUtils<T>.Additate(TUtils<T>.Scalar(TUtils<T>.Subtract(c2, c1), val), c1);

        return new T[] { d0, d1 };
    }
}
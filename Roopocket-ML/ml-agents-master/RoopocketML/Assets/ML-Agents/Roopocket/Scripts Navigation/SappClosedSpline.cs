using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SappClosedSpline : MonoBehaviour
{
    public string splineName = "";

    public SappPlot plot;

    public bool refresh = false;

    public GameObject[] controlPoints;

    public ClosedSpline<Vector3> clSpline;

    // Use this for initialization
    void Start()
    {
        Vector3[] cpPosses = new Vector3[controlPoints.Length];
        for (int i = 0; i < controlPoints.Length; i++)
        {
            cpPosses[i] = controlPoints[i].transform.position;
        }

        clSpline = new ClosedSpline<Vector3>(cpPosses);

        for (int i = 0; i < controlPoints.Length; i++)
        {
            if (Application.isPlaying)
                controlPoints[i].GetComponent<MeshRenderer>().enabled = false;
            else
            {
                controlPoints[i].GetComponent<MeshRenderer>().enabled = true;
            }
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

        if (controlPoints.Length >= 4)
        {
            float pos = 0f;
            float posNext = 0f;
            for (int i = 0; i < 15 * controlPoints.Length; i++)
            {
                pos = i / ((float)15 * controlPoints.Length);
                posNext = (i + 1) / ((float)15 * controlPoints.Length);
                Debug.DrawLine(SplineAt(pos), SplineAt(posNext), Color.red, 0.2f);
            }
        }
    }

    public Vector3 SplineAt(float x)
    {
        if (clSpline == null)
        {
            Start();
        }
        return clSpline.SplineAt(x);
    }

    public Vector3[] TangentAt(float x)
    {
        return clSpline.TangentAt(x);
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


public class TensorProductPlane
{
    private float[,] controlPoints;
    private Vector3 position;
    private float scale;

    public TensorProductPlane(Vector3 position, float scale, float[,] controlPointsParam)
    {
        controlPoints = controlPointsParam;
        this.position = position;
        this.scale = scale;

        for (int x = 1; x < controlPoints.GetLength(0) - 1; x++)
        {
            for (int y = 1; y < controlPoints.GetLength(1) - 1; y++)
            {
                if (x % 3 != 0 || y % 3 != 0)
                {
                    if (x % 3 == 0)
                    {
                        controlPoints[x, y] = (controlPoints[x - 1, y] + controlPoints[x + 1, y]) * 0.5f;
                    }
                    else if (y % 3 == 0)
                    {
                        controlPoints[x, y] = (controlPoints[x, y - 1] + controlPoints[x, y + 1]) * 0.5f;
                    }
                }
            }
        }

        for (int x = 1; x < controlPoints.GetLength(0) - 1; x++)
        {
            for (int y = 1; y < controlPoints.GetLength(1) - 1; y++)
            {
                if (x % 3 == 0 && y % 3 == 0)
                {
                    controlPoints[x, y] = ((controlPoints[x - 1, y] + controlPoints[x + 1, y]) * 0.5f + (controlPoints[x, y - 1] + controlPoints[x, y + 1]) * 0.5f) * 0.5f;
                }
            }
        }
    }

    public float[,] Controlpoints
    {
        get
        {
            return controlPoints;
        }
    }

    public float At(Vector3 pos)
    {
        return At(pos.x, pos.z);
    }

    public float At(Vector2 pos)
    {
        return At(pos.x, pos.y);
    }

    public float At(float x, float z)
    {
        float transX = x - position.x;
        float transZ = z - position.z;

        transX = transX / scale;
        transZ = transZ / scale;

        int intX = (int)transX;
        int intZ = (int)transZ;

        if (intX >= 0 && intX < controlPoints.GetLength(0) && intZ >= 0 && intZ < controlPoints.GetLength(1))
        {
            int multiplier = 3;

            int quaterX = intX / multiplier;
            int quaterZ = intZ / multiplier;

            float u = ((transX / ((float)multiplier)) - quaterX);
            float v = ((transZ / ((float)multiplier)) - quaterZ);

            float c00 = (((controlPoints[quaterX * multiplier + 1, quaterZ * multiplier + 1] - controlPoints[quaterX * multiplier, quaterZ * multiplier + 1]) * u + controlPoints[quaterX * multiplier, quaterZ * multiplier + 1])
                - ((controlPoints[quaterX * multiplier + 1, quaterZ * multiplier] - controlPoints[quaterX * multiplier, quaterZ * multiplier]) * u + controlPoints[quaterX * multiplier, quaterZ * multiplier]))
                * v
                + ((controlPoints[quaterX * multiplier + 1, quaterZ * multiplier] - controlPoints[quaterX * multiplier, quaterZ * multiplier]) * u + controlPoints[quaterX * multiplier, quaterZ * multiplier]);
            float c10 = (((controlPoints[quaterX * multiplier + 2, quaterZ * multiplier + 1] - controlPoints[quaterX * multiplier + 1, quaterZ * multiplier + 1]) * u + controlPoints[quaterX * multiplier + 1, quaterZ * multiplier + 1])
                - ((controlPoints[quaterX * multiplier + 2, quaterZ * multiplier] - controlPoints[quaterX * multiplier + 1, quaterZ * multiplier]) * u + controlPoints[quaterX * multiplier + 1, quaterZ * multiplier]))
                * v
                + ((controlPoints[quaterX * multiplier + 2, quaterZ * multiplier] - controlPoints[quaterX * multiplier + 1, quaterZ * multiplier]) * u + controlPoints[quaterX * multiplier + 1, quaterZ * multiplier]);
            float c20 = (((controlPoints[quaterX * multiplier + 3, quaterZ * multiplier + 1] - controlPoints[quaterX * multiplier + 2, quaterZ * multiplier + 1]) * u + controlPoints[quaterX * multiplier + 2, quaterZ * multiplier + 1])
                - ((controlPoints[quaterX * multiplier + 3, quaterZ * multiplier] - controlPoints[quaterX * multiplier + 2, quaterZ * multiplier]) * u + controlPoints[quaterX * multiplier + 2, quaterZ * multiplier]))
                * v
                + ((controlPoints[quaterX * multiplier + 3, quaterZ * multiplier] - controlPoints[quaterX * multiplier + 2, quaterZ * multiplier]) * u + controlPoints[quaterX * multiplier + 2, quaterZ * multiplier]);

            float c01 = (((controlPoints[quaterX * multiplier + 1, quaterZ * multiplier + 2] - controlPoints[quaterX * multiplier, quaterZ * multiplier + 2]) * u + controlPoints[quaterX * multiplier, quaterZ * multiplier + 2])
                - ((controlPoints[quaterX * multiplier + 1, quaterZ * multiplier + 1] - controlPoints[quaterX * multiplier, quaterZ * multiplier + 1]) * u + controlPoints[quaterX * multiplier, quaterZ * multiplier + 1]))
                * v
                + ((controlPoints[quaterX * multiplier + 1, quaterZ * multiplier + 1] - controlPoints[quaterX * multiplier, quaterZ * multiplier + 1]) * u + controlPoints[quaterX * multiplier, quaterZ * multiplier + 1]);
            float c11 = (((controlPoints[quaterX * multiplier + 2, quaterZ * multiplier + 2] - controlPoints[quaterX * multiplier + 1, quaterZ * multiplier + 2]) * u + controlPoints[quaterX * multiplier + 1, quaterZ * multiplier + 2])
                - ((controlPoints[quaterX * multiplier + 2, quaterZ * multiplier + 1] - controlPoints[quaterX * multiplier + 1, quaterZ * multiplier + 1]) * u + controlPoints[quaterX * multiplier + 1, quaterZ * multiplier + 1]))
                * v
                + ((controlPoints[quaterX * multiplier + 2, quaterZ * multiplier + 1] - controlPoints[quaterX * multiplier + 1, quaterZ * multiplier + 1]) * u + controlPoints[quaterX * multiplier + 1, quaterZ * multiplier + 1]);
            float c21 = (((controlPoints[quaterX * multiplier + 3, quaterZ * multiplier + 2] - controlPoints[quaterX * multiplier + 2, quaterZ * multiplier + 2]) * u + controlPoints[quaterX * multiplier + 2, quaterZ * multiplier + 2])
                - ((controlPoints[quaterX * multiplier + 3, quaterZ * multiplier + 1] - controlPoints[quaterX * multiplier + 2, quaterZ * multiplier + 1]) * u + controlPoints[quaterX * multiplier + 2, quaterZ * multiplier + 1]))
                * v
                + ((controlPoints[quaterX * multiplier + 3, quaterZ * multiplier + 1] - controlPoints[quaterX * multiplier + 2, quaterZ * multiplier + 1]) * u + controlPoints[quaterX * multiplier + 2, quaterZ * multiplier + 1]);

            float c02 = (((controlPoints[quaterX * multiplier + 1, quaterZ * multiplier + 3] - controlPoints[quaterX * multiplier, quaterZ * multiplier + 3]) * u + controlPoints[quaterX * multiplier, quaterZ * multiplier + 3])
                - ((controlPoints[quaterX * multiplier + 1, quaterZ * multiplier + 2] - controlPoints[quaterX * multiplier, quaterZ * multiplier + 2]) * u + controlPoints[quaterX * multiplier, quaterZ * multiplier + 2]))
                * v
                + ((controlPoints[quaterX * multiplier + 1, quaterZ * multiplier + 2] - controlPoints[quaterX * multiplier, quaterZ * multiplier + 2]) * u + controlPoints[quaterX * multiplier, quaterZ * multiplier + 2]);
            float c12 = (((controlPoints[quaterX * multiplier + 2, quaterZ * multiplier + 3] - controlPoints[quaterX * multiplier + 1, quaterZ * multiplier + 3]) * u + controlPoints[quaterX * multiplier + 1, quaterZ * multiplier + 3])
                - ((controlPoints[quaterX * multiplier + 2, quaterZ * multiplier + 2] - controlPoints[quaterX * multiplier + 1, quaterZ * multiplier + 2]) * u + controlPoints[quaterX * multiplier + 1, quaterZ * multiplier + 2]))
                * v
                + ((controlPoints[quaterX * multiplier + 2, quaterZ * multiplier + 2] - controlPoints[quaterX * multiplier + 1, quaterZ * multiplier + 2]) * u + controlPoints[quaterX * multiplier + 1, quaterZ * multiplier + 2]);
            float c22 = (((controlPoints[quaterX * multiplier + 3, quaterZ * multiplier + 3] - controlPoints[quaterX * multiplier + 2, quaterZ * multiplier + 3]) * u + controlPoints[quaterX * multiplier + 2, quaterZ * multiplier + 3])
                - ((controlPoints[quaterX * multiplier + 3, quaterZ * multiplier + 2] - controlPoints[quaterX * multiplier + 2, quaterZ * multiplier + 2]) * u + controlPoints[quaterX * multiplier + 2, quaterZ * multiplier + 2]))
                * v
                + ((controlPoints[quaterX * multiplier + 3, quaterZ * multiplier + 2] - controlPoints[quaterX * multiplier + 2, quaterZ * multiplier + 2]) * u + controlPoints[quaterX * multiplier + 2, quaterZ * multiplier + 2]);


            float d00 = (((c11 - c01) * u + c01) - ((c10 - c00) * u + c00)) * v + ((c10 - c00) * u + c00);
            float d10 = (((c21 - c11) * u + c11) - ((c20 - c10) * u + c10)) * v + ((c20 - c10) * u + c10);
            float d01 = (((c12 - c02) * u + c02) - ((c11 - c01) * u + c01)) * v + ((c11 - c01) * u + c01);
            float d11 = (((c22 - c12) * u + c12) - ((c21 - c11) * u + c11)) * v + ((c21 - c11) * u + c11);

            float eValue = (((d11 - d01) * u + d01) - ((d10 - d00) * u + d00)) * v + ((d10 - d00) * u + d00);

            return eValue;
        }
        else
        {
            return 0f;
        }
    }
}


public class BezierPlane
{
    private float[,] controlPoints;

    private BezierPlanepart[,] parts;

    private Vector3 position;
    private float scale;

    public BezierPlane(Vector3 position, float scale, float[,] controlPoints)
    {
        this.controlPoints = controlPoints;
        this.position = position;
        this.scale = scale;

        parts = new BezierPlanepart[controlPoints.GetLength(0) - 1, controlPoints.GetLength(1) - 1];

        // Calculate borders, then the remaining parts
        // ...

        for (int x = 0; x < parts.GetLength(0); x++)
        {
            for (int y = 0; y < parts.GetLength(1); y++)
            {
                parts[x, y] = new BezierPlanepart();
                parts[x, y].B00 = controlPoints[x, y];
                parts[x, y].B03 = controlPoints[x, y + 1];
                parts[x, y].B30 = controlPoints[x + 1, y];
                parts[x, y].B33 = controlPoints[x + 1, y + 1];
            }
        }


        for (int x = 0; x < parts.GetLength(0); x++)
        {
            for (int y = 0; y < parts.GetLength(1); y++)
            {
                float prevX0 = 0f;
                float prevX3 = 0f;
                float nextX0 = 0f;
                float nextX3 = 0f;
                float prevY0 = 0f;
                float prevY3 = 0f;
                float nextY0 = 0f;
                float nextY3 = 0f;

                if (x == 0)
                {
                    prevX0 = parts[x, y].B00;
                    prevX3 = parts[x, y].B30;
                }
                else
                {
                    prevX0 = parts[x - 1, y].B03;
                    prevX3 = parts[x - 1, y].B33;
                }
                if (x == parts.GetLength(0) - 1)
                {
                    nextX0 = parts[x, y].B03;
                    nextX3 = parts[x, y].B33;
                }
                else
                {
                    nextX0 = parts[x + 1, y].B00;
                    nextX3 = parts[x + 1, y].B30;
                }

                if (y == 0)
                {
                    prevY0 = parts[x, y].B00;
                    prevY3 = parts[x, y].B03;
                }
                else
                {
                    prevY0 = parts[x, y - 1].B30;
                    prevY3 = parts[x, y - 1].B33;
                }
                if (y == parts.GetLength(1) - 1)
                {
                    nextY0 = parts[x, y].B30;
                    nextY3 = parts[x, y].B33;
                }
                else
                {
                    nextY0 = parts[x, y + 1].B00;
                    nextY3 = parts[x, y + 1].B03;
                }


                parts[x, y].B01 = ((parts[x, y].B03 - prevX0) / 8f) + parts[x, y].B00;
                parts[x, y].B31 = ((parts[x, y].B33 - prevX3) / 8f) + parts[x, y].B30;
                parts[x, y].B02 = ((nextX0 - parts[x, y].B00) / 8f) + parts[x, y].B03;
                parts[x, y].B32 = ((nextX3 - parts[x, y].B30) / 8f) + parts[x, y].B33;

                parts[x, y].B10 = ((parts[x, y].B30 - prevY0) / 8f) + parts[x, y].B00;
                parts[x, y].B13 = ((parts[x, y].B33 - prevY3) / 8f) + parts[x, y].B03;
                parts[x, y].B20 = ((nextY0 - parts[x, y].B00) / 8f) + parts[x, y].B30;
                parts[x, y].B23 = ((nextY3 - parts[x, y].B03) / 8f) + parts[x, y].B33;
            }
        }

    }

    public float At(float x, float z)
    {
        float relX = x - position.x;
        float relZ = z - position.z;

        float scX = x / scale;
        float scZ = z / scale;

        float u = scX - ((int)scX);
        float v = scZ - ((int)scZ);

        return 0f;
    }

    public float At(Vector2 posXZ)
    {
        return At(posXZ.x, posXZ.y);
    }

    public float At(Vector3 pos)
    {
        return At(pos.x, pos.z);
    }

}

public class BezierPlanepart
{
    private float[,] bs;

    public BezierPlanepart()
    {
        bs = new float[4, 4];
    }

    public float[,] BS
    {
        get
        {
            return bs;
        }
        set
        {
            bs = value;
        }
    }

    public float B00 { get { return bs[0, 0]; } set { bs[0, 0] = value; } }
    public float B01 { get { return bs[0, 1]; } set { bs[0, 1] = value; } }
    public float B02 { get { return bs[0, 2]; } set { bs[0, 2] = value; } }
    public float B03 { get { return bs[0, 3]; } set { bs[0, 3] = value; } }
    public float B10 { get { return bs[1, 0]; } set { bs[1, 0] = value; } }
    public float B11 { get { return bs[1, 1]; } set { bs[1, 1] = value; } }
    public float B12 { get { return bs[1, 2]; } set { bs[1, 2] = value; } }
    public float B13 { get { return bs[1, 3]; } set { bs[1, 3] = value; } }
    public float B20 { get { return bs[2, 0]; } set { bs[2, 0] = value; } }
    public float B21 { get { return bs[2, 1]; } set { bs[2, 1] = value; } }
    public float B22 { get { return bs[2, 2]; } set { bs[2, 2] = value; } }
    public float B23 { get { return bs[2, 3]; } set { bs[2, 3] = value; } }
    public float B30 { get { return bs[3, 0]; } set { bs[3, 0] = value; } }
    public float B31 { get { return bs[3, 1]; } set { bs[3, 1] = value; } }
    public float B32 { get { return bs[3, 2]; } set { bs[3, 2] = value; } }
    public float B33 { get { return bs[3, 3]; } set { bs[3, 3] = value; } }
}



public class ClosedSpline<T>
{
    public T[] controlPoints;

    public Bezier<T>[] beziers;

    public ClosedSpline(T[] controlPointsP)
    {
        controlPoints = controlPointsP;

        fillBeziers();
    }

    private void fillBeziers()
    {
        beziers = new Bezier<T>[controlPoints.Length];

        Bezier<T> first = new Bezier<T>();
        first.b0 = controlPoints[0];
        first.b1 = TUtils<T>.Additate(TUtils<T>.Scalar((TUtils<T>.Subtract(controlPoints[1], controlPoints[0])), (1f / 3f)), controlPoints[0]);
        first.b2 = TUtils<T>.Additate(TUtils<T>.Scalar((TUtils<T>.Subtract(controlPoints[1], controlPoints[0])), (2f / 3f)), controlPoints[0]);
        beziers[0] = first;

        Bezier<T> last = new Bezier<T>();
        last.b1 = TUtils<T>.Additate(TUtils<T>.Scalar((TUtils<T>.Subtract(controlPoints[0], controlPoints[controlPoints.Length - 1])), (1f / 3f)), controlPoints[controlPoints.Length - 1]);
        last.b2 = TUtils<T>.Additate(TUtils<T>.Scalar((TUtils<T>.Subtract(controlPoints[0], controlPoints[controlPoints.Length - 1])), (2f / 3f)), controlPoints[controlPoints.Length - 1]);
        last.b3 = controlPoints[0];
        beziers[beziers.Length - 1] = last;

        for (int i = 1; i < beziers.Length - 1; i++)
        {
            beziers[i] = new Bezier<T>();
            beziers[i].b1 = TUtils<T>.Additate(TUtils<T>.Scalar((TUtils<T>.Subtract(controlPoints[i + 1], controlPoints[i])), (1f / 3f)), controlPoints[i]);
            beziers[i].b2 = TUtils<T>.Additate(TUtils<T>.Scalar((TUtils<T>.Subtract(controlPoints[i + 1], controlPoints[i])), (2f / 3f)), controlPoints[i]);
        }

        beziers[beziers.Length - 1].b3 = TUtils<T>.Additate(TUtils<T>.Scalar((TUtils<T>.Subtract(beziers[0].b1, beziers[beziers.Length - 1].b2)), 0.5f), beziers[beziers.Length - 1].b2);
        beziers[0].b0 = beziers[beziers.Length - 1].b3;

        for (int i = 0; i < beziers.Length - 1; i++)
        {
            beziers[i].b3 = TUtils<T>.Additate(TUtils<T>.Scalar((TUtils<T>.Subtract(beziers[i + 1].b1, beziers[i].b2)), 0.5f), beziers[i].b2);
            beziers[i + 1].b0 = beziers[i].b3;
        }
    }

    public int ControlPointsAmount
    {
        get
        {
            return controlPoints.Length;
        }
    }

    public void SetControlPoint(int index, T controlPoint)
    {
        if (index >= 0 && index < ControlPointsAmount)
        {
            controlPoints[index] = controlPoint;

            fillBeziers();
        }
    }

    public T GetControlPoint(int index)
    {
        if (index >= 0 && index < ControlPointsAmount)
        {
            return controlPoints[index];
        }

        return controlPoints[0];
    }

    public T SplineAt(float x)
    {
        int sectionsAmount = controlPoints.Length;
        float xSized = x * sectionsAmount;
        int section = (int)xSized;
        section = section > sectionsAmount ? sectionsAmount : section;
        float secPart = xSized - section;

        if (section >= beziers.Length)
            return beziers[beziers.Length - 1].b3;
        return beziers[section].At(secPart);
    }

    public T[] TangentAt(float x)
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
}

public class TUtils<T>
{
    public static T Subtract(T a, T b)
    {
        if (typeof(float).IsAssignableFrom(typeof(T)))
        {
            float aC = (float)System.Convert.ChangeType(a, typeof(float));
            float bC = (float)System.Convert.ChangeType(b, typeof(float));

            return (T)System.Convert.ChangeType((aC - bC), typeof(T));
        }
        else if (typeof(Vector3).IsAssignableFrom(typeof(T)))
        {
            Vector3 aC = (Vector3)System.Convert.ChangeType(a, typeof(Vector3));
            Vector3 bC = (Vector3)System.Convert.ChangeType(b, typeof(Vector3));

            return (T)System.Convert.ChangeType((aC - bC), typeof(T));
        }

        return a;
    }

    public static T Additate(T a, T b)
    {
        if (typeof(float).IsAssignableFrom(typeof(T)))
        {
            float aC = (float)System.Convert.ChangeType(a, typeof(float));
            float bC = (float)System.Convert.ChangeType(b, typeof(float));

            return (T)System.Convert.ChangeType((aC + bC), typeof(T));
        }
        else if (typeof(Vector3).IsAssignableFrom(typeof(T)))
        {
            Vector3 aC = (Vector3)System.Convert.ChangeType(a, typeof(Vector3));
            Vector3 bC = (Vector3)System.Convert.ChangeType(b, typeof(Vector3));

            return (T)System.Convert.ChangeType((aC + bC), typeof(T));
        }

        return a;
    }

    public static T Scalar(T a, float s)
    {
        if (typeof(float).IsAssignableFrom(typeof(T)))
        {
            float aC = (float)System.Convert.ChangeType(a, typeof(float));

            return (T)System.Convert.ChangeType((aC * s), typeof(T));
        }
        else if (typeof(Vector3).IsAssignableFrom(typeof(T)))
        {
            Vector3 aC = (Vector3)System.Convert.ChangeType(a, typeof(Vector3));

            return (T)System.Convert.ChangeType((aC * s), typeof(T));
        }

        return a;
    }
}
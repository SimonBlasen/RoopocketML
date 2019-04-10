using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SappPlot : MonoBehaviour
{

    public bool refresh = false;

    private List<Vector2> points = new List<Vector2>();

	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
		if (refresh)
        {
            refresh = false;
            Draw();
        }
	}

    public void Draw()
    {
        List<Vector2> sorted = new List<Vector2>();

        float x = float.MinValue;
        for (int i = 0; i < points.Count; i++)
        {
            float localMin = float.MaxValue;
            int curIndex = 0;
            for (int j = 0; j < points.Count; j++)
            {
                if (points[j].x < localMin && points[i].x > x)
                {
                    localMin = points[j].x;
                    curIndex = j;
                }
            }

            sorted.Add(new Vector2(points[curIndex].x, points[curIndex].y));

            x = localMin;
        }

        for (int i = 0; i < sorted.Count - 1; i++)
        {
            Debug.DrawLine(new Vector3(sorted[i].x, 0f, sorted[i].y) + transform.position, new Vector3(sorted[i + 1].x, 0f, sorted[i + 1].y) + transform.position, Color.red, 0.2f);
        }
    }

    public void AddPoint(float x, float y)
    {
        points.Add(new Vector2(x, y));
    }
}

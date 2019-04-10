using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RoundPlacer : MonoBehaviour {

    public Transform[] cubes;
    public Vector3 mid;
    public float radius;


    public bool place = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (place)
        {
            place = false;

            for (int i = 0; i < cubes.Length; i++)
            {
                float angle = ((float)i) / (cubes.Length - 1f);

                cubes[i].position = new Vector3(Mathf.Sin(angle * Mathf.PI * 0.5f) * radius, 0f, Mathf.Cos(angle * Mathf.PI * 0.5f) * radius);
                cubes[i].rotation = Quaternion.Euler(0f, angle * 90f, 0f);
            }
        }
	}
}

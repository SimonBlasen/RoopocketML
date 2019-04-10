using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarsSpawner : MonoBehaviour {

    [SerializeField]
    private GameObject[] starsPrefabs;
    [SerializeField]
    private Vector3 midPoint = Vector3.zero;
    [SerializeField]
    private int starsAmount = 100;
    [SerializeField]
    private float minDistance = 50f;
    [SerializeField]
    private float maxDistance = 500f;
    [SerializeField]
    private float scaleFactor = 1f;


    // Use this for initialization
    void Start ()
    {
		for (int i = 0; i < starsAmount; i++)
        {
            /*float alpha = Random.Range(0f, 360f);

            float u = Mathf.Cos(alpha);
            float x = Mathf.Sqrt(1f - u * u) * Mathf.Cos(alpha);
            float y = Mathf.Sqrt(1f - u * u) * Mathf.Sin(alpha);
            float z = u;*/

            float x1 = Random.Range(-1f, 1f);
            float x2 = Random.Range(-1f, 1f);

            while (x1 * x1 + x2 * x2 >= 1f)
            {
                x1 = Random.Range(-1f, 1f);
                x2 = Random.Range(-1f, 1f);
            }

            float x = 2f * x1 * Mathf.Sqrt(1f - x1 * x1 - x2 * x2);
            float y = 2f * x2 * Mathf.Sqrt(1f - x1 * x1 - x2 * x2);
            float z = 1f - 2f * (x1 * x1 + x2 * x2);


            float distance = Random.Range(minDistance, maxDistance);

            int starIndex = Random.Range(0, starsPrefabs.Length);

            GameObject instStar = Instantiate(starsPrefabs[starIndex], transform);
            instStar.transform.position = ((new Vector3(x, y, z) ).normalized * distance) + midPoint;

            instStar.transform.localScale = (new Vector3(1f, 1f, 1f)) * distance * scaleFactor;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

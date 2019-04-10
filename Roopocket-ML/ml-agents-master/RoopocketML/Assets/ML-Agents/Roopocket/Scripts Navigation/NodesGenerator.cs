using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NodesGenerator : MonoBehaviour
{

    [Header("Execute In Editmode")]
    [SerializeField]
    private bool generateGrid = false;

    [Space]

    [Header("Settings")]
    [SerializeField]
    private float gridMaxDistance = 5f;
    [SerializeField]
    private float minDistanceObstacles = 3f;
    [SerializeField]
    private Vector3 expansionDirections = Vector3.zero;
    [SerializeField]
    private float placingDistance = 3f;

    [Header("References")]
    [SerializeField]
    private Transform graphParent;
    [SerializeField]
    private Transform[] landingNodes;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject spherePrefab;
    [SerializeField]
    private GameObject spherePrefabNoCollider;

    private List<GameObject> generatedGO = new List<GameObject>();
    private List<GameObject> generatedSpheres = new List<GameObject>();

    // Use this for initialization
    void Start ()
    {
        
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (generateGrid)
        {
            generatedGO.Clear();
            generatedGO = new List<GameObject>();
            generatedSpheres.Clear();
            generatedSpheres = new List<GameObject>();

            generateGrid = false;
            float stepX = (1f / transform.localScale.x) * placingDistance;
            float stepY = (1f / transform.localScale.y) * placingDistance;

            Vector3 curPos = new Vector3(-0.5f, -0.5f, 0f);
            while (curPos.y <= 0.5f)
            {
                while (curPos.x <= 0.5f)
                {
                    GameObject newGo = Instantiate(spherePrefabNoCollider, transform);
                    newGo.transform.localPosition = curPos;

                    generatedGO.Add(newGo);

                    curPos += new Vector3(stepX, 0f, 0f);
                }
                curPos = new Vector3(-0.5f, curPos.y + stepY, 0f);
            }

            int indexWhenLanding = generatedGO.Count;

            for (int i = 0; i < landingNodes.Length; i++)
            {
                GameObject newGo = Instantiate(spherePrefabNoCollider, transform);
                newGo.transform.position = landingNodes[i].position;

                generatedGO.Add(newGo);
            }

            List<int> indcsFrom = new List<int>();
            List<int> indcsTo = new List<int>();

            for (int i = 0; i < generatedGO.Count; i++)
            {
                for (int j = i + 1; j < generatedGO.Count; j++)
                {
                    if (j != i)
                    {
                        if (Vector3.Distance(generatedGO[i].transform.position, generatedGO[j].transform.position) <= gridMaxDistance)
                        {
                            RaycastHit[] hit = Physics.RaycastAll(new Ray(generatedGO[i].transform.position, generatedGO[j].transform.position - generatedGO[i].transform.position), Vector3.Distance(generatedGO[i].transform.position, generatedGO[j].transform.position));

                            bool blocked = false;
                            for (int k = 0; k < hit.Length; k++)
                            {
                                if (hit[k].transform.GetComponent<RPNavNode>() == null)
                                {
                                    blocked = true;
                                    break;
                                }
                            }

                            if (!blocked)
                            {
                                indcsFrom.Add(i);
                                indcsTo.Add(j);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < generatedGO.Count; i++)
            {
                if (i < indexWhenLanding)
                {
                    GameObject newGo = Instantiate(spherePrefab, transform);
                    newGo.transform.position = generatedGO[i].transform.position;

                    generatedSpheres.Add(newGo);
                }
                else
                {
                    generatedSpheres.Add(landingNodes[i - indexWhenLanding].gameObject);
                }
            }

            for (int i = 0; i < indcsFrom.Count; i++)
            {
                Transform[] oldNeighbours = generatedSpheres[indcsFrom[i]].GetComponent<RPNavNode>().neighbours;
                Transform[] newNeighbours = new Transform[oldNeighbours.Length + 1];
                for (int k = 0; k < oldNeighbours.Length; k++)
                {
                    newNeighbours[k] = oldNeighbours[k];
                }
                newNeighbours[newNeighbours.Length - 1] = generatedSpheres[indcsTo[i]].transform;
                generatedSpheres[indcsFrom[i]].GetComponent<RPNavNode>().neighbours = newNeighbours;
            }

            for (int i = 0; i < generatedGO.Count; i++)
            {
                generatedGO[i].transform.parent = graphParent;
            }
        }
	}

    public bool GeneratedAll
    {
        get;
        protected set;
    }
}

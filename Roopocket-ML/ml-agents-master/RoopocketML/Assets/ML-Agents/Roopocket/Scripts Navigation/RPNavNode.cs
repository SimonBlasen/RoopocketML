using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RPNavNode : MonoBehaviour
{
    public string nodeName = "";
    public Transform[] neighbours;
    public GameObject landingPlatform;

    public GameObject prefabCollider;

    private string oldName = "";

    private RPNavGraph navGraph = null;

    // Use this for initialization
    void Start ()
    {
		if (navGraph == null)
        {
            navGraph = RPNavGraph.Inst;
            navGraph.RegisterNode(this);
        }
	}

    private bool changedColorYet = false;
    private int oldNeighbours = 0;
	
	// Update is called once per frame
	void Update ()
    {
        if (Application.isEditor && oldNeighbours != neighbours.Length)
        {
            oldNeighbours = neighbours.Length;
            if (navGraph == null)
            {
                navGraph = RPNavGraph.Inst;
            }
            navGraph.refresh = true;
        }

        if (Application.isEditor && nodeName != oldName)
        {
            oldName = nodeName;
            if (navGraph == null)
            {
                navGraph = RPNavGraph.Inst;
            }

            if (nodeName.Length > 0)
            {
                Material cop = new Material(GetComponent<MeshRenderer>().sharedMaterial);
                cop.color = navGraph.endNodeColor;
                GetComponent<MeshRenderer>().sharedMaterial = cop;
            }
            else
            {
                Material cop = new Material(GetComponent<MeshRenderer>().sharedMaterial);
                cop.color = navGraph.normalNodeColor;
                GetComponent<MeshRenderer>().sharedMaterial = cop;
            }
            navGraph.RegisterNode(this);
        }

        if (Application.isEditor && changedColorYet == false)
        {
            changedColorYet = true;
            if (navGraph == null)
            {
                navGraph = RPNavGraph.Inst;
            }

            if (nodeName.Length > 0)
            {
                Material cop = new Material(GetComponent<MeshRenderer>().sharedMaterial);
                cop.color = navGraph.endNodeColor;
                GetComponent<MeshRenderer>().sharedMaterial = cop;
            }
            else
            {
                Material cop = new Material(GetComponent<MeshRenderer>().sharedMaterial);
                cop.color = navGraph.normalNodeColor;
                GetComponent<MeshRenderer>().sharedMaterial = cop;
            }
            navGraph.RegisterNode(this);
        }
    }

    private List<GameObject> nodeColliders = new List<GameObject>();

    public void BuildVoronoiMeshes()
    {
        if (Application.isPlaying)
        {

            if (nodeName.Length > 0)
            {
                GameObject newGo = Instantiate(prefabCollider);
                newGo.transform.position = transform.position;

                newGo.transform.localScale = new Vector3(1f, 1f, 1f);


                newGo.GetComponent<NodeCollider>().NavNode = this;

                nodeColliders.Add(newGo);
            }
            else
            {
                for (int i = 0; i < nodeColliders.Count; i++)
                {
                    Destroy(nodeColliders[i]);
                }
                nodeColliders.Clear();
                nodeColliders = new List<GameObject>();


                /*BoxCollider[] children = GetComponentsInChildren<BoxCollider>();
                for (int i = 0; i < children.Length; i++)
                {
                    if (children[i] != null && children[i].gameObject != null)
                    {
                        DestroyImmediate(children[i].gameObject);
                    }
                }*/

                List<Edge<RPNavNode>> outEdges = navGraph.Graph.GetOutcommingEdges(navGraph.Graph.GetNode(this));

                //Debug.Log("Out Edges: " + outEdges.Count);

                for (int i = 0; i < outEdges.Count; i++)
                {
                    RPNavNode node = outEdges[i].To.ID;

                    float distance = Vector3.Distance(transform.position, node.transform.position);
                    Vector3 dirToNode = node.transform.position - transform.position;

                    GameObject newGo = Instantiate(prefabCollider);
                    newGo.transform.position = transform.position + dirToNode * 0.25f;
                    newGo.transform.forward = dirToNode;
                    //newGo.transform.up = Vector3.Cross(Vector3.Cross(dirToNode, Vector3.up), dirToNode);

                    float angle = Vector3.Angle(new Vector3(0f, 0f, 1f), dirToNode);
                    if (Vector3.Angle(new Vector3(1f, 0f, 0f), dirToNode) > 90f)
                    {
                        angle = 360f - angle;
                    }
                    angle = angle * Mathf.PI / 180f + 90f;

                    float angleY = angle;
                    float angleX = Vector3.Angle(new Vector3(0f, 0f, 1f), dirToNode);
                    if (Vector3.Angle(new Vector3(1f, 0f, 0f), dirToNode) > 90f)
                    {
                        angleX = 360f - angleX;
                    }
                    angleX = angleX * Mathf.PI / 180f;

                    angle = 90f;

                    float width = 3f;
                    float height = 20f;

                    newGo.transform.localScale = new Vector3(width, height, distance * 0.5f);

                    /*if (Vector3.Angle(dirToNode, newGo.transform.up) < 45f)
                    {
                        newGo.transform.localScale = new Vector3(height * Mathf.Cos(angle) + width * Mathf.Sin(angle), distance * 0.5f, width * Mathf.Cos(angle) + height * Mathf.Sin(angle));
                    }
                    else
                    {
                        newGo.transform.localScale = new Vector3(distance * 0.5f * Mathf.Cos(angle) + width * Mathf.Sin(angle), height, width * Mathf.Cos(angle) + distance * 0.5f * Mathf.Sin(angle));
                    }*/

                    newGo.GetComponent<NodeCollider>().NavNode = this;

                    nodeColliders.Add(newGo);
                }
            }
        }
    }

    


    private void OnTriggerEnter(Collider other)
    {
        /*Transform parent = other.transform;
        while (parent.parent != null)
        {
            parent = parent.parent;
        }

        if (parent.GetComponent<RoopocketAI>() != null)
        {
            parent.GetComponent<RoopocketAI>().ReachedNode(this);
        }*/
    }
}

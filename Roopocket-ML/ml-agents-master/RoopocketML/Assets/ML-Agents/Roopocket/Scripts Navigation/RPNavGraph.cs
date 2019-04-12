using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RPNavGraph : MonoBehaviour
{
    public Color endNodeColor;
    public Color normalNodeColor;

    public NodesGenerator[] nodesGenerators;

    public bool refresh = false;
    public bool refreshLines = false;
    private float counterLines = 0f;

    private List<RPNavNode> nodes = new List<RPNavNode>();

    private UndirGraphFloat<RPNavNode> graph = new UndirGraphFloat<RPNavNode>();

    private float noRegistrFor = 0f;
    private bool graphMade = false;

    private bool builtGraphOnce = false;

	// Use this for initialization
	void Start ()
    {
        //buildGraph();
        if (nodesGenerators.Length == 0)
        {
            buildGraph();
            builtGraphOnce = true;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!builtGraphOnce)
        {
            bool oneNot = false;
            for (int i = 0; i < nodesGenerators.Length; i++)
            {
                if (nodesGenerators[i].GeneratedAll == false)
                {
                    oneNot = true;
                    break;
                }
            }

            if (oneNot == false)
            {
                buildGraph();
                builtGraphOnce = true;
            }
        }

        if (graphMade == false)
        {
            noRegistrFor += Time.deltaTime;
        }

        if (noRegistrFor >= 1f && graphMade == false)
        {
            buildGraph();
            graphMade = true;
        }

        if (refresh)
        {
            refresh = false;
            if (Application.isEditor)
            {
                buildGraph();

                for (int i = 0; i < graph.Nodes.Count; i++)
                {
                    for (int j = 0; j < graph.GetOutcommingEdges(graph.Nodes[i]).Count; j++)
                    {
                        Transform from = graph.Nodes[i].ID.transform;
                        Transform to = graph.GetOutcommingEdges(graph.Nodes[i])[j].To.ID.transform;
                        Debug.DrawLine(from.position, to.position, Color.yellow, 0.2f);

                    }
                }
            }
        }

        if (refreshLines)
        {
            counterLines += Time.deltaTime;

            if (Application.isEditor && counterLines >= 1f)
            {
                counterLines = 0f;
                for (int i = 0; i < graph.Nodes.Count; i++)
                {
                    for (int j = 0; j < graph.GetOutcommingEdges(graph.Nodes[i]).Count; j++)
                    {
                        Transform from = graph.Nodes[i].ID.transform;
                        Transform to = graph.GetOutcommingEdges(graph.Nodes[i])[j].To.ID.transform;
                        Debug.DrawLine(from.position, to.position, Color.yellow, 1f);

                    }
                }
            }
        }
    }

    private void buildGraph()
    {
        Debug.Log("Building graph...");

        graph = new UndirGraphFloat<RPNavNode>();

        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] == null)
            {
                nodes.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            graph.AddNode(nodes[i]);
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            for (int j = 0; j < nodes[i].neighbours.Length; j++)
            {
                if (graph.GetNode(nodes[i]) != null && graph.GetNode(nodes[i].neighbours[j].GetComponent<RPNavNode>()) != null)
                {
                    graph.AddEdge(graph.GetNode(nodes[i]), graph.GetNode(nodes[i].neighbours[j].GetComponent<RPNavNode>()), (Vector3.Distance(nodes[i].transform.position, nodes[i].neighbours[j].position)));
                }
            }
        }



        // Voronoi Meshes
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].BuildVoronoiMeshes();
        }

        Debug.Log("Graph generated!");
    }

    public List<RPNavNode> GetPathTo(RPNavNode from, RPNavNode goal)
    {
        List<Node<RPNavNode>> path = graph.Dijkstra(graph.GetNode(from), graph.GetNode(goal));
        List<RPNavNode> rpPath = new List<RPNavNode>();
        for (int i = 0; i < path.Count; i++)
        {
            rpPath.Add(path[i].ID);
        }
        return rpPath;
    }

    public RPNavNode GetNode(string stationName)
    {
        RPNavNode node = null;
        for (int i = 0; i < graph.Nodes.Count; i++)
        {
            if (graph.Nodes[i].ID.nodeName == stationName)
            {
                node = graph.Nodes[i].ID;
            }
        }

        return node;
    }

    public List<RPNavNode> GetPathTo(string from, string goal)
    {
        RPNavNode fromNode = GetNode(from);
        RPNavNode toNode = GetNode(goal);

        if (fromNode != null && toNode != null)
        {
            return GetPathTo(fromNode, toNode);
        }

        return new List<RPNavNode>();
    }


    public void RegisterNode(RPNavNode node)
    {
        noRegistrFor = 0f;
        if (nodes.Contains(node) == false)
        {
            nodes.Add(node);
            Debug.Log("Node registered. Now amount: " + nodes.Count.ToString());
        }
    }

    public UndirGraphFloat<RPNavNode> Graph
    {
        get
        {
            return graph;
        }
    }

    public bool GraphReady
    {
        get
        {
            return graphMade;
        }
    }


    private static RPNavGraph inst = null;
    public static RPNavGraph Inst
    {
        get
        {
            if (inst == null)
            {
                inst = GameObject.FindObjectOfType<RPNavGraph>();
            }
            return inst;
        }
    }
}

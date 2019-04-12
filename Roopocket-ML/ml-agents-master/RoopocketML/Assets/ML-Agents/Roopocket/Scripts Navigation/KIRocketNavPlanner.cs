using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KIRocketNavPlanner : MonoBehaviour
{
    [SerializeField]
    private RPNavGraph graph;

    private List<string> stations = new List<string>();

    private bool gotStations = false;

	// Use this for initialization
	void Start ()
    {

	}

    public List<string> Stations
    {
        get
        {
            return stations;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		if ((!gotStations) && graph.GraphReady)
        {
            gotStations = true;
            for (int i = 0; i < graph.Graph.Nodes.Count; i++)
            {
                if (graph.Graph.Nodes[i].ID.nodeName.Length > 0)
                {
                    stations.Add(graph.Graph.Nodes[i].ID.nodeName);
                }
            }
        }
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UndirGraph<T> : Graph<T>
{
    public UndirGraph()
    {
        nodes = new List<Node<T>>();
        edges = new List<Edge<T>>();
    }

    public override void DeleteEdge(Edge<T> edge)
    {
        Node<T> from = edge.From;
        Node<T> to = edge.To;

        for (int i = 0; i < edges.Count; i++)
        {
            if ((edges[i].To == to && edges[i].From == from) || (edges[i].To == from && edges[i].From == to))
            {
                edges.RemoveAt(i);
                i--;
            }
        }
    }

    public override void AddEdge(Node<T> from, Node<T> to)
    {
        Edge<T> newEdge = new Edge<T>(from, to);
        Edge<T> backEdge = new Edge<T>(to, from);
        if (!edges.Contains(newEdge))
        {
            edges.Add(newEdge);
        }
        if (!edges.Contains(backEdge))
        {
            edges.Add(backEdge);
        }
    }

    public override bool AddNode(T node)
    {
        Node<T> newNode = new Node<T>(node);
        if (!nodes.Contains(newNode))
        {
            nodes.Add(newNode);
            return true;
        }

        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class UndirGraphTag<T, T2> : Graph<T>
{
    public UndirGraphTag()
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
        EdgeTag<T, T2> newEdge = new EdgeTag<T, T2>(from, to);
        EdgeTag<T, T2> backEdge = new EdgeTag<T, T2>(to, from);
        if (!edges.Contains(newEdge))
        {
            edges.Add(newEdge);
        }
        if (!edges.Contains(backEdge))
        {
            edges.Add(backEdge);
        }
    }

    public void AddEdge(Node<T> from, Node<T> to, T2 tag)
    {
        EdgeTag<T, T2> newEdge = new EdgeTag<T, T2>(from, to);
        EdgeTag<T, T2> backEdge = new EdgeTag<T, T2>(to, from);
        newEdge.Tag = tag;
        backEdge.Tag = tag;
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


[Serializable]
public class UndirGraphFloat<T> : UndirGraphTag<T, float>
{
    public UndirGraphFloat()
    {
        nodes = new List<Node<T>>();
        edges = new List<Edge<T>>();
    }

    public List<Node<T>> Dijkstra(Node<T> from, Node<T> to)
    {
        List<Node<T>> Q = new List<Node<T>>();
        Dictionary<Node<T>, float> distances = new Dictionary<Node<T>, float>();
        Dictionary<Node<T>, Node<T>> prev = new Dictionary<Node<T>, Node<T>>();

        for (int i = 0; i < nodes.Count; i++)
        {
            distances.Add(nodes[i], float.MaxValue);
            prev.Add(nodes[i], null);
            Q.Add(nodes[i]);
        }

        distances[from] = 0f;

        while (Q.Count > 0)
        {
            Node<T> u = null;
            float minDistance = float.MaxValue;
            for (int i = 0; i < Q.Count; i++)
            {
                if (distances[Q[i]] < minDistance)
                {
                    u = Q[i];
                    minDistance = distances[Q[i]];
                }
            }

            Q.Remove(u);

            for (int i = 0; i < GetOutcommingEdges(u).Count; i++)
            {
                Node<T> v = GetOutcommingEdges(u)[i].To;
                float alt = distances[u] + ((EdgeTag<T, float>)GetOutcommingEdges(u)[i]).Tag;
                if (alt < distances[v])
                {
                    distances[v] = alt;
                    prev[v] = u;
                }
            }
        }

        List<Node<T>> pathRev = new List<Node<T>>();
        pathRev.Add(to);
        while (prev[pathRev[pathRev.Count - 1]] != from)
        {
            pathRev.Add(prev[pathRev[pathRev.Count - 1]]);
        }
        pathRev.Add(prev[pathRev[pathRev.Count - 1]]);

        pathRev.Reverse();

        return pathRev;
    }
}
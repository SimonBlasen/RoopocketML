using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public abstract class Graph<T>
{
    protected List<Node<T>> nodes;

    protected List<Edge<T>> edges;

    public List<Node<T>> Nodes
    {
        get
        {
            return nodes;
        }
    }

    public List<Edge<T>> Edges
    {
        get
        {
            return edges;
        }
    }

    public abstract bool AddNode(T node);

    public abstract void AddEdge(Node<T> from, Node<T> to);

    public Node<T> GetNode(T element)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].ID.Equals(element))
            {
                return nodes[i];
            }
        }

        return null;
    }

    public int GetNodeIndex(Node<T> node)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] == node)
            {
                return i;
            }
        }

        return -1;
    }


    public bool ExistsEdgeFromTo(Node<T> from, Node<T> to)
    {
        for (int i = 0; i < edges.Count; i++)
        {
            if (edges[i].From == from && edges[i].To == to)
            {
                return true;
            }
        }

        return false;
    }

    public void DeleteNode(Node<T> node)
    {
        for (int i = 0; i < edges.Count; i++)
        {
            if (edges[i].From == node || edges[i].To == node)
            {
                edges.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] == node)
            {
                nodes.RemoveAt(i);
                break;
            }
        }
    }

    public virtual void DeleteEdge(Edge<T> edge)
    {
        for (int i = 0; i < edges.Count; i++)
        {
            if (edges[i].From == edge.From || edges[i].To == edge.To)
            {
                edges.RemoveAt(i);
                i--;
            }
        }
    }

    public bool ContainsNode(Node<T> node)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] == node)
            {
                return true;
            }
        }

        return false;
    }

    public List<Edge<T>> GetOutcommingEdges(Node<T> node)
    {
        List<Edge<T>> outEdges = new List<Edge<T>>();
        for (int i = 0; i < edges.Count; i++)
        {
            if (edges[i].From.Equals(node))
            {
                outEdges.Add(edges[i]);
            }
        }

        return outEdges;
    }

    public List<Edge<T>> GetIncommingEdges(Node<T> node)
    {
        List<Edge<T>> inEdges = new List<Edge<T>>();
        for (int i = 0; i < edges.Count; i++)
        {
            if (edges[i].To.Equals(node))
            {
                inEdges.Add(edges[i]);
            }
        }

        return inEdges;
    }

    public bool ExistsPathFromTo(Node<T> from, Node<T> to)
    {
        List<Node<T>> left = new List<Node<T>>();
        List<Node<T>> right = new List<Node<T>>();

        left.Add(from);
        right.Add(to);
        fillFromNode(from, left);
        fillToNode(to, right);

        //return left.Count + right.Count == nodes.Count * 2;
        if (left.Count == right.Count)
        {
            Node<T> check = left[0];
            for (int i = 0; i < right.Count; i++)
            {
                if (check == right[i])
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void fillFromNode(Node<T> from, List<Node<T>> yetFilled)
    {
        List<Edge<T>> outEdges = GetOutcommingEdges(from);
        for (int i = 0; i < outEdges.Count; i++)
        {
            if (!yetFilled.Contains(outEdges[i].To))
            {
                yetFilled.Add(outEdges[i].To);
                fillFromNode(outEdges[i].To, yetFilled);
            }
        }
    }

    private void fillToNode(Node<T> to, List<Node<T>> yetFilled)
    {
        List<Edge<T>> inEdges = GetIncommingEdges(to);
        for (int i = 0; i < inEdges.Count; i++)
        {
            if (!yetFilled.Contains(inEdges[i].From))
            {
                yetFilled.Add(inEdges[i].From);
                fillFromNode(inEdges[i].From, yetFilled);
            }
        }
    }

    public Graph<T> Clone()
    {
        Graph<T> newGraph = null;
        //= this.GetType();
        if (GetType() == typeof(UndirGraph<T>))
        {
            newGraph = new UndirGraph<T>();
        }
        List<Node<T>> newNodes = new List<Node<T>>();
        List<Edge<T>> newEdges = new List<Edge<T>>();
        for (int i = 0; i < nodes.Count; i++)
        {
            newNodes.Add(new Node<T>(nodes[i].ID));
        }

        for (int i = 0; i < edges.Count; i++)
        {
            Node<T> newFrom = null;
            Node<T> newTo = null;
            for (int j = 0; j < newNodes.Count; j++)
            {
                if (newNodes[j].ID.Equals(edges[i].From.ID))
                {
                    newFrom = newNodes[j];
                }
                if (newNodes[j].ID.Equals(edges[i].To.ID))
                {
                    newTo = newNodes[j];
                }
                if (newFrom != null && newTo != null)
                {
                    break;
                }
            }

            newEdges.Add(new Edge<T>(newFrom, newTo));
        }

        newGraph.nodes = newNodes;
        newGraph.edges = newEdges;

        return newGraph;
    }

    public void WriteToFile(string filename)
    {
        List<string> lines = new List<string>();
        for (int i = 0; i < nodes.Count; i++)
        {
            lines.Add("Node:" + nodes[i].ID.ToString());
        }
        for (int i = 0; i < edges.Count; i++)
        {
            lines.Add("Edge:" + edges[i].From.ID.ToString() + "$" + edges[i].To.ID.ToString());
        }

        System.IO.File.WriteAllLines(filename, lines.ToArray());
    }

    public void SortByZshgk()
    {
        List<Node<T>> newNodes = new List<Node<T>>();
        for (int i = 0; i < nodes.Count; i++)
        {
            if (newNodes.Contains(nodes[i]) == false)
            {
                newNodes.Add(nodes[i]);
                for (int j = 0; j < nodes.Count; j++)
                {
                    if (i != j)
                    {
                        if (ExistsPathFromTo(nodes[i], nodes[j]))
                        {
                            newNodes.Add(nodes[j]);
                        }
                    }
                }
            }

        }

        if (newNodes.Count == nodes.Count)
        {
            Debug.Log("Success sorting");

            nodes = newNodes;
        }
        else
        {
            Debug.Log("Sorting failed");
        }
    }

    public string[] ToDot()
    {
        List<string> lines = new List<string>();
        lines.Add("digraph G {");
        for (int i = 0; i < nodes.Count; i++)
        {
            lines.Add(i + " [label = \"" + nodes[i].ID.ToString() + "\"] ;");
        }
        for (int i = 0; i < edges.Count; i++)
        {
            int fromIndex = 0;
            int toIndex = 0;
            for (int j = 0; j < nodes.Count; j++)
            {
                if (edges[i].From == nodes[j])
                {
                    fromIndex = j;
                }
                if (edges[i].To == nodes[j])
                {
                    toIndex = j;
                }
            }

            lines.Add(fromIndex + " -> " + toIndex + " [];");
        }

        lines.Add("}");
        return lines.ToArray();
    }
}

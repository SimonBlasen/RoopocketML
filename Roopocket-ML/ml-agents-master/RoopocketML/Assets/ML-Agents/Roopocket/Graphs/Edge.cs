using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Edge<T>
{
    protected Node<T> from;
    protected Node<T> to;

    public Edge(Node<T> from, Node<T> to)
    {
        this.from = from;
        this.to = to;
    }

    public Node<T> From
    {
        get
        {
            return from;
        }
    }

    public Node<T> To
    {
        get
        {
            return to;
        }
    }

    public override bool Equals(object obj)
    {
        if (obj is Edge<T>)
        {
            Edge<T> other = (Edge<T>)obj;

            if (other == null && from == null)
            {
                return true;
            }
            else if ((other == null && from != null) || (other != null && from == null))
            {
                return false;
            }

            return other.from.Equals(from) && other.to.Equals(to);
        }
        else
        {
            return base.Equals(obj);
        }
    }

    public static bool operator ==(Edge<T> a, Edge<T> b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(Edge<T> a, Edge<T> b)
    {
        return !a.Equals(b);
    }
}

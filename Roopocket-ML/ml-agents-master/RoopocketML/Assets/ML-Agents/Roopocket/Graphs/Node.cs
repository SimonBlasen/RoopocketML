using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Node<T>
{
    protected T id;

    public Node(T id)
    {
        this.id = id;
    }

    public T ID
    {
        get
        {
            return id;
        }
    }

    public override bool Equals(object obj)
    {
        if (obj is Node<T>)
        {
            return ((Node<T>)obj).id.Equals(id);
        }
        else
        {
            return base.Equals(obj);
        }
    }

    public static bool operator ==(Node<T> a, Node<T> b)
    {
        if (((object)a) == null && ((object)b) == null)
        {
            return true;
        }
        else if (((object)a) != null && ((object)b) != null)
        {
            return a.Equals(b);
        }

        return false;
    }

    public static bool operator !=(Node<T> a, Node<T> b)
    {
        if (((object)a) == null && ((object)b) == null)
        {
            return false;
        }
        else if (((object)a) != null && ((object)b) != null)
        {
            return !a.Equals(b);
        }

        return true;
    }
}

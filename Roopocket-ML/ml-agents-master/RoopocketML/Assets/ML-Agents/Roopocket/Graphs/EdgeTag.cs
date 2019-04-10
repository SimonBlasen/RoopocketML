using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class EdgeTag<T, T2> : Edge<T>
{
    protected T2 tag;

    public EdgeTag(Node<T> from, Node<T> to) : base(from, to)
    {
    }

    public T2 Tag
    {
        get
        {
            return tag;
        }
        set
        {
            tag = value;
        }
    }
}
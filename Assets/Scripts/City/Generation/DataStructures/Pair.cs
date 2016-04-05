using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pair<T1, T2>
{
    private Pair(T1 first, T2 second) 
    {
        this.first = first;
        this.second = second;
    }
    
    public static Pair<T1, T2> Create(T1 first, T2 second)
    {
        return new Pair<T1, T2>(first, second);
    }
    
    public T1 first;
    public T2 second;
}

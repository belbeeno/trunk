using System;
using System.Collections.Generic;
using System.Linq;

public static class LinqExtensions
{
    public static IEnumerable<T> RemoveWhere<T>(this IEnumerable<T> source, Func<T, bool> check)
    {
        var toRemove = source.Where(check).ToArray();
        var result = source.Except(toRemove);
        return result;
    }
    
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
        {
            action(item);
        }
    }
    
    public static T RandomMember<T>(this IEnumerable<T> source)
    {
        var index = UnityEngine.Random.Range(0, source.Count());
        var member = source.ElementAt(index);
        
        return member;
    }
}
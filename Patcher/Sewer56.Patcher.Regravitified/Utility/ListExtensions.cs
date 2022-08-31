using System;
using System.Collections.Generic;

namespace Sewer56.Patcher.Riders.Utility;

/// <summary>
/// Extensions related to lists and their interfaces.
/// </summary>
public static class ListExtensions
{
    private static Random _random = new Random();

    /// <summary>
    /// Shuffles a list using an implementation based on Fisher-Yates.
    /// </summary>
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = _random.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}
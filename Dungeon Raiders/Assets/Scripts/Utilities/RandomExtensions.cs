using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomExtensions
{
    public static bool Bool(float chance)
    {
        return Random.Range(0, 0.99f) < Mathf.Clamp01(chance);
    }
}

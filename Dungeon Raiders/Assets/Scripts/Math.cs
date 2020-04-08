using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtilities
{
    public static int GetRandomIndexFromListOfChances(List<float> chances)
    {
        float max = 0;
        for (int i = 0; i < chances.Count; i++)
            max += chances[i];

        float roll = Random.Range(0, max);

        float lastMin = 0;
        float lastMax = 0;
        int index = -1;
        for (int i = 0; i < chances.Count; i++)
        {
            if (i > 0)
                lastMin += chances[i - 1];

            lastMax += chances[i];

            if (roll >= lastMin && roll < lastMax && chances[i] != 0)
                index = i;
        }
        return index;
    }
}

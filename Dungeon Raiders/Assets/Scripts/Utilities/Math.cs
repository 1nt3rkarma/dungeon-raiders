using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtils
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

    public static int LineDistance(Unit fromUnit, Unit toUnit)
    {
        return Mathf.Abs(LineDelta(fromUnit, toUnit));
    }

    public static int LineDelta(Unit fromUnit, Unit toUnit)
    {
        return fromUnit.block.GetRowIndex() - toUnit.block.GetRowIndex();
    }

    /// <summary>
    /// Converts angle from [-180; 180] to [0; 360]
    /// </summary>
    public static float SignedTo360(float signedAngle)
    {
        if (signedAngle < 0)
            return 360 + signedAngle;
        else
            return signedAngle;
    }
}

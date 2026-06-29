using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FloatExtensions
{
    public static float SafeDivide(this float dividend, float divider)
    {
        if (divider == 0f)
        {
            return 0f;
        }

        return dividend / divider;
    }
}

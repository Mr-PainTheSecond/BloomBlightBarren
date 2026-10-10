using UnityEngine;

public class Utilities
{
    public static bool approxEqual(float a, float b, float delta)
    {
        return Mathf.Abs(a - b) < delta;
    }
}

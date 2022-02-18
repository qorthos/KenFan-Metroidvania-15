using UnityEngine;

public static class Vector3Extension
{
    public static bool Approximately(this Vector3 v,Vector3 other)
    {
        return Mathf.Approximately(v.x, other.x) && Mathf.Approximately(v.z, other.z) && Mathf.Approximately(v.y, other.y);
    }
}


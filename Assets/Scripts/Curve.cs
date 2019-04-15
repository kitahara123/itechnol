using UnityEngine;

public class Curve
{
    public string Name { get; set; }
    public Vector3[] xyzPoints { get; private set; }

    public void CreateCoordinateMassives(int coordinateLinesLength)
    {
        xyzPoints = new Vector3[coordinateLinesLength];
    }
}
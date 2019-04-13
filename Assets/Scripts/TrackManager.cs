using System.Collections.Generic;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    [SerializeField] private GameObject primitive;
    [SerializeField] private float trackOffset = -5f;
    [SerializeField] private string waypointsFileName = "Assets/Seg03Waypoints.ma";
    
    private static List<Curve> curves;
    public static List<Curve> Curves => curves;

    private void Awake()
    {
        curves = MaNURBSParser.Parse(waypointsFileName);
        
        for (var i = 0; i < curves[1].xyzPoints.Length; i++)
        {
            foreach (var curve in curves)
            {
                var point = curve.xyzPoints[i];
                var go = Instantiate(primitive);
                go.transform.position = new Vector3(point.x, point.y + trackOffset, point.z);
                go.transform.SetParent(transform);
            }
        }
    }
    
}
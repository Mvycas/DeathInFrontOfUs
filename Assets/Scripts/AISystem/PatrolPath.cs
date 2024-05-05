using UnityEngine;

public class PatrolPath : MonoBehaviour
{
    public Transform[] GetWaypoints()
    {
        Transform[] waypoints = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            waypoints[i] = transform.GetChild(i);
        }
        return waypoints;
    }
}
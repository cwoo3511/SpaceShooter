using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGizmos : MonoBehaviour
{
    private const string wayPointFile = "Enemy";

    public enum Type { NORMAL, WAYPOINT }
    public Color _color = Color.yellow;
    public float radius = 0.1f;
    public Type _type = Type.NORMAL;

    private void OnDrawGizmos()
    {
        if (_type == Type.NORMAL)
        {
            Gizmos.color = _color;
            Gizmos.DrawSphere(transform.position, radius);
        }
        else
        {
            Gizmos.color = _color;
            Gizmos.DrawIcon(transform.position + Vector3.up * 1.2f
                , wayPointFile, true);
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
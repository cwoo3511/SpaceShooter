using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform target; //추적 대상
    public float moveDamping = 15.0f; //이동속도 계수
    public float rotateDamping = 10.0f; //회전속도 계수
    public float distance = 5.0f; //추적 대상과의 거리
    public float height = 4.0f; //추적 대상과의 높이
    public float targetOffset = 2.0f; //추적 좌표의 수정값

    private Transform tr;

    void Start()
    {
        tr = GetComponent<Transform>();
    }

    void LateUpdate()
    {
        var camPos = target.position - (target.forward 
            * distance) + (target.up * height);
        tr.position = Vector3.Slerp(tr.position, camPos
            , Time.deltaTime * moveDamping);
        tr.rotation = Quaternion.Slerp(tr.rotation, target
            .rotation, Time.deltaTime * rotateDamping);
        tr.LookAt(target.position + (target.up * targetOffset));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(target.position
            + (target.up * targetOffset), 0.1f);
        Gizmos.DrawLine(target.position
            + (target.up * targetOffset), transform.position);
    }
}

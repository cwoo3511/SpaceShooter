using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAnim
{
    public AnimationClip idle;
    public AnimationClip runF;
    public AnimationClip runB;
    public AnimationClip runL;
    public AnimationClip runR;
}

public class PlayerCtrl : MonoBehaviour
{
    //[System.Serializable]
    //public class PlayerAnim
    //{
    //    public AnimationClip idle;
    //    public AnimationClip runF;
    //    public AnimationClip runB;
    //    public AnimationClip runL;
    //    public AnimationClip runR;
    //}

    private float h = 0.0f;
    private float v = 0.0f;
    private float r = 0.0f;
    private Transform tr;

    public float moveSpeed = 10.0f;
    public float rotSpeed = 80.0f;
    public PlayerAnim _playerAnim;
    public Animation anim;

    private void Start()
    {
        tr = this.gameObject.GetComponent<Transform>();
        anim = GetComponent<Animation>();
        anim.clip = _playerAnim.idle;
        anim.Play();
    }

    private void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        r = Input.GetAxis("Mouse X");

        //Debug.Log("H : " + h.ToString());
        //Debug.Log("V : " + v.ToString());

        Vector3 dir = (Vector3.right * h)
            + (Vector3.forward * v);

        if (dir.sqrMagnitude >= 1.0f)
        {
            dir = dir.normalized;
        }

        tr.Translate(dir * moveSpeed * Time.deltaTime
            , Space.Self);
        //tr.Translate(dir.normalized * moveSpeed
        // * Time.deltaTime, Space.Self);
        tr.Rotate(Vector3.up * r * rotSpeed
            * Time.deltaTime);

        if (v >= 0.1f)
        {
            //anim.clip = _playerAnim.runF;
            //anim.Play();

            anim.CrossFade(_playerAnim.runF.name, 0.3f);
        }
        else if (v <= -0.1f)
        {
            anim.CrossFade(_playerAnim.runB.name, 0.3f);
        }
        else if (h >= 0.1f)
        {
            anim.CrossFade(_playerAnim.runR.name, 0.3f);
        }
        else if (h <= -0.1f)
        {
            anim.CrossFade(_playerAnim.runL.name, 0.3f);
        }
        else
        {
            anim.CrossFade(_playerAnim.idle.name, 0.3f);
        }
    }
}
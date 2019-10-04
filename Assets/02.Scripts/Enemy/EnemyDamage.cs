using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    private const string bulletTag = "BULLET";
    private float hp = 100.0f;
    private GameObject bloodEffect;
    private SkinnedMeshRenderer meshRenderer;

    public GameObject fireEffect;
    public Transform firePivot;
    public Texture fireTexture;

    void Start()
    {
        bloodEffect = Resources
            .Load<GameObject>("BulletImpactFleshBigEffect");
        meshRenderer 
            = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.tag == bulletTag)
        {
            ShowBloodEffect(coll);
            Destroy(coll.gameObject);
            hp -= coll.gameObject.GetComponent<BulletCtrl>()
                .damage;
            if (hp <= 0.0f)
            {
                this.gameObject.GetComponent<EnemyAI>()._state
                    = EnemyAI.State.DIE;
            }
        }
    }

    private void ShowBloodEffect(Collision coll)
    {
        Vector3 pos = coll.contacts[0].point;
        Vector3 _normal = coll.contacts[0].normal;
        Quaternion rot = Quaternion.FromToRotation(-Vector3
            .forward, _normal);
        GameObject blood = Instantiate<GameObject>(bloodEffect
            , pos, rot);
        Destroy(blood, 1.0f);
    }

    public void OnExpDamage()
    {
        GameObject fire = Instantiate(fireEffect
            , firePivot.position, Quaternion.identity);
        fire.transform.SetParent(firePivot);
        meshRenderer.material.mainTexture = fireTexture;
        GetComponent<EnemyAI>()._state = EnemyAI.State.DIE;
    }

    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    public GameObject expEffect;
    public GameObject fireEffect;
    public Mesh[] meshes;
    public Texture[] textures;
    public float expRadius = 10.0f;
    public AudioClip expSfx;
    public Transform firePivot;

    private int hitCount = 0;
    private Rigidbody rb;
    private MeshFilter _meshFilter;
    private MeshRenderer _renderer;
    private AudioSource _audio;
    private Shake _shake;
    private bool isExp = false;
    private bool isFire = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        _shake = GameObject.Find("CameraRig")
            .GetComponent<Shake>();
        _meshFilter = GetComponent<MeshFilter>();
        _renderer = GetComponent<MeshRenderer>();
        _renderer.material.mainTexture
            = textures[Random.Range(0
            , textures.Length - 2)];
        _audio = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("BULLET"))
        {
            if (++hitCount == 3 && !isFire)
            {
                FireBarrel();
            }
            else if (hitCount == 6 && !isExp)
            {
                ExpBarrel();
            }
        }
    }

    public void FireBarrel()
    {
        GameObject fire = Instantiate(fireEffect
            , firePivot.position, Quaternion.identity);
        fire.transform.SetParent(firePivot);
        if (!isFire)
        {
            _renderer.material.mainTexture
            = textures[textures.Length - 2];
        }
        if (!isExp) StartCoroutine(DelayExp());
        isFire = true;
    }

    private IEnumerator DelayExp()
    {
        yield return new WaitForSeconds(Random
            .Range(8.0f, 15.0f));

        if (!isExp) ExpBarrel();
    }

    private void ExpBarrel()
    {
        isExp = true;
        GameObject exp = Instantiate(expEffect
            , transform.position, Quaternion.identity);
        Destroy(exp, 5.0f);
        //rb.mass = 1.0f;
        //rb.AddForce(Vector3.up * 1000.0f);
        IndirectDamage(transform.position);

        int idx = Random.Range(0, meshes.Length);
        //_meshFilter.sharedMesh = meshes[idx];
        _meshFilter.mesh = meshes[idx];
        _renderer.material.mainTexture
            = textures[textures.Length - 1];
        _audio.PlayOneShot(expSfx, 3.0f);
        _shake.shakeRotate = true;
        StartCoroutine(_shake.ShakeCamera(0.1f, 0.2f, 0.5f));
        //Destroy(this.gameObject, 30.0f);
    }

    private void IndirectDamage(Vector3 pos)
    {
        Collider[] colls = Physics.OverlapSphere(pos
            , expRadius, 1 << 8 | 1 << 11);
        foreach (var item in colls)
        {
            if (item.tag == "ENEMY")
            {
                item.gameObject.layer = LayerMask
                    .NameToLayer("Default");
                item.gameObject.SendMessage("OnExpDamage", pos
                , SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                var _rb = item.GetComponent<Rigidbody>();
                _rb.mass = 1.0f;
                _rb.AddExplosionForce(1200.0f, pos
                    , expRadius, 1000.0f);
                item.GetComponent<BarrelCtrl>().FireBarrel();
            }
        }
    }
}
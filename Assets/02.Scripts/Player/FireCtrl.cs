using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct PlayerSfx
{
    public AudioClip[] fireSfx;
    public AudioClip[] reloadSfx;
}

public class FireCtrl : MonoBehaviour
{
    public enum WeaponType { RIFLE, SHOTGUN }
    public WeaponType currWeapon = WeaponType.RIFLE;
    public GameObject bullet;
    public Transform firePos;
    public ParticleSystem cartridge;
    public float fireRate = 0.1f;
    public PlayerSfx _playerSfx;
    public Image magazineImg;
    public Text magazineText;
    public int maxBullet = 10;
    public int remainingBullet = 10;

    private ParticleSystem muzzleFlash;
    private AudioSource _audio;
    private Shake _shake;
    private float nextFire = 0.0f;
    private bool isReloading = false;

    private void Start()
    {
        muzzleFlash = firePos
            .GetComponentInChildren<ParticleSystem>();
        _audio = GetComponent<AudioSource>();
        _shake = GameObject.Find("CameraRig")
            .GetComponent<Shake>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0)
            && Time.time >= nextFire && remainingBullet > 0)
        {
            --remainingBullet;
            Fire();
            nextFire = Time.time + fireRate;
            if (remainingBullet == 0)
            {
                StartCoroutine(Reloading());
            }
        }
        //if (Input.GetMouseButton(0))
        //{
        //    Fire();
        //}
    }

    private void Fire()
    {
        StartCoroutine(_shake.ShakeCamera());
        Instantiate(bullet, firePos.position
            , firePos.rotation);
        cartridge.Play();
        muzzleFlash.Play();
        FireSfx();
        magazineImg.fillAmount 
            = (float)remainingBullet / (float)maxBullet;
        UpdateBulletText();
    }

    private void FireSfx()
    {
        var sfx = _playerSfx.fireSfx[(int)currWeapon];
        _audio.PlayOneShot(sfx, 1.0f);
    }

    private IEnumerator Reloading()
    {
        isReloading = true;
        yield return new WaitForSeconds(0.3f);
        _audio.PlayOneShot(_playerSfx
            .reloadSfx[(int)currWeapon], 3.0f);
        yield return new WaitForSeconds(_playerSfx
            .reloadSfx[(int)currWeapon].length);
        isReloading = false;
        magazineImg.fillAmount = 1.0f;
        remainingBullet = maxBullet;
        UpdateBulletText();
    }

    private void UpdateBulletText()
    {
        magazineText.text = string.Format("<color=#ff0000>{0}" +
            "</color>/{1}", remainingBullet, maxBullet);
    }   
}
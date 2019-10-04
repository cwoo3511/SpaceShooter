using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
    private const string bulletTag = "BULLET";
    private const string enemyTag = "ENEMY";
    private float initHp = 100.0f;
    private readonly Color initColor 
        = new Color(0, 1.0f, 0, 1.0f);
    private Color currColor;

    public float currHp;
    public Image bloodScreen;
    public Image hpBar;

    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;

    void Start()
    {
        currHp = initHp;
        hpBar.color = initColor;
        currColor = initColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == bulletTag)
        {
            Destroy(other.gameObject);
            StartCoroutine(ShowBloodScreen());
            currHp -= 5.0f;
            Debug.Log("Player Hp : " + currHp.ToString());
            DisplayHpbar();
            if (currHp <= 0.0f)
            {
                PlayerDie();
            }
        }
    }

    private void DisplayHpbar()
    {
        float hp = currHp / initHp;
        if (hp >= 0.5f)
        {
            currColor.r = (1.0f - hp) * 2.0f;
        }
        else
        {
            currColor.g = hp * 2.0f;
        }        
        hpBar.color = currColor;
        hpBar.fillAmount = hp;
    }

    private IEnumerator ShowBloodScreen()
    {
        bloodScreen.color
            = new Color(1.0f, 0, 0, Random.Range(0.3f, 0.6f));
        yield return new WaitForSeconds(0.2f);
        bloodScreen.color = Color.clear;
    }

    private void PlayerDie()
    {
        Debug.Log("Player Die!!!");

        OnPlayerDie();

        //GameObject[] enemies = GameObject
        //    .FindGameObjectsWithTag(enemyTag);
        //foreach (var item in enemies)
        //{
        //    item.SendMessage("OnPlayerDie", SendMessageOptions
        //        .DontRequireReceiver);
        //}
    }

    void Update()
    {
        
    }
}

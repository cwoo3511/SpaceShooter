using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum State { PATROL, TRACE, ATTACK, DIE }
    public State _state = State.PATROL;

    public float attackDist = 5.0f;
    public float traceDist = 10.0f;
    public bool isDie = false;

    private Transform playerTr;
    private Transform enemyTr;
    private WaitForSeconds ws;
    private MoveAgent _moveAgent;
    private Animator _animator;
    private EnemyFire _enemyFire;

    private readonly int hashMove
        = Animator.StringToHash("IsMove");
    private readonly int hashSpeed
        = Animator.StringToHash("Speed");
    private readonly int hashDie
        = Animator.StringToHash("Die");
    private readonly int hashDieIdx
        = Animator.StringToHash("DieIdx");
    private readonly int hashOffset
        = Animator.StringToHash("Offset");
    private readonly int hashWalkSpeed
        = Animator.StringToHash("WalkSpeed");
    private readonly int hashPlayerDie
        = Animator.StringToHash("PlayerDie");

    //모든 Start() 함수보다 먼저 호출됨.
    //스크립트가 비활성화 되어 있어도 실행됨.
    //코루틴 함수를 호출할 수 없음.
    private void Awake()
    {
        //var player = GameObject.Find("Player");
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTr = player.GetComponent<Transform>();
        }
        enemyTr = GetComponent<Transform>();
        _moveAgent = GetComponent<MoveAgent>();
        _animator = GetComponent<Animator>();
        _enemyFire = GetComponent<EnemyFire>();
        ws = new WaitForSeconds(0.3f);
        _animator.SetFloat(hashOffset, Random.Range(0.0f, 1.0f));
        _animator.SetFloat(hashWalkSpeed
            , Random.Range(0.85f, 1.2f));
    }

    //스크립트가 활성화 될 때마다 호출되는 함수
    //코루틴 함수를 호출할 수 있음
    private void OnEnable()
    {
        StartCoroutine(CheckState());
        StartCoroutine(Action());
        Damage.OnPlayerDie += this.OnPlayerDie;
    }

    private void OnDisable()
    {
        Damage.OnPlayerDie -= this.OnPlayerDie;
    }

    private IEnumerator CheckState()
    {
        while (!isDie)
        {
            if (_state == State.DIE) yield break;
            float dist = Vector3.Distance(playerTr.position
                , enemyTr.position);
            if (dist <= attackDist)
            {
                _state = State.ATTACK;
            }
            else if (dist <= traceDist)
            {
                _state = State.TRACE;
            }
            else
            {
                _state = State.PATROL;
            }
            yield return ws;
        }
    }

    private IEnumerator Action()
    {
        while (!isDie)
        {
            yield return ws;
            switch (_state)
            {
                case State.PATROL:
                    _enemyFire.isFire = false;
                    _moveAgent.Patrolling = true;
                    _animator.SetBool(hashMove, true);
                    break;
                case State.TRACE:
                    _enemyFire.isFire = false;
                    _moveAgent.TraceTarget = playerTr.position;
                    _animator.SetBool(hashMove, true);
                    break;
                case State.ATTACK:
                    _moveAgent.Stop();
                    _animator.SetBool(hashMove, false);
                    if (_enemyFire.isFire == false)
                        _enemyFire.isFire = true;
                    break;
                case State.DIE:
                    isDie = true;
                    _enemyFire.isFire = false;
                    _moveAgent.Stop();
                    _animator.SetInteger(hashDieIdx
                        , Random.Range(0, 3));
                    _animator.SetTrigger(hashDie);
                    GetComponent<CapsuleCollider>()
                        .enabled = false;
                    break;    
            }
        }
    }

    private void Update()
    {
        _animator.SetFloat(hashSpeed, _moveAgent.Speed);
    }

    public void OnPlayerDie()
    {
        _moveAgent.Stop();
        _enemyFire.isFire = false;
        StopAllCoroutines();
        _animator.SetTrigger(hashPlayerDie);
    }
}

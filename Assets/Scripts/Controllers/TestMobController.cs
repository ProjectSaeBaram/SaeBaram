using System;
using UnityEditor;
using UnityEngine;

public class TestMobController : MobController
{
    [SerializeField] private float speed = 400f;
    [SerializeField] private float detectionRange = 300f;
    [SerializeField] private float chasableRange = 500f;
    [SerializeField] private float attackRange = 150f;
    
    [SerializeField] private Vector2 patrolPointA;
    [SerializeField] private Vector2 patrolPointB;
    
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;

    public override float GetDetectionRange() => detectionRange;
    public override float GetAttackRange() => attackRange;
    public override float GetChasableRange() => chasableRange;
    
    public override Vector2 GetPatrolPointA() => patrolPointA;
    public override Vector2 GetPatrolPointB() => patrolPointB;
    
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        // 초기 상태를 순찰 상태로 설정
        ChangeState(new PatrolState(patrolPointA, patrolPointB));
    }

    void FixedUpdate()
    {
        currentState?.Execute();
    }

    public override void ChangeState(IMobState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
        UpdateAnimationState(newState);
    }

    private void UpdateAnimationState(IMobState state)
    {
        switch (state)
        {
            case PatrolState:
                _animator.CrossFade("Patrol", 0.1f);
                break;
            case ChaseState:
                _animator.CrossFade("Chase", 0.1f);
                break;
            case AttackState:
                _animator.CrossFade("Attack", 0.1f);
                break;
        }
    }

    public override void MoveTowards(Vector2 target)
    {
        Vector2 currentPosition = transform.position;
        Vector2 direction = (target - currentPosition).normalized;
        _rigidbody2D.velocity = direction * speed;
    
        // 이미지 좌우 반전 처리
        if (direction.x != 0) // 방향이 정지 상태가 아닐 때만 처리
        {
            // 오른쪽으로 이동 시에만 이미지를 반전시키고, 왼쪽으로 이동 시에는 기본 방향을 유지
            float scaleX = Mathf.Abs(transform.localScale.x);
            transform.localScale = new Vector3((direction.x > 0 ? -scaleX : scaleX), transform.localScale.y, transform.localScale.z);
        }
    }
    
    public override bool IsPlayerDetected()
    {
        return Vector2.Distance(transform.position, GameObject.FindWithTag("Player").transform.position) <= detectionRange;
    }

    public override void SetDestination(Vector2 destination)
    {
        MoveTowards(destination);
    }

    public override void SetPatrolPoints(Vector2 pointA, Vector2 pointB)
    {
        patrolPointA = pointA;
        patrolPointB = pointB;
        EditorUtility.SetDirty(this); // 에디터에서 변경사항을 감지하도록 설정
    }
}
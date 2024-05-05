using System;
using UnityEngine;
using VInspector;

[RequireComponent(typeof(Rigidbody2D))] [RequireComponent(typeof(CapsuleCollider2D))] [RequireComponent(typeof(Animator))]
public class MobController : MonoBehaviour
{
    public interface IMobState
    {
        void Enter(MobController mob);
        void Execute();
        void Exit();
    }

    public class MobPatrolState : IMobState
    {
        private MobController _mob;
        
        private Vector2 _currentTargetPoint;
        public void Enter(MobController mob)
        {
            _mob = mob;

            _currentTargetPoint = mob.patrolPointA; // 첫 순찰 지점을 현재 목표로 설정
            
            mob.SetDestination(_currentTargetPoint); // 순찰 시작 지점으로 이동 설정
        }

        public void Execute() 
        {
            if (_mob.IsPlayerDetected())                    // 플레이어 감지
            {
                DebugEx.Log("Player Detected!");
                _mob.CurrentState = _mob._chaseState;       // 플레이어를 쫓도록 상태 변경
                return;
            }

            // 목적지 도달 여부 확인
            if (Vector2.Distance(_mob.transform.position, _currentTargetPoint) < 1f)   
            {
                SetNextPatrolPoint(); // 도달하면 다음 순찰 지점 설정
            }

            _mob.MoveTowards(_currentTargetPoint); // 순찰 중 이동 로직
        }

        public void Exit() 
        {
            // Patrol 상태에서 나갈 때 정리 로직
        }
        
        private void SetNextPatrolPoint()
        {
            // 몬스터가 현재 지점 A에 가까우면 다음 목표를 지점 B로 설정,
            // 몬스터가 현재 지점 B에 가까우면 다음 목표를 지점 A로 설정
            _currentTargetPoint = _currentTargetPoint == _mob.patrolPointA ? _mob.patrolPointB : _mob.patrolPointA;

            // 새 순찰 지점으로 이동 설정
            _mob.SetDestination(_currentTargetPoint);
        }
    }

    public class MobChaseState : IMobState
    {
        private MobController _mob;
        private Transform _playerTransform; // 플레이어의 Transform

        public void Enter(MobController mob)
        {
            _mob = mob;
            _playerTransform = Managers.Game.GetPlayer().transform; // 플레이어 Transform 찾기
        }

        public void Execute()
        {
            // 플레이어와의 거리 계산
            float distanceToPlayer = Vector2.Distance(_mob.transform.position, _playerTransform.position);

            // 플레이어가 공격 범위 내에 있으면 AttackState로 전환
            if (distanceToPlayer <= _mob.AttackRange)
            {
                _mob.CurrentState = _mob._attackState;
            }
            // 플레이어가 감지 범위 내에 있으면 플레이어를 향해 이동
            else if (distanceToPlayer <= _mob.ChasableRange)
            {
                _mob.MoveTowards(_playerTransform.position);
            }
            else // 플레이어가 감지 범위를 벗어나면 복귀 상태로 돌아감
            {
                //_mob.CurrentState = _mob._returnState;
                _mob.CurrentState = _mob._patrolState;
            }
        }

        public void Exit()
        {
            // Chase 상태에서 나갈 때 필요한 정리 로직
        }
    }

    public class MobAttackState : IMobState
    {
        private MobController _mob;
        private float _lastAttackTime;
        private float _attackCooldown = 2f; // 공격 쿨다운 시간
        
        public void Enter(MobController mob)
        {
            _mob = mob;
            
            // Attack 상태에 진입할 때 필요한 초기화 로직
            Attack();
        }
        
        public void Execute()
        {
            // Attack 상태에서의 주요 로직
            
            // 플레이어와의 거리를 계산
            float distanceToPlayer = Vector2.Distance(_mob.transform.position, Managers.Game.GetPlayer().transform.position);
            
            // 플레이어가 공격 범위 내에 있는지 확인
            if (distanceToPlayer <= _mob.AttackRange)
            {
                // 쿨다운이 지나면 다시 공격
                if (Time.time - _lastAttackTime > _attackCooldown)
                {
                    Attack();
                }
            }
            else
            {
                // 플레이어가 공격 범위를 벗어났으면 Chase 상태로 전환
                _mob.CurrentState = _mob._chaseState;
            }
        }
        
        private void Attack()
        {
            // TODO 실제 공격 로직 구현
            
            DebugEx.Log($"{_mob.gameObject.name} attacked Player!");
            _lastAttackTime = Time.time;
        }
        
        public void Exit() 
        {
            // Attack 상태에서 나갈 때 정리 로직
        }
    }

    // public class MobReturnState : IMobState
    // {
    //     private MobController _mob;
    //     private Vector2 _returnPosition;
    //     
    //     public void Enter(MobController mob)
    //     {
    //         // Return 상태에 진입할 때 필요한 초기화 로직
    //         _mob = mob;
    //         
    //         // 현재 몬스터의 위치가 순찰 지점 A와 더 가까우면 지점 B로 설정하고, 그렇지 않으면 지점 A로 설정
    //         if (Vector2.Distance(_mob.transform.position, _mob.patrolPointA) < Vector2.Distance(_mob.transform.position, _mob.patrolPointB))
    //         {
    //             _returnPosition = _mob.patrolPointA;
    //         }
    //         else
    //         {
    //             _returnPosition = _mob.patrolPointB;
    //         }
    //
    //         Debug.Log("Returning to patrol point.");
    //     }
    //
    //     public void Execute() 
    //     {
    //         // Return 상태에서의 주요 로직
    //         
    //         // 몬스터가 복귀 지점에 도달했는지 확인
    //         if (Vector2.Distance(_mob.transform.position, _returnPosition) < 1f)
    //         {
    //             Debug.Log("Reached patrol point, returning to patrol state.");
    //             _mob.CurrentState = _mob._patrolState; // 도착하면 순찰 상태로 전환
    //         }
    //         else
    //         {
    //             // 반환 지점으로 계속 이동
    //             _mob.MoveTowards(_returnPosition);
    //         }
    //     }
    //
    //     public void Exit() 
    //     {
    //         // Return 상태에서 나갈 때 정리 로직
    //     }
    // }

    [Tab("Information")]
    [SerializeField] private float speed = 400f;
    [SerializeField] public Vector2 patrolPointA;
    [SerializeField] public Vector2 patrolPointB;
    [SerializeField] private float detectionRange = 300f;
    public float DetectionRange => detectionRange;
    [SerializeField] private float chasableRange = 500f;
    public float ChasableRange => chasableRange;

    [SerializeField] private float attackRange = 150f;
    public float AttackRange => attackRange;


    [SerializeField] private IMobState _currentState;
    [SerializeField] public IMobState CurrentState
    {
        get => _currentState;
        private set
        {
            if(_currentState?.GetType() == value.GetType())  //  상태가 변하지 않았다면 업데이트하지 않는다. 
            {
                DebugEx.Log(_currentState?.GetType().ToString() + " -> " + value.GetType().ToString());
                return;
            }
            
            _currentState?.Exit();                 // 이전의 State에서 탈출
            
            _currentState = value;

            _currentState.Enter(this);      // State에 진입
            switch (_currentState)
            {
                case MobAttackState mobAttackState:
                    _animator.CrossFade("Mob_TestAttack", 0.1f);
                    break;
                case MobChaseState mobChaseState:
                    _animator.CrossFade("Mob_TestChase", 0.1f);
                    break;
                case MobPatrolState mobPatrolState:
                    _animator.CrossFade("Mob_TestPatrol", 0.1f);
                    break;
                // case MobReturnState mobReturnState:
                //     _animator.CrossFade("Mob_TestReturn", 0.1f);
                //     break;
            }
        }
    }
    
    [Tab("KeyComponents")]
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private Animator _animator;

    private MobPatrolState _patrolState = new MobPatrolState();
    private MobChaseState _chaseState = new MobChaseState();
    private MobAttackState _attackState = new MobAttackState();
    //private MobReturnState _returnState = new MobReturnState();

    void Awake()
    {
        InitKeyComponents();
        InitStates();
    }

    /// <summary>
    /// 중요한 컴포넌트 초기화
    /// </summary>
    void InitKeyComponents()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.freezeRotation = true;
        _animator = GetComponent<Animator>();
    }
    
    /// <summary>
    /// State 객체들 초기화
    /// </summary>
    private void InitStates()
    {
        _patrolState = new MobPatrolState();
        _chaseState = new MobChaseState();
        _attackState = new MobAttackState();
        //_returnState = new MobReturnState();

        CurrentState = _patrolState;
    }
    
    void FixedUpdate()
    {
        CurrentState?.Execute();
    }


    /// <summary>
    /// 플레이어와의 거리가 일정 범위 이내인지 확인
    /// </summary>
    /// <returns></returns>
    private bool IsPlayerDetected()
    {
        var player = Managers.Game.GetPlayer();
        if (player is not null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            return distanceToPlayer <= DetectionRange;
        }
        return false;
    }
    
    /// <summary>
    /// 순찰 지점 변경 시 사용
    /// </summary>
    /// <param name="destination"></param>
    private void SetDestination(Vector2 destination)
    {
        Vector2 direction = (destination - (Vector2)transform.position).normalized;
        _rigidbody2D.velocity = direction * speed;
    }
    
    /// <summary>
    /// 목표 위치까지 지속적인 이동
    /// </summary>
    /// <param name="target"></param>
    private void MoveTowards(Vector2 target)
    {
        // 현재 위치를 기준으로 목표 위치까지의 방향 벡터 계산
        Vector2 currentPosition = _rigidbody2D.position;
        Vector2 direction = (target - currentPosition).normalized;

        // 이동할 새 위치 계산. speed * Time.fixedDeltaTime을 사용하여 프레임 속도에 독립적인 이동 거리를 보장
        Vector2 newPosition = Vector2.MoveTowards(currentPosition, target, speed * Time.fixedDeltaTime);

        // Rigidbody2D를 사용하여 새 위치로 이동
        _rigidbody2D.MovePosition(newPosition);
    }
}

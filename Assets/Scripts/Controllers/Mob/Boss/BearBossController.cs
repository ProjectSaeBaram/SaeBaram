using Controllers.Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Controllers.Mob.Boss
{
    /// <summary>
    /// 곰 보스의 컨트롤러
    /// </summary>
    public class BearBossController : EntityController
    {
        public Animator animator;
        private IBearBossState _currentState;            // 현 상태
        
        public float attackRange = 200;
        
        public float idleDuration = 1.5f;                       // Idle 상태 지속 시간
        public float walkDuration = 3.0f;                       // Walk 상태 지속 시간
        public float stateStartTime;                            // 상태가 시작된 시간
        
        public float moveSpeed = 200.0f;                        // 곰 보스의 이동 속도
        public float chargeSpeed = 1000.0f;                      // 곰 보스의 돌진 속도
        
        private PlayerController player;                        // 플레이어 컨트롤러
        private Transform playerTransform;                      // 플레이어의 Transform
        private Rigidbody2D playerRigidbody;                    // 플레이어의 Rigidbody2D
        
        public UnityAction OnDamaged;                           // 곰 보스가 피해를 받을 때 Invoke

        public BossRoom bossRoom;
        
        // 피격 이펙트 시각화를 위한 필드들
        private List<SpriteRenderer> spriteRenderers;            // 스프라이트 렌더러 리스트
        private MaterialPropertyBlock propertyBlock;            // 머티리얼 프로퍼티 블록

        public List<BoxCollider2D> MeleeAttackAreas;            // 근접 공격 collider들
        public List<BoxCollider2D> HitBoxColliders;             // 피격 판정을 위한 Collider들
        
        void Start()
        {
            animator = GetComponent<Animator>();
            
            // Player 관련 컴포넌트 캐싱
            playerTransform = Managers.Game.GetPlayer().transform;
            player = playerTransform.GetComponent<PlayerController>();
            playerRigidbody = playerTransform.GetComponent<Rigidbody2D>();
            
            // EntityType 설정
            EntityType = Define.ItemMainStatType.Attack;        
            
            // 모든 자식 객체의 SpriteRenderer를 찾아서 리스트에 추가
            spriteRenderers = new List<SpriteRenderer>();
            foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
            {
                spriteRenderers.Add(sr);
            }
            
            // 시작 State는 Idle로
            ChangeState(new IdleState(0.0f));
        }

        void Update()
        {
            _currentState?.Execute();
        }
        
        /// <summary>
        /// State 변경
        /// </summary>
        /// <param name="newState"></param>
        public void ChangeState(IBearBossState newState)
        {
            // 체력이 0일때는, ExhaustedState로 가는 경우가 아니면 상태 전환이 안돼게
            if (currentHp == 0 && newState.GetType() != typeof(ExhaustedState)) return; 
            
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter(this);
            
            stateStartTime = Time.time; // 상태 전환 시 시간 기록
        }

        /// <summary>
        /// 피해를 받을 때
        /// </summary>
        /// <param name="item"></param>
        /// <param name="damage"></param>
        public override void GetHit(Handled_Item item, int damage)
        {
            currentHp  -= damage;
            OnDamaged.Invoke();
            StartCoroutine(FlashRed());                 // 피격 시 깜빡임 효과
            if (currentHp <= 0)
            {
                currentHp = 0;
                
                // 기진맥진
                ChangeState(new ExhaustedState(0.0f));
            }
            else if (currentHp <= maxHp / 2 && _currentState is not RoarState)
            {
                // 포효 시작
                ChangeState(new RoarState(0.0f));
            }
        }

        /// <summary>
        /// 데미지를 받을 때 시각화
        /// </summary>
        /// <returns></returns>
        private IEnumerator FlashRed()
        {
            // 모든 스프라이트 색상을 빨간색으로 변경
            foreach (var sr in spriteRenderers)
            {
                sr.color = Color.red;
            }
            yield return new WaitForSeconds(0.1f);  // 0.1초 대기
            // 모든 스프라이트 색상을 원래 색상으로 복원
            foreach (var sr in spriteRenderers)
            {
                sr.color = Color.white;
            }
        }
        
        /// <summary>
        /// 플레이어가 공격 범위 내에 있는지 확인
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public bool IsPlayerInAttackRange(float range)
        {
            return Vector3.Distance(transform.position, Managers.Game.GetPlayer().transform.position) < range;
        }

        /// <summary>
        /// 애니메이션이 끝났는지 여부를 확인
        /// </summary>
        /// <param name="animationName"></param>
        /// <returns></returns>
        public bool IsAnimationFinished(string animationName)
        {
            return animator.GetCurrentAnimatorStateInfo(0).IsName(animationName) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
        }

        /// <summary>
        /// 플레이어를 향해 천천히 이동하는 함수.
        /// </summary>
        public void MoveTowardsPlayer()
        {
            // 플레이어를 향해 천천히 이동하는 로직
            Vector2 playerDir = new Vector2(playerTransform.position.x, transform.position.y);
            Vector2 myPos = transform.position;
            Vector2 direction = (playerDir - myPos).normalized;
            transform.position = Vector2.MoveTowards(myPos, playerDir, moveSpeed * Time.deltaTime);

            // 스프라이트 플립
            FlipSprite(direction);
        }
        
        /// <summary>
        /// 스프라이트를 플레이어 방향으로 플립하는 함수.
        /// </summary>
        /// <param name="direction"></param>
        public void FlipSprite(Vector2 direction)
        {
            if (direction.x > 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (direction.x < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
        
        /// <summary>
        /// 플레이어를 향해 빠르게 돌진하는 함수.
        /// </summary>
        public void Charge(Vector2 targetDirection)
        {
            Vector2 myPos = transform.position;
            Vector2 targetPos = myPos + targetDirection * chargeSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(myPos, targetPos, chargeSpeed * Time.deltaTime);
        }

        /// <summary>
        /// 바위와 충돌했는지 확인하는 함수
        /// </summary>
        /// <returns></returns>
        public bool HasCollidedWithRock()
        {
            // 바위와 충돌했는지 확인하는 로직
            // 충돌 판정을 위해 필요에 따라 구현
            FallingRockController[] rocks = FindObjectsOfType<FallingRockController>();
            foreach (var rock in rocks)
            {
                if (rock.HasLanded() && rock.IsPlayerHiding())
                {
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// 모든 충돌 관련 처리
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // 현재 State에 따른 충돌 처리
            _currentState?.OnTriggerEnter2D(collision);
        }
        
        /// <summary>
        /// 에디터에서 공격 범위를 시각적으로 표시하는 함수
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            // 공격 범위 표시
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}

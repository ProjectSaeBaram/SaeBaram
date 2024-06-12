using UnityEngine;

namespace Controllers.Mob.Boss
{
    /// <summary>
    /// 곰 보스가 가질 수 있는 모든 상태의 공통 기능을 선언하는 인터페이스.
    /// </summary>
    public interface IBearBossState
    {
        void Enter(BearBossController boss);
        void Execute();
        void Exit();
        
        /// <summary>
        /// BossBear의 상태에 따른 충돌 관련 로직을 다루기 위한 메서드
        /// </summary>
        /// <param name="collision"></param>
        void OnTriggerEnter2D(Collider2D collision);
    }
    
    /// <summary>
    /// 곰 보스가 가질 수 있는 상태의 공통 부분을 정의하는 클래스
    /// 곰 보스가 가질 수 있는 상태를 추가할 때는, BearBossStateBase를 상속받아 만든다.
    /// </summary>
    public abstract class BearBossStateBase : IBearBossState
    {
        // boss 인스턴스 캐싱
        protected BearBossController boss;
        
        // 애니메이션 전환 시간
        protected float animationTransitionDuration;

        protected BearBossStateBase(float animationTransitionDuration)
        {
            this.animationTransitionDuration = animationTransitionDuration;
        }

        public virtual void Enter(BearBossController boss)
        {
            this.boss = boss;
            //DebugEx.LogWarning($"Enter {this}!");
        }

        public virtual void Execute() { }
        public virtual void Exit() { }
        
        public virtual void OnTriggerEnter2D(Collider2D collision) { }
    }

    /// <summary>   
    /// 곰 보스의 대기 상태.
    /// </summary>
    public class IdleState : BearBossStateBase
    {
        public IdleState(float animationTransitionDuration) : base(animationTransitionDuration) { }

        public override void Enter(BearBossController boss)
        {
            // boss 인스턴스 캐싱
            base.Enter(boss);
            
            boss.animator.CrossFade("Idle", animationTransitionDuration);
        }

        public override void Execute()
        {
            // 일정 시간이 지나면 Walk 상태로 전환.
            if (Time.time - boss.stateStartTime > boss.idleDuration)
            {
                boss.ChangeState(new WalkState(0.0f));
            }
        }
    }

    /// <summary>
    /// 곰 보스의 걷기 상태.
    /// </summary>
    public class WalkState : BearBossStateBase
    {
        public WalkState(float animationTransitionDuration) : base(animationTransitionDuration) { }

        public override void Enter(BearBossController boss)
        {
            // boss 인스턴스 캐싱
            base.Enter(boss);
            
            // 애니메이터를 Walk 상태로 설정.
            boss.animator.CrossFade("Walk", animationTransitionDuration);
        }

        public override void Execute()
        {
            // 플레이어를 향해 이동.
            boss.MoveTowardsPlayer();

            // 플레이어가 공격 범위 내에 있으면 Melee Attack 상태로 전환
            if (boss.IsPlayerInAttackRange(boss.attackRange))
            {
                boss.ChangeState(new MeleeAttackState(0.0f));
            }
            // 일정 시간이 지나면 Prepare Charge 상태로 전환
            else if (Time.time - boss.stateStartTime > boss.walkDuration)
            {
                boss.ChangeState(boss.currentHp <= boss.maxHp / 2 ? new RoarState(0.0f) : new PrepareChargeState(0.0f));
            }
        }
    }

    /// <summary>
    /// 곰 보스의 근접 공격 상태.
    /// </summary>
    public class MeleeAttackState : BearBossStateBase
    {
        private int meleeAttackDamage = 25;         // 근접 공격 데미지
        private int meleeAttackForce = 1500;        // 근접 공격 시 밀쳐내는 힘
        
        public MeleeAttackState(float animationTransitionDuration) : base(animationTransitionDuration) { }
        
        public override void Enter(BearBossController boss)
        {
            // 보스 인스턴스를 캐싱.
            base.Enter(boss);
            
            // 애니메이터를 MeleeAttack 상태로 설정.
            boss.animator.CrossFade("MeleeAttack", animationTransitionDuration);
        }

        public override void Execute()
        {
            // 공격이 끝나면 다시 Idle 상태로 전환.
            if (boss.IsAnimationFinished("MeleeAttack"))
            {
                boss.ChangeState(new IdleState(0.0f));
            }
        }
        
        public override void OnTriggerEnter2D(Collider2D collision)
        {
            // 근접 공격 중 플레이어와 충돌
            if (collision.CompareTag("Player"))
            {
                PlayerController player = collision.GetComponent<PlayerController>();
                
                // 근접 공격 영역과 닿았는지 검사
                foreach (BoxCollider2D attackArea in boss.MeleeAttackAreas)
                {
                    if (collision.IsTouching(attackArea))
                    {
                        // 플레이어 데미지 주고 밀쳐내기
                        player.GetHit(meleeAttackDamage);
                        Vector2 pushDirection = (player.transform.position - boss.transform.position).normalized;
                        player.GetRigidBody2D().AddForce(pushDirection * meleeAttackForce, ForceMode2D.Impulse);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 곰 보스의 돌진 준비 상태.
    /// </summary>
    public class PrepareChargeState : BearBossStateBase
    {
        public PrepareChargeState(float animationTransitionDuration) : base(animationTransitionDuration) { }
        
        public override void Enter(BearBossController boss)
        {
            // 보스 인스턴스를 캐싱.
            base.Enter(boss);
            
            // 애니메이터를 PrepareCharge 상태로 설정.
            boss.animator.CrossFade("PrepareCharge", animationTransitionDuration);
            
            // AirGround 소환
            boss.bossRoom.SpawnAirGrounds();
        }

        public override void Execute()  
        {
            // 준비 동작이 끝나면 Charge 상태로 전환.
            if (boss.IsAnimationFinished("PrepareCharge"))
            {
                boss.ChangeState(new ChargeState(0.0f));
            }
        }
    }

    /// <summary>
    /// 곰 보스의 돌진 상태.
    /// </summary>
    public class ChargeState : BearBossStateBase
    {
        private Vector2 chargeDirection;            // 돌진하는 방향 
        
        private int chargeDamage = 40;              // 돌진 데미지
        private int chargePushForce = 1500;         // 돌진 시 밀쳐내는 힘

        private int damageWhenChargeIntoRock = 20;  // 바위에 돌진 시 받는 데미지
        
        public ChargeState(float animationTransitionDuration) : base(animationTransitionDuration) { }
        
        public override void Enter(BearBossController boss)
        {
            // 보스 인스턴스를 캐싱.
            base.Enter(boss);
            
            // 애니메이터를 Charge 상태로 설정.
            boss.animator.CrossFade("Charge", animationTransitionDuration);
            
            // Charge 상태 진입 시 플레이어의 방향을 인식.
            Transform playerTransform = Managers.Game.GetPlayer().transform;
            Vector2 playerDir = new Vector2(playerTransform.position.x, boss.transform.position.y);
            Vector2 myPos = boss.transform.position;
            chargeDirection = (playerDir - myPos).normalized;
            
            // 스프라이트 플립
            boss.FlipSprite(chargeDirection);
        }
        
        public override void Execute()
        {
            // 돌진
            boss.Charge(chargeDirection);
        }   
        
        public override void OnTriggerEnter2D(Collider2D collision)
        {
            // 돌진 중 플레이어와 충돌 시 데미지를 주고 밀쳐내기
            if (collision.CompareTag("Player"))
            {
                PlayerController player = collision.GetComponent<PlayerController>();
                
                player.GetHit(chargeDamage);
                Vector2 pushDirection = (player.transform.position - boss.transform.position).normalized;
                player.GetRigidBody2D().AddForce(pushDirection * chargePushForce, ForceMode2D.Impulse);
            }
            // 돌진 중에 BossChargeStopper와 닿으면 돌진을 멈춘다
            else if (collision.CompareTag("BossChargeStopper"))
            {
                boss.ChangeState(new IdleState(0.0f));
                
                // AirGround 소환 해제 
                boss.bossRoom.DespawnAirGrounds();
                
                // 바위들 소환 해제
                boss.bossRoom.DespawnFallingRocks();
            }
            // 플레이어가 숨어있는 바위에 충돌했다면 ChargeAfterEffectState로 상태전환
            else if (collision.GetComponent<FallingRockController>() != null)
            {
                FallingRockController fallingRock = collision.GetComponent<FallingRockController>();
                
                if (fallingRock.HasLanded() && fallingRock.IsPlayerHiding())
                {
                    boss.GetHit(null, damageWhenChargeIntoRock);
                    boss.ChangeState(new ChargeAfterEffectState(0.0f));
                }
            }
        }
    }

    /// <summary>
    /// 곰 보스의 돌진 후유증 상태
    /// </summary>
    public class ChargeAfterEffectState : BearBossStateBase
    {
        public ChargeAfterEffectState(float animationTransitionDuration) : base(animationTransitionDuration) { }

        public override void Enter(BearBossController boss)
        {
            // 보스 인스턴스를 캐싱.
            base.Enter(boss);
            // 애니메이터를 ChargeAfterEffect 상태로 설정.
            boss.animator.CrossFade("ChargeAfterEffect", animationTransitionDuration);
            
            // AirGround 소환 해제
            boss.bossRoom.DespawnAirGrounds();
            
            // 바위들 소환 해제
            boss.bossRoom.DespawnFallingRocks();
        }

        public override void Execute()
        {
            // 충돌 후 Idle 상태로 전환.
            if (boss.IsAnimationFinished("ChargeAfterEffect"))
            {
                boss.ChangeState(new IdleState(0.0f));
            }
        }
    }

    /// <summary>
    /// 곰 보스의 포효 상태.
    /// </summary>
    public class RoarState : BearBossStateBase
    {
        public RoarState(float animationTransitionDuration) : base(animationTransitionDuration) { }
        
        public override void Enter(BearBossController boss)
        {
            // 보스 인스턴스를 캐싱.
            base.Enter(boss);
            // 애니메이터를 Roar 상태로 설정.
            boss.animator.CrossFade("Roar", animationTransitionDuration);
            
            // 추락하는 바위 생성
            boss.bossRoom.SpawnFallingRocks();
        }

        public override void Execute()
        {
            // 포효가 끝나면 다시 PrepareCharge 상태로 전환.
            if (boss.IsAnimationFinished("Roar"))
            {
                boss.ChangeState(new PrepareChargeState(0.0f));
            }
        }
    }

    /// <summary>
    /// 곰 보스의 기진맥진 상태.
    /// </summary>
    public class ExhaustedState : BearBossStateBase
    {
        public ExhaustedState(float animationTransitionDuration) : base(animationTransitionDuration) { }

        public override void Enter(BearBossController boss)
        {
            // 보스 인스턴스를 캐싱.
            base.Enter(boss);
            // 애니메이터를 Exhausted 상태로 설정.
            boss.animator.CrossFade("Exhausted", animationTransitionDuration);
            
            boss.bossRoom.OnBossDied?.Invoke();
        }
    }
}

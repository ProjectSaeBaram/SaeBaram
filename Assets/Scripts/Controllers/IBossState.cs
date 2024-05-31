using UnityEngine;

public interface IBossState {
    void Enter();
    void Execute();
    void Exit();
}

public class BossIdleState : IBossState
{
    BossController _boss;
    public BossIdleState(BossController boss) {
        this._boss = boss;
    }
    
    public void Enter()
    {
        throw new System.NotImplementedException();
    }

    public void Execute()
    {
        throw new System.NotImplementedException();
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }
}


public class BossSwipeState : IBossState {
    BossController boss;
    float attackDuration = 1.5f; // 공격 지속 시간
    float timer;

    public BossSwipeState(BossController boss) {
        this.boss = boss;
    }

    public void Enter() {
        // boss.animator.Play("Swipe");
        timer = attackDuration;
    }

    public void Execute() {
        timer -= Time.deltaTime;
        if (timer <= 0) {
            boss.ChangeState(boss.idleState); // 공격 완료 후 Idle 상태로 복귀
        }
    }

    public void Exit() { }
}

public class BossChargeState : IBossState {
    BossController boss;
    float chargeSpeed = 20f;

    public BossChargeState(BossController boss) {
        this.boss = boss;
    }

    public void Enter() {
        // boss.animator.Play("Charge");
        // boss.rigidbody2D.velocity = new Vector2(boss.transform.localScale.x * chargeSpeed, 0);
    }

    public void Execute() {
        if (boss.transform.position.x < -10 || boss.transform.position.x > 10) {
            // boss.rigidbody2D.velocity = Vector2.zero; // 맵 끝에 도달하면 멈춤
            boss.ChangeState(boss.idleState);
        }
    }

    public void Exit() {
    }
}

public class BossRoarState : IBossState {
    BossController boss;

    public BossRoarState(BossController boss) {
        this.boss = boss;
    }
    public void Enter()
    {
        // boss.animator.Play("Roar");
        // Trigger falling rocks
    }

    public void Execute() {
        // Check for hiding player and perform actions
    }

    public void Exit() {
    }
}



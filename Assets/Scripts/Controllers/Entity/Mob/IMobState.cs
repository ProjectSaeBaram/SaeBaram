using UnityEngine;

public interface IMobState
{
    void Enter(MobController mob);
    void Execute();
    void Exit();
}

public class PatrolState : IMobState
{
    private MobController mob;
    private Vector2 currentTargetPoint;
    private Vector2 patrolPointA;
    private Vector2 patrolPointB;

    public PatrolState(Vector2 pointA, Vector2 pointB)
    {
        patrolPointA = pointA;
        patrolPointB = pointB;
        currentTargetPoint = patrolPointA;
    }

    public void Enter(MobController mob)
    {
        this.mob = mob;
        mob.SetDestination(currentTargetPoint);
    }

    public void Execute()
    {
        if (mob.IsPlayerDetected())
        {
            mob.ChangeState(new ChaseState());
            return;
        }

        if (Vector2.Distance(mob.transform.position, currentTargetPoint) < 10f)
        {
            currentTargetPoint = (currentTargetPoint == patrolPointA) ? patrolPointB : patrolPointA;
            mob.SetDestination(currentTargetPoint);
        }

        mob.MoveTowards(currentTargetPoint);
    }

    public void Exit()
    {
        // Cleanup or transition logic
    }
}

public class ChaseState : IMobState
{
    private MobController mob;
    private Transform playerTransform;

    public void Enter(MobController mob)
    {
        this.mob = mob;
        playerTransform = Managers.Game.GetPlayer().transform;
    }

    public void Execute()
    {
        float distanceToPlayer = Vector2.Distance(mob.transform.position, playerTransform.position);
        if (distanceToPlayer <= mob.GetAttackRange())
        {
            mob.ChangeState(new AttackState());
        }
        else if (distanceToPlayer > mob.GetChasableRange()) // 플레이어 추격 가능 거리를 넘어서면
        {
            Vector2 pointA = mob.GetPatrolPointA();
            Vector2 pointB = mob.GetPatrolPointB();
            mob.ChangeState(new PatrolState(pointA, pointB)); // 순찰 상태로 돌아가기
        }
        else
        {
            mob.MoveTowards(playerTransform.position);
        }
    }

    public void Exit()
    {
        // Cleanup logic
    }
}

public class AttackState : IMobState
{
    private MobController mob;
    private float lastAttackTime;
    private float attackCooldown = 2f;

    public void Enter(MobController mob)
    {
        this.mob = mob;
        Attack();
    }

    public void Execute()
    {
        float distanceToPlayer = Vector2.Distance(mob.transform.position, GameObject.FindWithTag("Player").transform.position);
        if (distanceToPlayer <= mob.GetAttackRange())
        {
            if (Time.time - lastAttackTime > attackCooldown)
            {
                Attack();
            }
        }
        else
        {
            mob.ChangeState(new ChaseState()); // 플레이어가 공격 범위를 벗어나면 추격 상태로 변경
        }
    }

    public void Exit()
    {
        // Cleanup logic
    }

    private void Attack()
    {
        Debug.Log(mob.name + " attacks player!");
        lastAttackTime = Time.time;
    }
}

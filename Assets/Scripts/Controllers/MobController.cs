using UnityEngine;

public abstract class MobController : MonoBehaviour
{
    protected IMobState currentState;

    public IMobState GetCurrentState()
    {
        return currentState;
    }
    
    public abstract void ChangeState(IMobState newState);
    public abstract float GetDetectionRange();
    public abstract float GetAttackRange();
    public abstract float GetChasableRange();
    public abstract Vector2 GetPatrolPointA();
    public abstract Vector2 GetPatrolPointB();
    public abstract void SetDestination(Vector2 destination);
    public abstract void SetPatrolPoints(Vector2 pointA, Vector2 pointB);
    public abstract bool IsPlayerDetected();
    public abstract void MoveTowards(Vector2 target);
}

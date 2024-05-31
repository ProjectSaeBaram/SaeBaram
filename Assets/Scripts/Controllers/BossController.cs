using UnityEngine;
using UnityEngine.Events;

public class BossController : MonoBehaviour
{
    private int maxHealth = 100;
    private int currentHealth = 100;

    public UnityAction OnDamaged;
    public UnityAction OnDie;

    private bool roarIntroduced = false;
    
    public int MaxHealthGetter()
    {
        return maxHealth;
    }
    
    public int CurrentHealthGetter()
    {
        return currentHealth;
    }
    
    public IBossState currentState;
    public BossIdleState idleState;
    public BossSwipeState swipeState;
    public BossChargeState chargeState;
    public BossRoarState roarState;
    
    void Start() {
        idleState = new BossIdleState(this);
        swipeState = new BossSwipeState(this);
        chargeState = new BossChargeState(this);
        roarState = new BossRoarState(this);
        currentState = idleState;
    }
    
    void Update() {
        if (currentHealth <= maxHealth * 0.5 && !roarIntroduced) {
            roarIntroduced = true;
            ChangeState(roarState);
        }
    
        currentState.Execute();
    }
    
    public void ChangeState(IBossState newState) {
        currentState.Exit();
        currentState = newState;
        newState.Enter();
    }
    
    public void OnAnimationTransition(string stateName) {
        switch (stateName) {
            case "Swipe":
                ChangeState(swipeState);
                break;
            case "Charge":
                ChangeState(chargeState);
                break;
            case "Roar":
                ChangeState(roarState);
                break;
            default:
                ChangeState(idleState);
                break;
        }
    }
    
    void GetDamage(int damage)
    {
        currentHealth -= damage;
        OnDamaged.Invoke();
        if (currentHealth <= 0) OnDie.Invoke();
    }
}

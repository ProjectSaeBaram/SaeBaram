using Controllers.Mob.Boss;
using UnityEngine;
using UnityEngine.UI;

public class UI_BossHPBar : UI_Base
{
    [SerializeField] private Image BackGround;
    [SerializeField] private Image CurrentHPImage;

    [SerializeField] private float CurrentHP;

    private BearBossController _bossController;
    
    public override void Init()
    {
    }

    public void SetBossController(BearBossController boss)
    {
        _bossController = boss;
        VisualizeCurrentHealth();
        
        // Boss의 체력이 변할 때마다 UI 갱신 
        _bossController.OnDamaged += VisualizeCurrentHealth;
    }
    
    public void VisualizeCurrentHealth()
    {
        float max = _bossController.maxHp;
        float current = _bossController.currentHp;
        CurrentHP = current;
        CurrentHPImage.fillAmount = current / max;
    }
}

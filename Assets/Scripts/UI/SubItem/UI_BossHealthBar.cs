using UnityEngine;
using UnityEngine.UI;

public class UI_BossHPBar : UI_Base
{
    [SerializeField] private Image BackGround;
    [SerializeField] private Image CurrentHPImage;

    [SerializeField] private int CurrentHP;

    private BossController _bossController;
    
    public override void Init()
    {
        _bossController = FindObjectOfType<BossController>();

        VisualizeCurrentHealth();
        _bossController.OnDamaged += VisualizeCurrentHealth;
    }
    
    private void VisualizeCurrentHealth()
    {
        int max = _bossController.MaxHealthGetter();
        int current = _bossController.CurrentHealthGetter();
        CurrentHP = current;
        CurrentHPImage.fillAmount = current / max;
    }

}

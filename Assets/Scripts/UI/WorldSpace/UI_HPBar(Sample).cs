using UnityEngine;
using UnityEngine.UI;

public class UI_HPBar : UI_Base
{
    enum GameObjects
    {
        HPBar,
    }

    private Transform parent;
    private Collider _parentCollider;
    private Stat _stat;
    private Slider _slider;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));

        parent = transform.parent;
        _parentCollider = parent.GetComponent<Collider>();
        _stat = parent.GetComponent<Stat>();
        _slider = GetObject((int)GameObjects.HPBar).gameObject.GetComponent<Slider>();
    }

    private void Update()
    {
        // 체력바의 위치와 회전을 코드로 조정
        // 플레이어 캐릭터의 충돌체 높이보다 살짝 더 위로 띄워서
        transform.position = parent.position + Vector3.up * (_parentCollider.bounds.size.y + 0.2f);

        // 체력바의 회전값을 카메라와 동일하게
        transform.rotation = Camera.main.transform.rotation;
        // transform.lookAt(Camera.main.transform) 는 체력바의 좌우가 뒤집힌다.

        // 실제 체력과 slider를 동기화
        float ratio = _stat.MaxHp == 0 ? 0 : (_stat.Hp / (float)_stat.MaxHp);       // 소수부 소실방지를 위한 타입 캐스팅
        SetHpRatio(ratio);
    }

    void SetHpRatio(float ratio)
    {
        _slider.value = ratio;
    }
}

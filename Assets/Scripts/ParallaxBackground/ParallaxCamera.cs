using UnityEngine;

/// <summary>
/// 카메라의 이동을 감지하고 패럴랙스 효과를 적용하는 이벤트를 발생시키는 클래스
/// </summary>
public class ParallaxCamera : MonoBehaviour
{
    /// <summary>
    /// 카메라가 이동할 때 호출되는 델리게이트
    /// </summary>
    /// <param name="deltaMovement">카메라의 이동 거리</param>
    public delegate void ParallaxCameraDelegate(float deltaMovement);
    
    /// <summary>
    /// 카메라가 이동할 때 호출되는 이벤트.
    /// </summary>
    public ParallaxCameraDelegate onCameraTranslate;

    private float oldPosition;

    /// <summary>
    /// 스크립트가 시작될 때 호출되며, 초기 카메라 위치를 설정.
    /// </summary>
    void Start()
    {
        oldPosition = transform.position.x;
    }

    /// <summary>
    /// 매 프레임마다 호출되며, 카메라가 이동했는지 확인하고 이벤트를 발생시킴.
    /// </summary>
    void LateUpdate()
    {
        // 카메라 이동 이벤트를 호출
        if (onCameraTranslate != null)
        {
            float delta = oldPosition - transform.position.x;
            onCameraTranslate(delta);
        }

        // 현재 위치를 이전 위치로 업데이트
        oldPosition = transform.position.x;
    }
}
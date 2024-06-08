using Cinemachine;
using UnityEngine;

/// <summary>
/// Parallax 이동을 위한 Background
/// 구름의 이동을 추가적으로 구현함
/// </summary>
public class ParallaxCloudLayer : ParallaxLayer
{
    /// <summary>
    /// 구름이 바람에 의해 움직이는 속도
    /// </summary>
    public float windSpeed;
    
    private float spriteWidth;
    private Transform parentTransform;
    
    private CinemachineVirtualCamera cinemachineCamera;
    
    void Start()
    {
        // 스프라이트 너비를 저장.
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;

        // Cinemachine 카메라.
        cinemachineCamera = FindObjectOfType<CinemachineVirtualCamera>();
        
        // 부모 객체를 설정.
        parentTransform = transform.parent;
        if (parentTransform.GetComponent<ParallaxCloudLayer>() != null)
        {
            windSpeed = 0;
        }
    }
    
    /// <summary>
    /// 기본 패럴랙스 이동 외에도 바람에 의한 이동을 추가로 처리.
    /// </summary>
    /// <param name="delta">카메라 이동 거리</param>
    public override void Move(float delta)
    {
        Vector3 newPos = transform.localPosition;
        newPos.x -= delta * parallaxFactor + windSpeed * Time.deltaTime;
        transform.localPosition = newPos;
    }
}
using UnityEngine;

/// <summary>
/// Parallax 이동을 위한 Background
/// </summary>
public class ParallaxLayer : MonoBehaviour
{
    /// <summary>
    /// 패럴랙스 이동 속도를 결정하는 인자.
    /// 음수일 때는 반대로 움직인다.
    /// </summary>
    public float parallaxFactor;

    /// <summary>
    /// Layer를 실제로 이동시키는 메서드
    /// </summary>
    /// <param name="delta">카메라 이동 거리</param>
    public virtual void Move(float delta)
    {
        Vector3 newPos = transform.localPosition;
        newPos.x -= delta * parallaxFactor;
        transform.localPosition = newPos;
    }
}
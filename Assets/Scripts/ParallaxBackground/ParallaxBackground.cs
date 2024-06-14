using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parallax Background을 관리하는 클래스
/// </summary>
public class ParallaxBackground : MonoBehaviour
{
    /// <summary>
    /// Parallax 이동을 처리하는 카메라
    /// </summary>
    public ParallaxCamera parallaxCamera;
    
    /// <summary>
    /// Parallax Layer들을 관리하는 리스트.
    /// </summary>
    List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>();

    /// <summary>
    /// 카메라와 레이어를 설정
    /// </summary>
    void Start()
    {
        if (parallaxCamera == null)
            parallaxCamera = Camera.main.GetComponent<ParallaxCamera>();

        if (parallaxCamera != null)
            parallaxCamera.onCameraTranslate += Move;

        SetLayers();
    }

    /// <summary>
    /// 자식 오브젝트에서 패럴랙스 레이어를 설정
    /// </summary>
    void SetLayers()
    {
        parallaxLayers.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            ParallaxLayer layer = transform.GetChild(i).GetComponent<ParallaxLayer>();

            if (layer != null)
            {
                layer.name = "Layer-" + i;
                parallaxLayers.Add(layer);

                // 새로운 배경 이미지 복제
                DuplicateBackground(layer);
            }
        }
    }

    /// <summary>
    /// 카메라가 이동할 때 레이어를 이동시키는 메서드
    /// </summary>
    /// <param name="delta">카메라 이동 거리</param>
    void Move(float delta)
    {
        foreach (ParallaxLayer layer in parallaxLayers)
        {
            layer.Move(delta);
            CheckBounds(layer);
        }
    }

    /// <summary>
    /// 레이어의 배경 이미지를 양쪽으로 복제하여 무한 배경을 구현.
    /// </summary>
    /// <param name="layer">복제할 레이어</param>
    void DuplicateBackground(ParallaxLayer layer)
    {
        SpriteRenderer spriteRenderer = layer.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Vector3 layerPosition = layer.transform.position;
            float spriteWidth = spriteRenderer.bounds.size.x;

            // 이미 생성된 복제본이 있는지 확인
            if (layer.transform.childCount == 0)
            {
                GameObject leftClone = Instantiate(layer.gameObject, new Vector3(layerPosition.x - spriteWidth, layerPosition.y, layerPosition.z), Quaternion.identity, layer.transform);
                leftClone.transform.localScale = Vector3.one;
                GameObject rightClone = Instantiate(layer.gameObject, new Vector3(layerPosition.x + spriteWidth, layerPosition.y, layerPosition.z), Quaternion.identity, layer.transform);
                rightClone.transform.localScale = Vector3.one;

                // 이름 변경으로 중복 방지
                leftClone.name = layer.name + "-LeftClone";
                rightClone.name = layer.name + "-RightClone";

                // 복제본의 자식 오브젝트를 삭제하여 무한 복제 방지
                foreach (Transform child in leftClone.transform)
                {
                    Destroy(child.gameObject);
                }
                foreach (Transform child in rightClone.transform)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// 레이어가 카메라 시야를 벗어났을 때 위치를 재배치하여 무한 배경을 구현.
    /// </summary>
    /// <param name="layer">위치를 체크할 레이어</param>
    void CheckBounds(ParallaxLayer layer)
    {
        SpriteRenderer spriteRenderer = layer.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Vector3 cameraPosition = parallaxCamera.transform.position;
            Vector3 layerPosition = layer.transform.position;
            float spriteWidth = spriteRenderer.bounds.size.x;

            if (cameraPosition.x > layerPosition.x + spriteWidth)
            {
                layer.transform.position = new Vector3(layerPosition.x + spriteWidth * 2, layerPosition.y, layerPosition.z);
            }
            else if (cameraPosition.x < layerPosition.x - spriteWidth)
            {
                layer.transform.position = new Vector3(layerPosition.x - spriteWidth * 2, layerPosition.y, layerPosition.z);
            }
        }
    }
}

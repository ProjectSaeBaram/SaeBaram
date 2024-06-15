using Cinemachine;
using UnityEngine;

public class CinemachineBlendController : MonoBehaviour
{
    [SerializeField] CinemachineBlendListCamera blendList;
    
    [SerializeField] CinemachineVirtualCameraBase originCamera;
    [SerializeField] CinemachineVirtualCameraBase BearBossFocusCamera;

    void Awake()
    {
        blendList = GetComponent<CinemachineBlendListCamera>();
        blendList.m_Loop = false;
    }
    
    public void LookBear()
    {
        // DebugEx.LogError("LookBear");
        
        originCamera.transform.SetParent(transform);
        BearBossFocusCamera.transform.SetParent(transform);

        blendList.m_Instructions[0].m_VirtualCamera = originCamera;
        blendList.m_Instructions[1].m_VirtualCamera = BearBossFocusCamera;

        blendList.m_Instructions[1].m_Blend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
        blendList.m_Instructions[1].m_Blend.m_Time = 2.0f;

        blendList.m_Instructions[0].m_Hold = 1.0f;
    }

    public void LookPlayer()
    {
        // DebugEx.LogError("LookPlayer");
        
        originCamera.transform.SetParent(transform);
        BearBossFocusCamera.transform.SetParent(transform);

        blendList.m_Instructions[0].m_VirtualCamera = BearBossFocusCamera;
        blendList.m_Instructions[1].m_VirtualCamera = originCamera;

        blendList.m_Instructions[1].m_Blend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
        blendList.m_Instructions[1].m_Blend.m_Time = 2.0f;

        blendList.m_Instructions[0].m_Hold = 1.0f;
    }

    public void Exit()
    {
        originCamera.transform.SetParent(null);
        gameObject.SetActive(false);
    }

}

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings_SoundPercentage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mySelf;
    [SerializeField] private Scrollbar slider;
    
    /// <summary>
    /// 사운드 Percentage 텍스트와 슬라이더를 동기화하는 함수.
    /// SoundManager의 GlobalSoundVolume과도 동기화한다.
    /// </summary>
    public void UpdatePercentageText()
    {
        mySelf.text = $"{(int)(slider.value * 100)}%";
        
        Managers.Sound.GlobalSoundVolume = slider.value;
        
        DebugEx.LogWarning($"current volume is {mySelf.text}");

        Managers.Sound.ChangeGlobalSoundVolume();
    }
}

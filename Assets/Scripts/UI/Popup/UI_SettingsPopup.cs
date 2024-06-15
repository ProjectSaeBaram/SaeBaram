using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SettingsPopup : UI_Popup
{
    // 모니터가 지원하는 해상도를 담은 리스트
    private List<Resolution> _resolutions = new List<Resolution>();
    [SerializeField] private int resolutionNum;                 // 현재 선택된 해상도 index를 저장하는 int
    [SerializeField] private FullScreenMode screenMode;         // 창모드 여부를 저장하는 enum
    
    enum Images
    {
        Background,
    }

    enum Dropdowns
    {
        ResolutionDropdown,
    }

    enum Toggles
    {
        FullScreenToggle,
    }

    enum Buttons
    {
        UpdateSettingBtn,
    }
    public override void Init()
    {
        base.Init();
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));
        Bind<TMP_Dropdown>(typeof(Dropdowns));
        Bind<Toggle>(typeof(Toggles));
        
        Toggle toggle = Get<Toggle>((int)Toggles.FullScreenToggle);
        toggle.onValueChanged.AddListener(FullScreenBtn);
        
        Get<TMP_Dropdown>((int)Dropdowns.ResolutionDropdown).onValueChanged.AddListener(DropboxOptionChanged);
        var dd = Get<TMP_Dropdown>((int)Dropdowns.ResolutionDropdown);
        dd.gameObject.AddUIEvent(OnDropdownClicked);
        SetResolutions(_resolutions);
        GetResolutions(_resolutions, dd, toggle);
        
        GetImage((int)Images.Background).gameObject.AddUIEvent(ClosePopupUI);
        GetButton((int)Buttons.UpdateSettingBtn).gameObject.AddUIEvent(UpdateSettingBtn);
    }

    /// <summary>
    ///  현재 모니터에서 지원하는 모든 해상도를 list에 받아오는 함수
    /// </summary>
    /// <param name="list"></param>
    void SetResolutions(List<Resolution> list)
    {
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].refreshRateRatio.value.ConvertTo<int>() == 60
                || Screen.resolutions[i].refreshRateRatio.value.ConvertTo<int>() == 144)
                // 프레임이 60 혹은 144가 지원되는 해상도만 가져온다
            {
                list.Add(Screen.resolutions[i]);
            }
        }
    }

    /// <summary>
    /// 모든 해상도를 list에서 받아와 dropdown에 집어넣는 함수
    /// </summary>
    /// <param name="list"></param>
    /// <param name="dropdown"></param>
    /// <param name="toggle"></param>
    void GetResolutions(List<Resolution> list,TMP_Dropdown dropdown, Toggle toggle)
    {
        dropdown.options.Clear();     // 담겨있는 옵션들을 초기화하고
        for(int i = 0; i < list.Count; i++)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = $"{list[i].width} X {list[i].height} ({list[i].refreshRateRatio.value.ConvertTo<int>()})";
            dropdown.options.Add(option);       // 리스트에 담긴 모든 해상도를 option으로 만들어 넣는다

            // $"{res.width} X {res.height} ({res.refreshRateRatio.value.ConvertTo<int>()})"
            // 그냥 res.refreshRateRatio는
            // res.refreshRateRatio.value와 같고, 이는 double 타입.

            if (list[i].width == Screen.width && list[i].height == Screen.height)
                dropdown.value = i;
        }
        dropdown.RefreshShownValue();       // 새로고침

        toggle.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow);        // 현재 스크린의 화면 설정을 반영
    }

    /// <summary>
    /// Dropdown의 Option을 담는 item이 viewport에 어색하게 짤려보이는 현상을 막기위한 함수
    /// </summary>
    void OnDropdownClicked(PointerEventData data)
    {
        var go = Util.FindChild(Util.FindChild(gameObject, "Dropdown List",true), "Content",true);
        VerticalLayoutGroup vlg = go.GetOrAddComponent<VerticalLayoutGroup>();
        vlg.spacing = 3;
    }

    void DropboxOptionChanged(int x)
    {
        resolutionNum = x;
    }
    
    void FullScreenBtn(bool isFull)
    {
        screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }
    
    void UpdateSettingBtn(PointerEventData action)
    {
        Screen.SetResolution(_resolutions[resolutionNum].width, _resolutions[resolutionNum].height, screenMode);
        // 위에서 설정한 해상도 옵션을 실제로 적용
        ClosePopupUI(action);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptDialogueManager 
{
    public bool dialogueIsPlaying { get; private set; }             //현재 대화창에 진입했는지 확인할 변수
    public PlayerController playerController;
    [SerializeField] public GameObject[] QuickSlot = new GameObject[3];

    public UI_DialoguePopup prevPopup = null;

    private Sprite[] portrait_player;
    private Sprite portrait_bear;

    public void Init()
    {
        portrait_player = Resources.LoadAll<Sprite>("Sprites/Character/Player_portrait");
        portrait_bear = Managers.Resource.Load<Sprite>("Sprites/Character/Bear_Portrait");
    }
    
    public void GetTalk(int idx,string speaker, string context, string emotion)
    {
        if (prevPopup != null)
        {
            prevPopup.ClosePopupUI(null);
        }
        
        dialogueIsPlaying = true;
        UI_DialoguePopup popup = Managers.UI.ShowPopupUI<UI_DialoguePopup>();

        //태그 초기화
        popup.displayNameText.text = speaker;
        popup.dialogueText.text = context;
        popup.portraitImage.sprite = GetSpriteByName(speaker, emotion);
        
        // 기존 스프라이트의 크기를 가져옵니다.
        float originalWidth = popup.portraitImage.sprite.rect.width;
        float originalHeight = popup.portraitImage.sprite.rect.height;
        
        // 새로운 크기를 설정합니다.
        popup.portraitImage.GetComponent<RectTransform>().sizeDelta = new Vector2(originalWidth, originalHeight);
        
        prevPopup = popup;
    }

    /// <summary>
    /// 화자의 이름에 따라 적절한 초상화 Sprite를 찾아주는 함수
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Sprite GetSpriteByName(string name, string emotion)
    {
        switch (name)
        {
            case "곰": 
                return portrait_bear;
            case "가온":
                {
                    foreach (var sprite in portrait_player)
                        if (sprite.name == $"Player_portrait_{emotion}") return sprite;
                }
                break;
            default:
                DebugEx.LogWarning("Speaker Unidentified!");
                return null;
        }
        

        return null;
    }

}

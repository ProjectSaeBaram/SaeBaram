using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptDialogueManager 
{
    public UI_DialoguePopup popup;

    public bool dialogueIsPlaying { get; private set; }             //현재 대화창에 진입했는지 확인할 변수
    public PlayerController playerController;
    [SerializeField] public GameObject[] QuickSlot = new GameObject[3];

    private void Update()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }
        // 상호작용 키를 눌렀을 때의 동작을 수정
        if ( playerController.GetInteractPressed())
        {
            // 선택지가 없을 때만 ContinueStory를 호출
            ContinueStory();
        }
    }

    private void ContinueStory()
    {
        throw new NotImplementedException();
    }

    public void GetTalk(string idx,string stext, string npcnname)
    {
        dialogueIsPlaying = true;
        Managers.UI.ShowPopupUI<UI_DialoguePopup>();
        popup.dialoguePanel.SetActive(true);
        foreach (var quick in QuickSlot)
        {
            quick.gameObject.SetActive(false);
        }
        //태그 초기화
        popup.displayNameText.text = npcnname;
        popup.dialogueText.text = stext;
        popup.portraitImage.sprite = null;
        ContinueStory();
    }
}

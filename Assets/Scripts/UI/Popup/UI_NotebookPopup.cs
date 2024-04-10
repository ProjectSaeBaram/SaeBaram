using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_NotebookPopup : UI_Popup
{
    enum GameObjects
    {
        CraftingLayer,
        ReinforcingLayer,
        QuestLayer,
        DiaryLayer,
        MapLayer,
        LastPage,
    }

    enum Images
    {
        BackPanel,
        NoteBook_Background,
    }
    
    enum Buttons
    {
        Bookmark_CraftingLayer,
        Bookmark_ReinforcingLayer,
        Bookmark_QuestLayer,
        Bookmark_DiaryLayer,
        Bookmark_MapLayer,
        Bookmark_LastPage,
    }

    // Key : Bookmark, Value : Layer
    private Dictionary<Buttons, GameObjects> BookmarksAndLayers = new();

    void Start()
    {
        Init();
    }
    
    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));

        // 북마크들을 각각 자신에 해당하는 레이어들과 연결
        ConnectBookmarksIntoLayers();
        
        Time.timeScale = 0;
    }

    /// <summary>
    /// 모든 북마크들에게 레이어들을 매칭시키는 함수
    /// </summary>
    void ConnectBookmarksIntoLayers()
    {
        BookmarksAndLayers.Add(Buttons.Bookmark_CraftingLayer, GameObjects.CraftingLayer);
        BookmarksAndLayers.Add(Buttons.Bookmark_ReinforcingLayer, GameObjects.ReinforcingLayer);
        BookmarksAndLayers.Add(Buttons.Bookmark_QuestLayer, GameObjects.QuestLayer);
        BookmarksAndLayers.Add(Buttons.Bookmark_DiaryLayer, GameObjects.DiaryLayer);
        BookmarksAndLayers.Add(Buttons.Bookmark_MapLayer, GameObjects.MapLayer);
        BookmarksAndLayers.Add(Buttons.Bookmark_LastPage, GameObjects.LastPage);

        CloseAllLayerWithException(Get<GameObject>((int)GameObjects.CraftingLayer));
    }
    
    void CloseAllLayer()
    {
        foreach (var pair in BookmarksAndLayers)
            Get<GameObject>((int)pair.Value).SetActive(false);
    }
    
    public void CloseAllLayerWithException(GameObject exceptLayer)
    {
        foreach (var pair in BookmarksAndLayers)
            Get<GameObject>((int)pair.Value).SetActive(false);

        Get<Image>((int)Images.NoteBook_Background).gameObject
            .SetActive(exceptLayer != Get<GameObject>((int)GameObjects.LastPage));
        exceptLayer.SetActive(true);
    }
    
    public override void ClosePopupUI(PointerEventData action)
    {
        Time.timeScale = 1;
        base.ClosePopupUI(action);
    }
}

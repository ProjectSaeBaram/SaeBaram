using UnityEngine;

/// <summary>
/// 인벤토리 UI를 관리하는 샘플 클래스.
/// UI_Scene 클래스를 상속받아, 인벤토리에 특화된 UI 초기화와 아이템 표시 로직을 구현.
/// 위 인벤토리는 일반적인 팝업의 형태가 아닌, Scene에서 상시 켜져있는 형태의 인벤토리이기에 UI_Scene을 상속하는 형태.
/// </summary>
public class UI_Inven : UI_Scene
{
    // GameObjects 열거형은 인벤토리 UI 내에서 관리할 게임 오브젝트를 정의.
    enum GameObjects
    {
        GridPanel, // 아이템을 표시할 그리드 패널
    }

    /// <summary>
    /// UI 요소들을 초기화하는 메서드.
    /// </summary>
    public override void Init()
    {
        base.Init(); // 상위 클래스 UI_Scene의 Init 메서드 호출.

        Bind<GameObject>(typeof(GameObjects)); // 인벤토리 UI 요소들을 바인딩합니다.

        // GridPanel 게임 오브젝트를 가져옵니다.
        GameObject gridPanel = Get<GameObject>((int)GameObjects.GridPanel);

        // GridPanel의 모든 자식 오브젝트를 순회하여 삭제.
        // 인벤토리를 갱신하기 전에 기존 아이템 표시를 초기화하는 과정.
        foreach (Transform child in gridPanel.transform)
            Managers.Resource.Destroy(child.gameObject);

        // 인벤토리 아이템을 동적으로 생성하는 예시 코드.
        // 여기서는 임시로 8개의 아이템을 생성하고, 각 아이템에 이름을 설정.
        for (int i = 0; i < 8; i++)
        {
            // UI_Inven_Item 타입의 서브 아이템을 만들고, 그리드 패널에 자식으로 추가.
            GameObject item = Managers.UI.MakeSubItem<UI_Inven_Item>(parent : gridPanel.transform).gameObject;
            
            // 생성된 아이템 게임 오브젝트에 UI_Inven_Item 컴포넌트를 가져오거나 추가.
            UI_Inven_Item invenItem = item.GetOrAddComponent<UI_Inven_Item>();
            
            // 아이템 정보를 설정합니다. 예시로 "집행검 {i}번"과 같은 이름을 사용함.
            invenItem.SetInfo($"집행검 {i}번");
        }
    }
}
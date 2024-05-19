public class Define
{
    // Layer 열거형은 Unity의 레이어를 식별하기 위해 사용됩니다.
    // 각 레이어는 Unity 에디터에서 설정한 레이어와 일치하는 인덱스 값을 가집니다.
    public enum Layer
    {
        Monster = 8, // 몬스터 객체에 사용될 레이어
        Ground = 9,  // 지면 객체에 사용될 레이어
        Block = 10,  // 장애물 또는 블록 객체에 사용될 레이어
    }
    
    // Scene 열거형은 프로젝트 내의 씬들을 식별하기 위해 사용됩니다.
    // 새로운 씬이 추가될 때마다 해당 열거형에도 추가해야 합니다.
    public enum Scene
    {
        Unknown, // 정의되지 않은 기본 상태
        LobbyScene,   // 로비 씬
        GameScene,    // 게임 씬
    }

    // Sound 열거형은 사운드의 종류를 구분하기 위해 사용됩니다.
    // BGM과 효과음 등을 구분하여 관리할 수 있습니다.
    public enum Sound
    {
        Bgm,      // 배경음악
        Effect,   // 효과음
        MaxCount, // 사운드 종류의 총 수를 나타냅니다. 배열 크기 지정 등에 사용될 수 있습니다.
    }
    
    // UIEvent 열거형은 UI 이벤트의 종류를 구분하기 위해 사용됩니다.
    public enum UIEvent
    {
        Click, // 클릭 이벤트
        Drag,  // 드래그 이벤트
    }
    
    // CameraMode 열거형은 카메라의 모드를 식별하기 위해 사용됩니다.
    public enum CameraMode
    {
        QuarterView, // 4분면 뷰
    }

    // MouseEvent 열거형은 마우스 이벤트의 종류를 구분하기 위해 사용됩니다.
    public enum MouseEvent
    {
        Press,       // 마우스 버튼을 누르는 이벤트
        PointerDown, // 포인터가 대상 위로 내려가는 이벤트
        PointerUp,   // 포인터가 대상에서 올라가는 이벤트
        Click,       // 클릭 이벤트
    }
    
    // 게임에 등장하는 GameObject를 구분하기 위한 열거형.
    // 지금은 다음과 같이 플레이어와 몬스터만 구분한다.
    public enum WorldObject
    {
        Unknown,
        Player,
        Monster,
        DroppedItem,
    }
    
    // 지금 게임 세션이 새로운 게임인지, 저장된 데이터를 불러온 게임인지 구분하는 열거형.
    // GameScene에 진입하면서 이를 검사한다.
    public enum ThisGameis
    {
        NewGame,
        LoadedGame,
    }
    
    // 아이템의 종류를 구분하는 enum
    public enum ItemType
    {
        Tool,
        Ingredient,
        Dummy,
    }

    /// <summary>
    /// 아이템의 주 스텟 종류
    /// </summary>
    public enum ItemMainStatType
    {
        Lumbering,
        Dismantling,
        Mining,
        Attack,
    }
}
using UnityEngine;

/// <summary>
/// 모든 기능별 매니저들을 중앙에서 관리하는 싱글톤 구현체.
/// </summary>
[DefaultExecutionOrder(0)]
public class Managers : MonoBehaviour
{
    static Managers s_instance; // 유일한 인스턴스를 담을 변수.
    static Managers Instance { get { Init(); return s_instance; } } // 유일한 인스턴스를 참조하는 property

    // 기능별 매니저 인스턴스들
    // 새로운 Manager가 추가될 때, 아래에 하나씩 추가.
    private DataManager _data = new DataManager();
    private GameManagerEx _game = new GameManagerEx();
    //private InputManager _input = new InputManager();
    private PoolManager _pool = new PoolManager();
    private ResourceManger _resource = new ResourceManger();
    private SceneManagerEx _scene = new SceneManagerEx();
    private SoundManager _sound = new SoundManager();
    private UIManager _ui = new UIManager();
    private CraftingManager _crafting = new CraftingManager();
    private ReputeManager _repute = new ReputeManager();
    
    public static DataManager Data => Instance._data;
    public static GameManagerEx Game => Instance._game;
    //public static InputManager Input => Instance._input;
    public static PoolManager Pool => Instance._pool;
    public static ResourceManger Resource => Instance._resource;
    public static SceneManagerEx Scene => Instance._scene;
    public static SoundManager Sound => Instance._sound;
    public static UIManager UI => Instance._ui;
    public static CraftingManager Crafting => Instance._crafting;
    public static ReputeManager Repute => Instance._repute;
    
    void Start()
    {
        Init();
    }

    private void Update()
    {
        //_input.OnUpdate();
    }

    /// <summary>
    /// Managers 클래스와 모든 기능별 매니저 컴포넌트의 초기화를 담당.
    /// </summary>
    static void Init()
    {
        // s_instance가 null일 때만 Managers를 찾아 Instance에 할당
        if (s_instance != null) return;
        
        GameObject go = GameObject.Find("@Managers");
        if (go == null)
        {
            go = new GameObject {name = "@Managers"};
            go.AddComponent<Managers>();
        }
        GameObject go2 = GameObject.Find("DialogueManager");
        if (go2 == null)
        {
            go2 = new GameObject { name = "DialogueManager" };
            go2.AddComponent<DialogueManager>();
        }
        DontDestroyOnLoad(go);
        s_instance = go.GetComponent<Managers>();
        
        // 여기 안에서는 Instance를 호출하면 Infinite Loop에 걸리기 때문에, 호출 금지.
        
        s_instance._data.Init();
        s_instance._pool.Init();
        s_instance._sound.Init();
        s_instance._crafting.Init();

        s_instance._repute.Init();
    }

    /// <summary>
    /// Scene을 이동할 때 호출해야 하는 함수.
    /// </summary>
    public static void Clear()
    {   
        // DataManager는 Clear X
        Sound.Clear();
        //Input.Clear();
        UI.Clear();
        Scene.Clear();
        
        Pool.Clear();       // 의도적으로 마지막에 Clear. 왜? 다른 Manager에서 pool 오브젝트를 사용할 수 있기 때문.
    }

    private void OnApplicationQuit()
    {
        // 인벤토리 데이터를 바이너리 파일로 저장하도록
        Managers.Data.OnClose?.Invoke();        // Test할 때 발생하는 오류를 막기 위해 ? (Nullable) 추가.
        
        // 퀵슬롯 데이터를 바이너리 파일로 저장하도록
        Managers.Data.OnCloseQ?.Invoke();       // Test할 때 발생하는 오류를 막기 위해 ? (Nullable) 추가.
    }
}

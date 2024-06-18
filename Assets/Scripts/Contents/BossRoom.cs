using Cinemachine;
using Controllers.Mob.Boss;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using VInspector;

/// <summary>
/// Player가 BossRoom에 진입함을 감지하는 객체
/// </summary>
public class BossRoom : MonoBehaviour
{
    [Tab("Settings")]
    // ReSharper disable once InvalidXmlDocComment
    /// <summary>
    /// 곰 보스
    /// </summary>
    [SerializeField] private BearBossController BearController;
    [SerializeField] private DummyBearBossController DummyBearController;
    
    [Header("Camera Control")] 
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private CinemachineConfiner2D cinemachineConfiner2D;
    [SerializeField] private PolygonCollider2D originCameraBorder;
    [SerializeField] private PolygonCollider2D bossRoomCameraBorder;

    /// <summary>
    /// 플레이어의 이동을 제한할 물리적 경계
    /// </summary>
    [SerializeField] private BoxCollider2D bossRoomPhysicalBorder1;
    [SerializeField] private BoxCollider2D bossRoomPhysicalBorder2;

    [SerializeField] private CinemachineBlendController _cinemachineBlendController;
    
    /// <summary>
    /// 보스 격파 시, 아이템에 추가될 로그
    /// </summary>
    [Header("Gimmick")]
    [SerializeField] private string LogForSuppression;
    
    /// <summary>
    /// 보스의 돌진 패턴을 피하기 위한 AirGround
    /// </summary>
    [SerializeField] public GameObject AirGround;
    
    /// <summary>
    /// 보스가 Roar할 때, 필드에 소환할 Rock
    /// </summary>
    [SerializeField] public GameObject RockPrefab;

    /// <summary>
    /// Rock들이 소환될 위치들
    /// </summary>
    [SerializeField] public Transform[] RockSpawnPositions;

    /// <summary>
    /// 소환된 Rocks
    /// </summary>
    private List<GameObject> SpawnedRocks = new List<GameObject>();

    /// <summary>
    /// UI에 표시될 Boss 체력 바
    /// </summary>
    private UI_BossHPBar bossHpBar;
    
    [SerializeField] private CinemachineTargetGroup _cinemachineTargetGroup;
    
    /// <summary>
    /// Player가 BossRoom에 진입할 때 Invoke시킬 UnityEvent
    /// </summary>
    [Header("Events")]
    [SerializeField] private UnityEvent _onPlayerEntered;
    
    /// <summary>
    /// Boss가 죽을 때 Invoke시킬 UnityEvent
    /// </summary>
    [SerializeField] public UnityEvent OnBossDied = null;


    [Tab("SoundClips")] 
    [SerializeField] private List<AudioClip> despawnSounds;
    
    private void Awake()
    {
        // 플레이어가 보스룸에 진입할 때 실행시킬 함수들을 _onPlayerEntered에 추가
        _onPlayerEntered?.AddListener(ActivateDummyBoss);
        _onPlayerEntered?.AddListener(ReplaceCameraBoarderIntoBossRoomCameraBorder);
        _onPlayerEntered?.AddListener(ActivatePhysicalBorders);
        _onPlayerEntered?.AddListener(AddPlayerIntoCinemachineTargetGroup);
        _onPlayerEntered?.AddListener(DisableEnterEvents);
        // _onPlayerEntered?.AddListener();
        
        // 보스가 토벌될 때 실행시킬 함수들을 OnBossDied에 추가
        OnBossDied?.AddListener(ReplaceCameraBoarderIntoOriginCameraBorder);
        OnBossDied?.AddListener(DeactivatePhysicalBorders); 
        OnBossDied?.AddListener(AddLogToPlayersWeapon);
        OnBossDied?.AddListener(DisableEnterTrigger);
        OnBossDied?.AddListener(DisableBossHpBarUI);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 보스룸에 진입하면
        if (other.gameObject.CompareTag("Player"))
        {
            DebugEx.Log("Entered BossRoom!");
            _onPlayerEntered?.Invoke();
        }
    }
    
    #region AboutRoomEntrance

    void DisableEnterEvents()
    {
        _onPlayerEntered?.RemoveAllListeners();
    }
    
    void ActivateDummyBoss()
    {
        // 더미 보스 활성화
        DummyBearController.gameObject.SetActive(true);
        
        // 플레이어 컨트롤 비활성화
        PlayerController player = Managers.Game.GetPlayer().GetComponent<PlayerController>();
        player.Disableall();
        
        // Scene UI 비활성화
        Managers.UI.GetCurrentSceneUI().gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 더미 보스 비활성화,
    /// 진짜 보스 활성화,
    /// 플레이어 활성화
    /// </summary>
    public void EngageStart()
    {
        // 더미 보스 비활성화
        DummyBearController.gameObject.SetActive(false);
        
        // 진짜 보스 활성화
        ActivateBoss();
        
        // 플레이어 활성화
        Managers.Game.GetPlayer().GetComponent<PlayerController>().Enableall();
        
        // 카메라 조정
        _cinemachineBlendController.Exit();
        
        // 다이얼로그 UI 끄기
        Managers.UI.ClosePopupUI(Managers.ScriptDialogue.prevPopup);
        
        // Scene UI 활성화
        Managers.UI.GetCurrentSceneUI().gameObject.SetActive(true);
    }
    
    /// <summary>
    /// 진짜 보스 활성화
    /// </summary>
    public void ActivateBoss()
    {
        // 보스 활성화
        BearController.gameObject.SetActive(true);
        
        // 보스 체력바 활성화 
        UI_Scene sceneUI = Managers.UI.GetCurrentSceneUI();
        bossHpBar = Managers.UI.MakeSubItem<UI_BossHPBar>(sceneUI.transform);
        bossHpBar.SetBossController(BearController);
    }
    
    /// <summary>
    /// VirtualCamera의 경계를 보스방 경계로 대체
    /// </summary>
    void ReplaceCameraBoarderIntoBossRoomCameraBorder()
    {
        cinemachineConfiner2D.m_BoundingShape2D = bossRoomCameraBorder;
    }
    
    /// <summary>
    /// VirtualCamera의 경계를 맵 경계로 복귀
    /// </summary>
    [ContextMenu("SetCameraToOriginBorder")]
    void ReplaceCameraBoarderIntoOriginCameraBorder()
    {
        cinemachineConfiner2D.m_BoundingShape2D = originCameraBorder;
    }

    /// <summary>
    /// 플레이어의 이동을 막는 물리적 경계 활성화
    /// </summary>
    void ActivatePhysicalBorders()
    {
        bossRoomPhysicalBorder1.gameObject.SetActive(true);
        bossRoomPhysicalBorder2.gameObject.SetActive(true);
    }
    
    /// <summary>
    /// 플레이어의 이동을 막는 물리적 경계 비활성화
    /// </summary>
    void DeactivatePhysicalBorders()
    {
        bossRoomPhysicalBorder1.gameObject.SetActive(false);
        bossRoomPhysicalBorder2.gameObject.SetActive(false);
    }

    /// <summary>
    /// 보스방 입장을 트리거하는 충돌체 비활성화
    /// </summary>
    void DisableEnterTrigger()
    {
        GetComponent<BoxCollider2D>().enabled = false;
    }

    void DisableBossHpBarUI()
    {
        Destroy(bossHpBar.gameObject);
    }
    
    #endregion
    
    #region Boss Gimmick

    /// <summary>
    /// AirGrounds 소환
    /// </summary>
    public void SpawnAirGrounds()
    {
        AirGround.SetActive(true);
        
        foreach (Transform child in AirGround.transform)
        {
            child.gameObject.SetActive(true);
        }
    }
    
    /// <summary>
    /// AirGrounds 소환 해제
    /// </summary>
    public void DespawnAirGrounds()
    {
        foreach (Transform child in AirGround.transform)
        {
            child.gameObject.SetActive(false);
        }
        
        AirGround.SetActive(false);
        DespawnSounds();
    }
    
    /// <summary>
    /// 떨어지는 바위들 소환
    /// </summary>
    public void SpawnFallingRocks()
    {
        if (SpawnedRocks.Count != 0) DespawnFallingRocks();
        
        foreach (Transform position in RockSpawnPositions)
        {
            GameObject rock = Instantiate(RockPrefab, position.position, Quaternion.identity, null);
            SpawnedRocks.Add(rock);
        }
    }
    
    /// <summary>
    /// 떨어진 바위들 제거
    /// </summary>
    public void DespawnFallingRocks()
    {
        foreach (GameObject rock in SpawnedRocks)
        {
            Destroy(rock);
        }
        SpawnedRocks.Clear();

        DespawnSounds();
    }
    
    #endregion
    
    /// <summary>
    /// 플레이어의 무기에 로그를 추가하는 기능
    /// </summary>
    private void AddLogToPlayersWeapon()
    {
        PlayerController player = Managers.Game.GetPlayer().GetComponent<PlayerController>();
        player._handledItem.GetOriginItemUI()?.AddLog(LogForSuppression);
    }

    private void DespawnSounds()
    {
        int rand = Random.Range(0, despawnSounds.Count);
        Managers.Sound.Play(despawnSounds[rand]);
    }

    /// <summary>
    /// BossRoom에 진입할 때 플레이어와 곰을 모두 비추기 위한 함수
    /// </summary>
    private void AddPlayerIntoCinemachineTargetGroup()
    {
        CinemachineTargetGroup.Target playerTarget = new CinemachineTargetGroup.Target { target = Managers.Game.GetPlayer().transform, weight = 1};
        _cinemachineTargetGroup.m_Targets[1] = playerTarget;

        cinemachineVirtualCamera.Follow = _cinemachineTargetGroup.transform;
        cinemachineVirtualCamera.LookAt = _cinemachineTargetGroup.transform;

        StartCoroutine(CutSceneEnd());
    }

    private float delay = 5.0f;
    
    /// <summary>
    /// 2초 딜레이 후에, 카메라를 원상복구 시키기 위한 함수 
    /// </summary>
    /// <returns></returns>
    private IEnumerator CutSceneEnd()
    {
        yield return new WaitForSecondsRealtime(delay);
        
        // 카메라 원상복구
        cinemachineVirtualCamera.Follow = Managers.Game.GetPlayer().transform;
        cinemachineVirtualCamera.LookAt = null;
    }
}

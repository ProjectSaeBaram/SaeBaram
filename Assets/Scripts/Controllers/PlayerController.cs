using Controllers.Entity;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using VInspector;
using Vector2 = UnityEngine.Vector2;

[RequireComponent(typeof(Rigidbody2D))][RequireComponent(typeof(CapsuleCollider2D))][RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    // Player의 컨트롤은 State에 기반해 이를 변경하는 방식으로 구현.
    // 현재 Player의 State는 Idle, Walk, Run, Jump, Fall, Attack 총 6개의 State로 구분.
    // 새로운 State를 추가할 때는 IPlayerState 인터페이스를 상속받는 클래스를 정의하고,
    // PlayerController가 해당 클래스의 인스턴스를 들고있도록 수정하면 바로 적용할 수 있다.

    #region about state
    
    public interface IPlayerState
    {
        void Enter(PlayerController player);
        void Execute();
        void Exit();
    }

    private class PlayerIdleState : IPlayerState
    {
        private PlayerController _playerController;
        public void Enter(PlayerController player)
        {
            _playerController = player;
            
            _playerController.animator.CrossFade("PlayerIdle", 0.1f);
        }

        public void Execute()
        {
            if (_playerController.inputVector.x == 0) return;
            _playerController.ChangeState(_playerController.shiftToggled ? _playerController._runState : _playerController._walkState);
        }

        public void Exit()
        {

        }
        
        ~PlayerIdleState() { DebugEx.LogWarning($"Finalize {this}");}
        
        public override string ToString()
        {
            return "<color='blue'><b>IdleState</b></color>";
        }
    }

    private class PlayerWalkState : IPlayerState
    {
        private PlayerController _playerController;
        public void Enter(PlayerController player)
        {
            _playerController = player;
            _playerController.animator.CrossFade("PlayerWalk", 0.1f);
        }

        public void Execute()
        {
            // 플레이어의 입력에 따라 캐릭터 이동
            _playerController._rigidbody2D.velocity =
                new Vector2(_playerController.inputVector.x * _playerController.speed,
                    _playerController._rigidbody2D.velocity.y);

            _playerController.FlipImage();
        }

        public void Exit()
        {

        }
        
        ~PlayerWalkState() { DebugEx.LogWarning($"Finalize {this}");}
        
        public override string ToString()
        {
            return "<color='yellow'><b>WalkState</b></color>";
        }
    }

    private class PlayerRunState : IPlayerState
    {
        private PlayerController _playerController;
        public void Enter(PlayerController player)
        {
            _playerController = player;
            
            _playerController.animator.CrossFade("PlayerRun", 0.1f);
        }

        public void Execute()
        {
            // 플레이어의 입력에 따라 캐릭터 이동
            _playerController._rigidbody2D.velocity = 
                new Vector2(_playerController.inputVector.x * _playerController.runningSpeed,
                    _playerController._rigidbody2D.velocity.y);

            _playerController.FlipImage();
        }

        public void Exit()
        {

        }
        
        ~PlayerRunState() { DebugEx.LogWarning($"Finalize {this}");}
        
        public override string ToString()
        {
            return "<color='Cyan'><b>RunState</b></color>";
        }
    }

    private class PlayerJumpState : IPlayerState
    {
        private PlayerController _playerController;
        public void Enter(PlayerController player)
        {
            _playerController = player;

            _playerController.animator.CrossFade("PlayerJump", 0.1f);
            
            if (_playerController.jumpForced) return;
            _playerController._rigidbody2D.velocity = new Vector2(_playerController._rigidbody2D.velocity.x, 0);
            _playerController._rigidbody2D.AddForce(Vector2.up * _playerController.jumpPower, ForceMode2D.Impulse);

            _playerController.jumpForced = true;
        }

        public void Execute()
        {
            float speed = (_playerController.shiftToggled) ? _playerController.runningSpeed : _playerController.speed;

            // 플레이어의 입력에 따라 캐릭터 이동
            _playerController._rigidbody2D.velocity =
                new Vector2(_playerController.inputVector.x * speed,
                    _playerController._rigidbody2D.velocity.y);

            _playerController.FlipImage();
        }

        public void Exit()
        {

        }
        ~PlayerJumpState() { DebugEx.LogWarning($"Finalize {this}");}
        
        public override string ToString()
        {
            return "<color='blue'><b>IdleState</b></color>";
        }
    }

    private class PlayerFallState : IPlayerState
    {
        private PlayerController _playerController;
        public void Enter(PlayerController player)
        {
            _playerController = player;
            
            _playerController.animator.CrossFade("PlayerFall", 0.1f);
        }

        public void Execute()
        {

        }

        public void Exit()
        {

        }
        ~PlayerFallState() { DebugEx.LogWarning($"Finalize {this}");}
        
        public override string ToString()
        {
            return "<color='brown'><b>FallState</b></color>";
        }
    }

    private class PlayerAttackState : IPlayerState
    {
        private PlayerController _playerController;
        public void Enter(PlayerController player)
        {
            _playerController = player;
            
            _playerController.animator.CrossFade("PlayerAttack", 0.1f);
        }

        public void Execute()
        {

        }

        public void Exit()
        {
            _playerController._handledItem?.ColliderDeactivate();
        }
        ~PlayerAttackState() { DebugEx.LogWarning($"Finalize {this}");}

        public override string ToString()
        {
            return "<color='red'><b>AttackState</b></color>";
        }
    }

    #endregion
    
    [Tab("Information")]
    [SerializeField] private Vector2 inputVector;
    [SerializeField] private float speed = 400f;
    [SerializeField] private float runningSpeed = 700f;
    [SerializeField] private float jumpPower = 1800f;
    [SerializeField] private bool isJumpable = false;
    [SerializeField] private bool jumpForced = false;
    [SerializeField] private bool shiftToggled = false;
    private IPlayerState _currentState;
    
    [SerializeField] private bool isInvulnerable = false;           // 무적 여부
    [SerializeField] private Sprite[] playerPortrait;
    
    // 체력 필드
    private int maxHp = 100;
    [SerializeField] private int currentHp = 100;
    
    [Tab("Quest & Dialogue")]
    [SerializeField] private QuestManager _questManager;
    [SerializeField] public int questIdx=0;                 //TODO: 새로 시작할 때는 0 아닐때는 저장된 questIdx 값 가져오기 
    [SerializeField] public bool isQuestPanelActive;
    [SerializeField] public ReputeState RState;
    [SerializeField] public UnityAction OnInteract;
    private bool interactPressed;
    private static PlayerController instance;
    public static PlayerController GetInstance() { return instance; }

    public int GetquestIdex()
    {
        return questIdx;
    }
    public void IncreaseQuestIdx()
    {
        questIdx++;
    }
    public Sprite getplayerPortrait(int num)
    {
        return playerPortrait[num];
    }
    
    private PlayerIdleState _idleState;
    private PlayerWalkState _walkState;
    private PlayerRunState _runState;
    private PlayerJumpState _jumpState;
    private PlayerFallState _fallState;
    private PlayerAttackState _attackState;

    [Tab("KeyComponents")]
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] public Animator animator;
    [SerializeField] private Transform _rightHandBone;
    [SerializeField] public Handled_Item _handledItem;
    
    // 피격 이펙트 시각화를 위한 필드들
    private List<SpriteRenderer> spriteRenderers;            // 스프라이트 렌더러 리스트
    private MaterialPropertyBlock propertyBlock;            // 머티리얼 프로퍼티 블록

    [Tab("SoundClips")] 
    // 걸을 때 재생돼는 발소리
    [SerializeField] private List<AudioClip> walkFootSteps;
    // 달릴 때 재생되는 발소리
    [SerializeField] private List<AudioClip> runFootSteps;
    // 손을 휘두를 때 재생되는 효과음
    [SerializeField] private List<AudioClip> swings;
    
    /// <summary>
    /// 발로 디디고 설 수 있는 Ground Layer
    /// </summary>
    LayerMask GroundLayer;
    
    /// <summary>
    /// 상호작용할 수 있는 Entity Layer
    /// </summary>
    int EntityLayer;
    
    private UI_Game_QuickSlotGroup _quickSlotGroup;
    
    private PlayerInputActions _playerInputActions;
    private Vector3 _localScale;
    
    // 게임이 초기화 되는 시점에 퀵슬롯 아이템을 손에 들리기 위한 필드
    private TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();

    [SerializeField] public UI_Game_QuickSlotGroup QuickSlotGroup
    {
        get => _quickSlotGroup;
        set
        {
            _quickSlotGroup = value;
            if (_quickSlotGroup != null)
            {
                _tcs.TrySetResult(true);
            }
        }
    }
    
    // 마우스 커서가 UI 요소 위에 있는지 여부
    private bool _isPointerOverUI;
    
    private void Awake()
    {
        // 중요한 컴포넌트들을 모두 초기화
        InitKeyComponents();
        // State 초기화
        InitStates();
        
        // 발로 디디고 설 수 있는 Ground Layer 초기화
        GroundLayer = LayerMask.GetMask("Ground") | LayerMask.GetMask("AirGround");
        
        // 상호작용할 수 있는 Entity Layer 초기화
        EntityLayer = LayerMask.GetMask("Entity");
        
        DialogueManager.GetInstance().playerController = this;
        _localScale = transform.localScale;
        interactPressed = false;
        instance = this;
        RState = Managers.Repute.GetRepute();
    }
    
    private async void Start()
    {
        // 첫 번째 작업을 시작
        var waitForQuickSlotGroupTask = WaitForQuickSlotGroup();

        // 두 번째 작업을 시작
        var findSpriteRenderersTask = FindSpriteRenderers();
    
        // 두 작업이 완료될 때까지 대기
        await waitForQuickSlotGroupTask;
        await findSpriteRenderersTask;

        instance = this;
    }
    
    /// <summary>
    /// 퀵슬롯 준비 대기
    /// </summary>
    async Task WaitForQuickSlotGroup()
    {
        await _tcs.Task;
        await _quickSlotGroup.WaitForAllItemsToBeInitialized();
        DebugEx.Log("QuickSlotGroup has been set.");
        ChangeItemsInHand(1);
    }

    /// <summary>
    /// 하위 스프라이트 렌더러 찾기
    /// </summary>
    /// <returns></returns>
    Task FindSpriteRenderers()
    {
        // 모든 자식 객체의 SpriteRenderer를 찾아서 리스트에 추가
        spriteRenderers = new List<SpriteRenderer>();
        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
        {
            spriteRenderers.Add(sr);
        }

        return Task.CompletedTask;
    }



    /// <summary>
    /// 플레이어 컨트롤러에서 중요한 역할을 하는 컴포넌트를 초기화하는 함수.
    /// 현재 부착되어있는 컴포넌트들을 가져와 캐싱한다.
    /// </summary>
    private void InitKeyComponents()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.freezeRotation = true;

        animator = GetComponent<Animator>();

        _playerInputActions = new PlayerInputActions();
    }

    /// <summary>
    /// 플레이어의 State 객체들을 초기화하는 함수.
    /// 이 객체들은 영구적으로 쓰인다.
    /// </summary>
    private void InitStates()
    {
        _idleState = new PlayerIdleState();
        _walkState = new PlayerWalkState();
        _runState = new PlayerRunState();
        _jumpState = new PlayerJumpState();
        _fallState = new PlayerFallState();
        _attackState = new PlayerAttackState();

        ChangeState(_idleState);
    }

    /// <summary>
    /// 메모리를 위해 State 객체들을 삭제하는 함수
    /// </summary>
    void DestructStatesObjects()
    {
        // State객체들 초기화
        _idleState = null;
        _walkState = null;
        _runState = null;
        _jumpState = null;
        _fallState = null;
        _attackState = null;
    }
    
    /// <summary>
    /// 손에 쥔 물건을 바꾸는 함수
    /// </summary>
    /// <param name="context"></param>
    private void OnUpperNumberKeyPressed(InputAction.CallbackContext context)
    {
        ChangeItemsInHand(int.Parse(context.control.name));
    }

    public void ChangeItemsInHand(int keyNumber)
    {
        int targetIndex = keyNumber - 1;

        foreach (Transform child in _rightHandBone.transform)
        {
            child.gameObject.SetActive(false);
        }
        
        UI_Inven_Item handledUIItem = _quickSlotGroup.ChangeItemInHand(targetIndex);

        _handledItem = _rightHandBone.GetChild(targetIndex).gameObject.GetComponent<Handled_Item>();
        
        _handledItem.ItemUIReferenceSetter(handledUIItem);
        _handledItem.gameObject.SetActive(true);
        
        DebugEx.LogWarning("Change Items In Hand!");
    }
    
    private void FixedUpdate()
    {
        _currentState?.Execute();
    }

    private void Update()
    {
        RayCheckGround();
    }
    
    void LateUpdate()
    {
        _isPointerOverUI = EventSystem.current.IsPointerOverGameObject();
    }

    public IPlayerState GetCurrentState() { return _currentState;}

    private void ChangeState(IPlayerState newState)
    {
        //DebugEx.LogWarning($"State Changed from {_currentState} to {newState}");
        
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter(this);
    }

    public Rigidbody2D GetRigidBody2D() { return _rigidbody2D;}
    
    private void RayCheckGround()
    {
        Vector2 rayStart = _rigidbody2D.position + Vector2.down * 10 + Vector2.left * 20;
        float rayDistance = 40;

        Debug.DrawRay(rayStart, Vector3.right * rayDistance, Color.red);
        
        RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector3.right, rayDistance, GroundLayer);
        if (Equals(hit.collider, null))     // ray에 땅이 닿지 않을때 (= 공중일 때)
        {
            isJumpable = false;
            if (_rigidbody2D.velocity.y < 0 && _currentState is not PlayerFallState)
                ChangeState(_fallState);
        }
        else                                // ray에 땅이 닿았을 때
        {
            jumpForced = false;
            isJumpable = true;
            if (_currentState is PlayerFallState)
            {
                // 착지 시 방향키 입력이 없으면 Idle 상태로 전환
                if (inputVector.x == 0)
                {
                    ChangeState(_idleState);
                }
                else
                {
                    ChangeState(shiftToggled ? _runState : _walkState);
                }
            }
        }
    }

    private void FlipImage()
    {
        Vector3 directedToRight = new Vector3(_localScale.x * -1, _localScale.y, _localScale.z);
        Vector3 directedToLeft = new Vector3(_localScale.x, _localScale.y, _localScale.z);

        if (inputVector.x == 0) { }
        else if (inputVector.x < 0) transform.localScale = directedToLeft;
        else transform.localScale = directedToRight;
    }
    
    private void FlipImageToCursor(Vector2 cursorPos)
    {
        Vector3 cursorWorldPosition = Camera.main.ScreenToWorldPoint(cursorPos);
        
        Vector3 directedToRight = new Vector3(_localScale.x * -1, _localScale.y, _localScale.z);
        Vector3 directedToLeft = new Vector3(_localScale.x, _localScale.y, _localScale.z);
        
        if (cursorWorldPosition.x < transform.position.x) transform.localScale = directedToLeft;
        else transform.localScale = directedToRight;
    }

    #region Move

    void MoveStarted(InputAction.CallbackContext context)
    {
        // DebugEx.Log($"MoveStarted");

        if (_currentState is PlayerFallState or PlayerJumpState) return;
        
        ChangeState(shiftToggled ? _runState : _walkState);
    }
    void MovePerformed(InputAction.CallbackContext context)
    {
        // DebugEx.Log($"MovePerformed");

        inputVector.x = context.ReadValue<Vector2>().x;
    }
    void MoveCanCeled(InputAction.CallbackContext context)
    {
        // DebugEx.Log($"MoveCanceled");

        if (_currentState is not PlayerFallState && _currentState is not PlayerJumpState)
            ChangeState(_idleState);
        
        inputVector.x = 0;
    }

    void RunningSwitchEnter(InputAction.CallbackContext context)
    {
        shiftToggled = true;

        if (_currentState is PlayerWalkState && shiftToggled)
            ChangeState(_runState);
    }

    void RunningSwitchExit(InputAction.CallbackContext context)
    {
        shiftToggled = false;

        if (_currentState is PlayerRunState && !shiftToggled)
            ChangeState(_walkState);
    }
    
    /// <summary>
    /// 걸을 때 발소리를 내는 메서드
    /// </summary>
    public void PlayWalkFootStepSound()
    {
        int rand = Random.Range(0, walkFootSteps.Count);
        Managers.Sound.Play(walkFootSteps[rand], Define.Sound.Effect, 0.8f);
    }
    
    /// <summary>
    /// 달릴 때 발소리를 내는 메서드
    /// </summary>
    public void PlayRunFootStepSound()
    {
        int rand = Random.Range(0, runFootSteps.Count);
        Managers.Sound.Play(runFootSteps[rand], Define.Sound.Effect, 0.2f);
    }
    
    #endregion

    #region Jump

    void JumpStarted(InputAction.CallbackContext context)
    {
        // DebugEx.Log($"JumpStarted");
        if (!isJumpable) return;

        ChangeState(_jumpState);
    }
    void JumpPerformed(InputAction.CallbackContext context)
    {
        // DebugEx.Log($"JumpPerformed");
    }
    void JumpCanceled(InputAction.CallbackContext context)
    {
        // DebugEx.Log($"JumpCanceled");
    }

    #endregion

    #region Interact
    
    void InteractStarted(InputAction.CallbackContext context)
    {
        DebugEx.Log($"InteractStarted");
        
        // 상호작용 가능한 대상을 찾기
        Collider2D target = Physics2D.OverlapBox(transform.position + Vector3.up * 30, Vector2.one * 60, 0, EntityLayer);
        
        if (target != null)
        {
            // 대상 GameObject에서 IInteractable을 상속받는 컴포넌트들을 모두 가져옴
            IInteractable[] interactables = target.GetComponents<IInteractable>();
            
            foreach (IInteractable interactable in interactables)
            {
                interactable.Interact();
            }
        }
        
        // 박스의 모서리 계산
        Vector2 halfBoxSize = Vector2.one * 60 / 2;
        Vector2 topLeft = (Vector2)(transform.position + Vector3.up * 30) + new Vector2(-halfBoxSize.x, halfBoxSize.y);
        Vector2 topRight = (Vector2)(transform.position + Vector3.up * 30) + new Vector2(halfBoxSize.x, halfBoxSize.y);
        Vector2 bottomLeft = (Vector2)(transform.position + Vector3.up * 30) + new Vector2(-halfBoxSize.x, -halfBoxSize.y);
        Vector2 bottomRight = (Vector2)(transform.position + Vector3.up * 30) + new Vector2(halfBoxSize.x, -halfBoxSize.y);

        // 모서리를 연결하여 박스 그리기
        Debug.DrawRay(topLeft, topRight - topLeft, Color.red, 1.0f);
        Debug.DrawRay(topRight, bottomRight - topRight, Color.red, 1.0f);
        Debug.DrawRay(bottomRight, bottomLeft - bottomRight, Color.red, 1.0f);
        Debug.DrawRay(bottomLeft, topLeft - bottomLeft, Color.red, 1.0f);
    }
    void InteractPerformed(InputAction.CallbackContext context)
    {
        interactPressed = true;
    }
    void InteractCanceled(InputAction.CallbackContext context)
    {
        // DebugEx.Log($"InteractCanceled");
    }

    public bool GetInteractPressed()
    {
        bool result = interactPressed;
        interactPressed = false;
        return result;
    }
    
    void CheckDroppedItem()
    {
        DebugEx.LogWarning("CheckDroppedItem");
        Collider2D underMyFeet = Physics2D.OverlapBox(transform.position, Vector2.one * 60, 0, LayerMask.GetMask("DroppedItem"));
        
        if (underMyFeet != null && underMyFeet.GetComponent<DroppedItem>() is not null)
        {
            underMyFeet.GetComponent<DroppedItem>().Fed();
        }
    }

    /// <summary>
    /// 피해를 받을 때
    /// </summary>
    /// <param name="damage"></param>
    public void GetHit(int damage)
    {
        if (isInvulnerable) return;             // 무적 상태일 때는 데미지 무시
        
        currentHp -= damage;
        StartCoroutine(FlashRed());      // 피격 시 깜빡임 효과
        if (currentHp <= 0)
        {
            currentHp = 0;
            
            // TODO 게임 오버 팝업 등장
            DebugEx.LogWarning("Game Over!");
        }
    }
    
    /// <summary>
    /// 데미지를 받을 때 시각화
    /// </summary>
    /// <returns></returns>
    private IEnumerator FlashRed()
    {
        isInvulnerable = true; // 무적 상태 시작

        for (int i = 0; i < 3; i++) // 세 차례에 걸쳐 깜박임
        {
            // 모든 스프라이트 색상을 빨간색으로 변경
            foreach (var sr in spriteRenderers)
            {
                sr.color = Color.red;
            }
            yield return new WaitForSeconds(0.1f); // 0.1초 대기

            // 모든 스프라이트 색상을 원래 색상으로 복원
            foreach (var sr in spriteRenderers)
            {
                sr.color = Color.white;
            }
            yield return new WaitForSeconds(0.1f); // 0.1초 대기
        }

        isInvulnerable = false; // 무적 상태 종료
    }
    
    #endregion

    void PickupStarted(InputAction.CallbackContext context)
    {
        CheckDroppedItem();
    }
    
    #region Pause

    void PauseOrResume(InputAction.CallbackContext context)
    {
        // 1. 뭐든지 열려있으면 다 닫기
        // 2. 아무것도 없으면 열기

        if (Managers.UI.GetStackSize() > 0)
            Managers.UI.CloseAllPopupUI();
        else
            Managers.UI.ShowPopupUI<UI_PausePopup>();
    }

    #endregion
   
    #region AdvanceQuest
    void AdvanceQuest(InputAction.CallbackContext context)
    {
        Advance();
    }

    private void Advance()
    {
        QuestManager.GetInstance().AdvanceQuest(1);
    }
    #endregion
    
    #region Notebook

    void OpenOrCloseNotebook(InputAction.CallbackContext context)
    {
        // 노트북 팝업이 열려있으면 닫고, 
        // 열려있지 않으면 열기
        
        UI_NotebookPopup notebookPopup = FindObjectOfType<UI_NotebookPopup>();
        if(notebookPopup == null)
            Managers.UI.ShowPopupUI<UI_NotebookPopup>();
        else
        {
            Managers.UI.CloseAllPopupUI();
            //notebookPopup.ClosePopupUI(null);
        }
    }
    
    #endregion

    #region Click

    void OnClick(InputAction.CallbackContext context)
    {
        // UI위에 마우스가 올라가 있다면
        if (_isPointerOverUI) return;
        
        // 들고있는 아이템을 휘두르기
        FlipImageToCursor(Input.mousePosition);
        
        ChangeState(_attackState);
    }
    
    /// <summary>
    /// 손에 쥔 도구를 휘두르기 시작할 때 Call되는 함수
    /// </summary>
    void SwingStart()
    {
        if (_handledItem != null)
            _handledItem.ColliderActivate();
        
        // 휘두르는 소리
        int rand = Random.Range(0, swings.Count);
        Managers.Sound.Play(swings[rand], Define.Sound.Effect, 0.2f, 2f);
    }
    
    /// <summary>
    /// 손에 쥔 도구를 휘두르는게 끝날 때 Call되는 함수
    /// </summary>
    void SwingEnd()
    {
        if (_handledItem != null)
            _handledItem.ColliderDeactivate();
    }
    
    void EndAttackState()
    {
        // 공격이 끝났으니까, IdleState로 복귀
        ChangeState(_idleState);
    }
    
    #endregion
    
    #region About PlayerInput

    public void DisableClick()
    {
        _playerInputActions.PlayerAction.Click.Disable();
    }
    
    public void EnableClick()
    {
        _playerInputActions.PlayerAction.Click.Enable();
    }
    
    private void OnEnable()
    {
        // PlayerInput을 컴포넌트 대신 스크립트로
        _playerInputActions.PlayerAction.Move.started += MoveStarted;
        _playerInputActions.PlayerAction.Move.performed += MovePerformed;
        _playerInputActions.PlayerAction.Move.canceled += MoveCanCeled;
        _playerInputActions.PlayerAction.RunningSwitch.started += RunningSwitchEnter;
        _playerInputActions.PlayerAction.RunningSwitch.canceled += RunningSwitchExit;
        _playerInputActions.PlayerAction.Jump.started += JumpStarted;
        _playerInputActions.PlayerAction.Jump.performed += JumpPerformed;
        _playerInputActions.PlayerAction.Jump.canceled += JumpCanceled;
        _playerInputActions.PlayerAction.Interact.started += InteractStarted;
        _playerInputActions.PlayerAction.Interact.performed += InteractPerformed;
        _playerInputActions.PlayerAction.Interact.canceled += InteractCanceled;
        _playerInputActions.PlayerAction.WeaponChange.performed += OnUpperNumberKeyPressed;
        _playerInputActions.PlayerAction.Escape.started += PauseOrResume;
        _playerInputActions.PlayerAction.OpenNotebook.started += OpenOrCloseNotebook;
        _playerInputActions.PlayerAction.PickupItem.started += PickupStarted;
        _playerInputActions.PlayerAction.Click.started += OnClick;
        _playerInputActions.Enable();   
    }
    
    private void OnDisable()
    {
        // PlayerInput을 컴포넌트 대신 스크립트로
        _playerInputActions.PlayerAction.Move.started -= MoveStarted;
        _playerInputActions.PlayerAction.Move.performed -= MovePerformed;
        _playerInputActions.PlayerAction.Move.canceled -= MoveCanCeled;
        _playerInputActions.PlayerAction.RunningSwitch.started -= RunningSwitchEnter;
        _playerInputActions.PlayerAction.RunningSwitch.canceled -= RunningSwitchExit;
        _playerInputActions.PlayerAction.Jump.started -= JumpStarted;
        _playerInputActions.PlayerAction.Jump.performed -= JumpPerformed;
        _playerInputActions.PlayerAction.Jump.canceled -= JumpCanceled;
        _playerInputActions.PlayerAction.Interact.started -= InteractStarted;
        _playerInputActions.PlayerAction.Interact.performed -= InteractPerformed;
        _playerInputActions.PlayerAction.Interact.canceled -= InteractCanceled;
        _playerInputActions.PlayerAction.WeaponChange.performed -= OnUpperNumberKeyPressed;
        _playerInputActions.PlayerAction.Escape.started -= PauseOrResume;
        _playerInputActions.PlayerAction.OpenNotebook.started -= OpenOrCloseNotebook;
        _playerInputActions.PlayerAction.PickupItem.started -= PickupStarted;
        _playerInputActions.PlayerAction.Click.started -= OnClick;
        _playerInputActions.Disable();
                
        DestructStatesObjects();
    }

    public void DisablePickupItem()
    {
        _playerInputActions.PlayerAction.PickupItem.Disable();
    }

    public void EnablePickupItem()
    {
        _playerInputActions.PlayerAction.PickupItem.Enable();
    }
    public void DisableInteract()
    {
        _playerInputActions.PlayerAction.Interact.Disable();
    }

    public void EnableInteract()
    {
        _playerInputActions.PlayerAction.Interact.Enable();
    }

    public void DisableExceptInteract()
    {
        _playerInputActions.PlayerAction.Move.started -= MoveStarted;
        _playerInputActions.PlayerAction.Move.performed -= MovePerformed;
        _playerInputActions.PlayerAction.Move.canceled -= MoveCanCeled;
        _playerInputActions.PlayerAction.RunningSwitch.started -= RunningSwitchEnter;
        _playerInputActions.PlayerAction.RunningSwitch.canceled -= RunningSwitchExit;
        _playerInputActions.PlayerAction.Jump.started -= JumpStarted;
        _playerInputActions.PlayerAction.Jump.performed -= JumpPerformed;
        _playerInputActions.PlayerAction.Jump.canceled -= JumpCanceled;
        _playerInputActions.PlayerAction.WeaponChange.performed -= OnUpperNumberKeyPressed;
        _playerInputActions.PlayerAction.Escape.started -= PauseOrResume;
        _playerInputActions.PlayerAction.OpenNotebook.started -= OpenOrCloseNotebook;
        _playerInputActions.PlayerAction.PickupItem.started -= PickupStarted;
        _playerInputActions.PlayerAction.Click.performed -= OnClick;
    }


    public void Disableall()
    {
        _playerInputActions.Disable();
    }

    public void Enableall()
    {
        _playerInputActions.Enable();
    }


    public void EnableExceptInter()
    {
        _playerInputActions.PlayerAction.Move.started += MoveStarted;
        _playerInputActions.PlayerAction.Move.performed += MovePerformed;
        _playerInputActions.PlayerAction.Move.canceled += MoveCanCeled;
        _playerInputActions.PlayerAction.RunningSwitch.started += RunningSwitchEnter;
        _playerInputActions.PlayerAction.RunningSwitch.canceled += RunningSwitchExit;
        _playerInputActions.PlayerAction.Jump.started += JumpStarted;
        _playerInputActions.PlayerAction.Jump.performed += JumpPerformed;
        _playerInputActions.PlayerAction.Jump.canceled += JumpCanceled;
        _playerInputActions.PlayerAction.WeaponChange.performed += OnUpperNumberKeyPressed;
        _playerInputActions.PlayerAction.Escape.started += PauseOrResume;
        _playerInputActions.PlayerAction.OpenNotebook.started += OpenOrCloseNotebook;
        _playerInputActions.PlayerAction.PickupItem.started += PickupStarted;
        _playerInputActions.PlayerAction.Click.performed += OnClick;
    }


    #endregion
}

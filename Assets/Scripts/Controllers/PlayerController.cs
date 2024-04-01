using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VInspector;

[RequireComponent(typeof(Rigidbody2D))] [RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerController : MonoBehaviour
{
    [Tab("Information")]
    [SerializeField] private Vector2 inputVector;
    [SerializeField] private float speed = 400f;
    [SerializeField] private float runningSpeed = 700f;
    [SerializeField] private float jumpPower = 1800f;
    [SerializeField] private bool isJumpable = false;
    [SerializeField] private bool jumpForced = false;
    [SerializeField] private bool shiftToggled = false;
    [SerializeField] private State _currentState = State.Idle;
    [SerializeField] private State CurrentState
    {
        get => _currentState;
        set
        {
            bool ran = CurrentState == State.Run;
            _currentState = value;

            switch (_currentState)
            {
                case State.Idle:
                    _animator.CrossFade("PlayerIdle", idleCFCoef * (ran ? 3 : 1) );
                    break;
                case State.Walk:
                    _animator.CrossFade("PlayerWalk", walkCFCoef * (ran ? 3 : 1) );
                    break;
                case State.Run:
                    _animator.CrossFade("PlayerRun", runCFCoef);
                    break;
                case State.Jump:
                    _animator.CrossFade("PlayerJump", jumpCFCoef);
                    break;
                case State.Fall:
                    _animator.CrossFade("PlayerFall", fallCFCoef);
                    break;
                case State.Attack:
                    break;
            }
            
        }
    }

    private float idleCFCoef = 0.1f;
    private float walkCFCoef = 0.1f;
    private float runCFCoef = 0.3f;
    private float jumpCFCoef = 0.1f;
    private float fallCFCoef = 0.1f;

    [Tab("KeyComponents")]
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _rightHandBone;
    
    private PlayerInputActions _playerInputActions;
    private Vector3 _localScale;
    
    // [Tab("Interaction")]
    // [SerializeField] public GameObject NPC;
    // [SerializeField] private bool isNPCAvailable = false;

    enum State
    {
        Idle,
        Walk,
        Run,
        Jump,
        Fall,
        Attack,
    }
    
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.freezeRotation = true;
        
        _animator = GetComponent<Animator>();
        
        _localScale = transform.localScale;
        
        _playerInputActions = new PlayerInputActions();
    }

    /// <summary>
    /// 손에 쥔 물건을 바꾸는 함수
    /// </summary>
    /// <param name="context"></param>
    private void OnChange(InputAction.CallbackContext context)
    {
        DebugEx.Log($"Changed to {context.control.name}th tool!");

        int targetTool = int.Parse(context.control.name) - 1;
        
        for (int i = 0; i < _rightHandBone.childCount; i++)
            _rightHandBone.GetChild(i).gameObject.SetActive(false);

        _rightHandBone.GetChild(targetTool).gameObject.SetActive(true);
        _rightHandBone.GetChild(targetTool).gameObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
    }

    private void FixedUpdate()
    {
        // if (_rigidbody2D.velocity.y < -0.1f && CurrentState != State.Fall)
        //     CurrentState = State.Fall;
                
        switch (CurrentState)
        {
            case State.Idle:
                UpdateIdle();
                break;
            case State.Walk:
                UpdateWalk();
                break;
            case State.Run:
                UpdateRun();
                break;
            case State.Jump:
                UpdateJump();
                break;
            case State.Attack:
                UpdateAttack();
                break;
        }
        
        #region NPC Interaction

        // Debug.DrawRay(_rigidbody2D.position,Vector3.right,Color.red );
        // RaycastHit2D hit = Physics2D.Raycast(_rigidbody2D.position, Vector3.right, 1, LayerMask.GetMask("NPC"));
        // if(hit.collider != null)
        // {
        //     isNPCAvailable = true;
        //     DebugEx.Log("NPC Available : " + isNPCAvailable);
        //     NPC = GameObject.Find("NPC");     
        // }
        // if (isNPCAvailable && Vector2.Distance(transform.position, NPC.transform.position) > 3)
        // {
        //     isNPCAvailable = false;
        //     DebugEx.Log("NPC Available : " + isNPCAvailable);
        //     NPC = GameObject.Find("NULLNPC");
        // }

        #endregion
    }

    private void Update()
    {
        RayCheckGround();
    }

    private void RayCheckGround()
    {
        Vector2 rayStart = _rigidbody2D.position + Vector2.down * 10 + Vector2.left * 20;
        float rayDistance = 40;
        
        Debug.DrawRay(rayStart,Vector3.right * rayDistance,Color.red);
        RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector3.right, rayDistance, LayerMask.GetMask("Ground"));
        if (Equals(hit.collider, null))     // ray에 땅이 닿지 않을때 (= 공중일 때)
        {
            isJumpable = false;
            if (_rigidbody2D.velocity.y < -0.1f && CurrentState != State.Fall)
                CurrentState = State.Fall;
        }
        else                                // ray에 땅이 닿았을 때
        {
            jumpForced = false;
            isJumpable = true;
            if(CurrentState == State.Fall)
                CurrentState = State.Idle;
        }
    }
    
    private void UpdateIdle()
    {
        // do nothing...
        if (inputVector.x == 0) return;
        CurrentState = shiftToggled ? State.Run : State.Walk;
    }
    private void UpdateWalk()
    {
        _rigidbody2D.velocity = new Vector2(inputVector.x * speed, _rigidbody2D.velocity.y);
        
        FlipImage();
    }
    
    private void UpdateRun()
    {
        _rigidbody2D.velocity = new Vector2(inputVector.x * runningSpeed, _rigidbody2D.velocity.y);
        
        FlipImage();
    }
    
    private void UpdateJump()
    {
        if (jumpForced) return;

        _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0);
        _rigidbody2D.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

        jumpForced = true;
    }
    
    private void UpdateAttack()
    {
        throw new NotImplementedException();
    }
    
    private void FlipImage()
    {
        Vector3 directedToRight = new Vector3(_localScale.x * -1 , _localScale.y, _localScale.z);
        Vector3 directedToLeft = new Vector3(_localScale.x, _localScale.y, _localScale.z);

        if (inputVector.x == 0) transform.localScale = _localScale;
        else if (inputVector.x < 0) transform.localScale = directedToLeft;
        else transform.localScale = directedToRight;
    }

    #region Move

    void MoveStarted(InputAction.CallbackContext context)
    {
        DebugEx.Log($"MoveStarted {context}");
        CurrentState = shiftToggled ? State.Run : State.Walk;
    }
    void MovePerformed(InputAction.CallbackContext context)
    {
        DebugEx.Log($"MovePerformed {context}");
        
        inputVector.x = context.ReadValue<Vector2>().x;
    }
    void MoveCanCeled(InputAction.CallbackContext context)
    {
        DebugEx.Log($"MoveCanceled {context}");

        CurrentState = State.Idle;
        inputVector.x = 0;
    }

    void RunningSwitchEnter(InputAction.CallbackContext context)
    {
        shiftToggled = true;
        
        if (CurrentState == State.Walk && shiftToggled)
            CurrentState = State.Run;
    }
    
    void RunningSwitchExit(InputAction.CallbackContext context)
    {
        shiftToggled = false;
        
        if (CurrentState == State.Run && !shiftToggled)
            CurrentState = State.Walk;
    }
    
    #endregion

    #region Jump
    
    void JumpStarted(InputAction.CallbackContext context)
    {
        DebugEx.Log($"JumpStarted {context}");
        if (!isJumpable) return;

        CurrentState = State.Jump;
    }
    void JumpPerformed(InputAction.CallbackContext context)
    {
        DebugEx.Log($"JumpPerformed {context}");
    }
    void JumpCanceled(InputAction.CallbackContext context)
    {
        DebugEx.Log($"JumpCanceled {context}");
    }

    #endregion

    #region Interact
    void InteractStarted(InputAction.CallbackContext context)
    {
        DebugEx.Log($"InteractStarted {context}");
    }
    void InteractPerformed(InputAction.CallbackContext context)
    {
        DebugEx.Log($"InteractPerformed {context}");
    }
    void InteractCanceled(InputAction.CallbackContext context)
    {
        DebugEx.Log($"InteractCanceled {context}");
    }
    #endregion
    
    #region Pause

    void PauseOrResume(InputAction.CallbackContext context)
    {
        // if (Managers.UI.FindPopup<UI_PausePopup>() == null)
        // {
        //     Managers.UI.ShowPopupUI<UI_PausePopup>();
        // }
        // else
        // {
        //     Managers.UI.CloseAllPopupUI();
        //     Time.timeScale = 1.0f;
        // }
        
    }
    #endregion
    
    private void OnEnable()
    {
        #region About PlayerInput
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
        _playerInputActions.PlayerAction.WeaponChange.performed += OnChange;
        _playerInputActions.PlayerAction.Escape.started += PauseOrResume;
        _playerInputActions.Enable();
        #endregion
    }
    
    private void OnDisable()
    {
        #region About PlayerInput
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
        _playerInputActions.PlayerAction.WeaponChange.performed -= OnChange;
        _playerInputActions.PlayerAction.Escape.started -= PauseOrResume;
        _playerInputActions.Disable();
        #endregion
    }
}

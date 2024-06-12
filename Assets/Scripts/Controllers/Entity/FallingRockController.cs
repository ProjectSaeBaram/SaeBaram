using Controllers.Entity;
using System;
using UnityEngine;

public class FallingRockController : MonoBehaviour, IInteractable
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    
    [SerializeField] private float gravityScaleFactor = 1.0f;   // 중력 계수
    [SerializeField] private int damage = 10;                   // 플레이어에게 줄 데미지

    [SerializeField] private bool hasLanded = false;            // 바위가 지면에 착지했는지 여부
    [SerializeField] private bool playerHiding = false;         // 플레이어가 바위에 숨어 있는지 여부

    private Rigidbody2D _rigidbody2D;

    private int _regularOrder = 1;                  // 평소 SpriteRenderer의 SortingOrder
    private int _playerConcealingOrder = 200;       // 플레이어를 숨기고 있는 동안의 SortingOrder
    
    /// <summary>
    /// 충돌 시 추락을 멈출 Ground Layer
    /// </summary>
    private int GroundLayer;
    
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        
        // 초기 중력 설정
        _rigidbody2D.gravityScale = gravityScaleFactor; 
        
        // Ground Layer 설정
        GroundLayer = LayerMask.GetMask("Ground", "AirGround");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasLanded && ((1 << collision.gameObject.layer) & GroundLayer) != 0)
        {
            DebugEx.LogWarning("Landed!");
            
            // 바위가 지면에 착지하면 낙하를 멈춤
            hasLanded = true;
            _rigidbody2D.velocity = Vector2.zero;
            _rigidbody2D.gravityScale = 0;
            
            // 바위의 Rigidbody를 비활성화하여 더 이상 물리적 충돌이 발생하지 않도록 함
            _rigidbody2D.isKinematic = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !hasLanded)
        {
            // 바위가 플레이어와 충돌하면 데미지를 주고 밀쳐내기
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.GetHit(damage); // 데미지 주기
                // 밀쳐내기
                Rigidbody2D playerRigidbody = player.GetComponent<Rigidbody2D>();
                Vector2 pushDirection = (playerRigidbody.position - (Vector2)transform.position).normalized;
                playerRigidbody.AddForce(pushDirection * 5.0f, ForceMode2D.Impulse); 
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && playerHiding)
        {
            // 플레이어가 숨어있다가 움직여서 돌 밖으로 나가면       
            RevealPlayer();     // 렌더링 순서를 바꾸고
            playerHiding = false;
        }
    }

    public bool IsPlayerHiding()
    {
        return playerHiding;
    }

    public bool HasLanded()
    {
        return hasLanded;
    }

    /// <summary>
    /// 플레이어와의 상호작용
    /// F 키를 누를 때 호출되는 함수
    /// </summary>
    public void Interact()
    {
        playerHiding = !playerHiding;
        
        // 숨어있지 않으면 숨겨주고
        // 숨어있으면 내보내고 
        if(playerHiding) ConcealPlayer();
        else RevealPlayer();
    }

    /// <summary>
    /// 플레이어를 숨기는 함수
    /// </summary>
    private void ConcealPlayer()
    {
        // 플레이어를 가리도록
        _spriteRenderer.sortingOrder = _playerConcealingOrder;
    }

    /// <summary>
    /// 반대로 플레이어를 드러내는 함수 
    /// </summary>
    private void RevealPlayer()
    {
        // 플레이어를 다시 드러내도록
        _spriteRenderer.sortingOrder = _regularOrder;
    }
}

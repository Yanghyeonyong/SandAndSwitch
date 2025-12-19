using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // FSM
    public enum State { Idle, Move, Jump, Fall }
    State state = State.Idle;
    // Move
    [Header("Move")]
    public float maxSpeed = 8f;
    public float acceleration = 60f;
    public float deceleration = 80f;
    //251216 - 양현용 : 넉백 당하는 힘
    public float knockBackForce;

    // Jump (Variable Jump)
    [Header("Jump")]
    public float jumpImpulse = 8f;
    public float holdForce = 25f;
    public float maxHoldTime = 0.18f;
    public float jumpCutMultiplier = 0.35f;

    [Header("Jump Assist")]
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.12f;

    // Ground Check (틈 방지)
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.12f;
    public LayerMask groundLayer;

    [Header("Ground Confirm")]
    public int groundedConfirmFrames = 2;
    int groundedFrames = 0;

    // Input
    [Header("Input Actions")]
    public InputActionAsset actions;

    // Components
    [Header("Components")]
    public Animator animator;

    Rigidbody2D rb;
    InputAction moveAction;
    InputAction jumpAction;

    // Runtime values
    float moveX;
    bool isGrounded;

    float coyoteTimer;
    float jumpBufferTimer;

    bool jumpHeld;
    float holdTimer;

    [SerializeField] float playerScale = 5f;


    //251216 - 양현용 : 기믹과의 상호작용
    private bool _checkGimmick = false;
    public bool CheckGimmick
    {
        get
        {
            return _checkGimmick;
        }
        set
        {
            _checkGimmick = value;
        }
    }
    private Gimmick _curGimmick;
    public Gimmick CurGimmick
    {
        get
        { return _curGimmick; }
        set
        {
            _curGimmick = value;
        }
    }

    [SerializeField] private int onPortal = 0;
    private QuickSlotController slot;
    public QuickSlotController Slot =>slot;

    Vector3 curVelocity;

    // Unity 생명주기
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        slot = GetComponent<QuickSlotController>();        
        rb.freezeRotation = true;

        if (animator == null)
            animator = GetComponent<Animator>();

        var map = actions.FindActionMap("Player", true);
        moveAction = map.FindAction("Move", true);
        jumpAction = map.FindAction("Jump", true);

        //251216 - 양현용 : 게임매니저에 플레이어 전달
        GameManager.Instance.Player = this;
    }

    void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();

        jumpAction.started += OnJumpStarted;
        jumpAction.canceled += OnJumpCanceled;
    }

    void OnDisable()
    {
        jumpAction.started -= OnJumpStarted;
        jumpAction.canceled -= OnJumpCanceled;

        moveAction.Disable();
        jumpAction.Disable();



    }

    void Update()
    {
        // Input
        moveX = moveAction.ReadValue<Vector2>().x;

        // Ground Check
        bool rawGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // 2️⃣ 상승 중엔 grounded 무시
        bool groundedCandidate = rawGrounded && rb.linearVelocity.y <= 0.01f;

        // 3️⃣ 연속 프레임 확인
        if (groundedCandidate) groundedFrames++;
        else groundedFrames = 0;

        isGrounded = groundedFrames >= groundedConfirmFrames;

        // Timers
        if (isGrounded) coyoteTimer = coyoteTime;
        else coyoteTimer -= Time.deltaTime;

        if (jumpBufferTimer > 0f)
            jumpBufferTimer -= Time.deltaTime;

        // FSM Transitions
        switch (state)
        {
            case State.Idle:
                if (!isGrounded) ChangeState(State.Fall);
                else if (TryConsumeJump()) ChangeState(State.Jump);
                else if (Mathf.Abs(moveX) > 0.01f) ChangeState(State.Move);
                break;

            case State.Move:
                if (!isGrounded) ChangeState(State.Fall);
                else if (TryConsumeJump()) ChangeState(State.Jump);
                else if (Mathf.Abs(moveX) <= 0.01f) ChangeState(State.Idle);
                break;

            case State.Jump:
                if (rb.linearVelocity.y < 0f)
                    ChangeState(State.Fall);
                break;

            case State.Fall:
                if (isGrounded)
                    ChangeState(Mathf.Abs(moveX) > 0.01f ? State.Move : State.Idle);
                break;
        }

        // Animator
        animator.SetFloat("Speed", Mathf.Abs(moveX));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("YVel", rb.linearVelocity.y);
    }

    void FixedUpdate()
    {
        // Horizontal Move
        float targetVx = moveX * maxSpeed;
        float accel = Mathf.Abs(targetVx) > 0.01f ? acceleration : deceleration;

        float newVx = Mathf.MoveTowards(
            rb.linearVelocity.x,
            targetVx,
            accel * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector2(newVx, rb.linearVelocity.y);

        // Direction
        if (Mathf.Abs(moveX) > 0.01f)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveX) * playerScale, playerScale, playerScale);
        }

        // Variable Jump Hold
        if (state == State.Jump && jumpHeld && holdTimer > 0f && rb.linearVelocity.y > 0f)
        {
            rb.AddForce(Vector2.up * holdForce * Time.fixedDeltaTime, ForceMode2D.Force);
            holdTimer -= Time.fixedDeltaTime;
        }
    }

    // Jump Handling
    void OnJumpStarted(InputAction.CallbackContext ctx)
    {
        jumpBufferTimer = jumpBufferTime;
        jumpHeld = true;
    }

    void OnJumpCanceled(InputAction.CallbackContext ctx)
    {
        jumpHeld = false;

        if (rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y * jumpCutMultiplier
            );
        }
    }

    bool TryConsumeJump()
    {
        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
            DoJump();
            return true;
        }
        return false;
    }

    void DoJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);

        holdTimer = maxHoldTime;
        animator.SetTrigger("Jump");
    }

    public void TakeDamage()
    {
        Debug.Log("데미지를 입었다");
        //curVelocity = rb.linearVelocity.normalized;
        animator.SetTrigger("Damage");
        GameManager.Instance.PlayerTakeDamage(1);
        //rb.AddForce(-curVelocity * knockBackForce, ForceMode2D.Impulse);
    }

    // FSM Helper
    void ChangeState(State next)
    {
        if (state == next) return;
        state = next;
    }

    // Debug
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    //251216 - 양현용 : 기믹과의 상호작용
    public void OnIneract(InputAction.CallbackContext ctx)
    {
        if (ctx.started && onPortal == 1)
        {
            GameManager.Instance.LoadNextScene();
        }
        if (ctx.started && onPortal == 2)
        {
            GameManager.Instance.LoadPrevScene();
        }

        if (ctx.started && _checkGimmick && !GameManager.Instance.OnProgressGimmick)
        {
            _curGimmick.StartGimmick();
        }
        else if (ctx.started && _checkGimmick && GameManager.Instance.OnProgressGimmick)
        {
            _curGimmick.ExitGimmick();
        }
        //아이템 관련 추가
        if (ctx.started && _nearbyItem != null)
        {
            //QuickSlotController slot = GetComponent<QuickSlotController>();
            if (slot != null && slot.TryPickup(_nearbyItem.ItemData))
            {
                _nearbyItem.Pickup();
                _nearbyItem = null;
                return;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Finish"))
        {
            onPortal = 1;
        }
        else if (collision.CompareTag("Respawn"))
        {
            onPortal = 2;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Finish") || collision.CompareTag("Respawn"))
        {
            onPortal = 0;
        }
    }
    //아이템 관련 추가
    private ItemPickup _nearbyItem;
    public void SetNearbyItem(ItemPickup item)
    {
        _nearbyItem = item;
    }

    public void ClearNearbyItem(ItemPickup item)
    {
        if (_nearbyItem == item)
        {
            _nearbyItem = null;
        }
    }

    public void OnSelectSlot(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
        {
            return;
        }
        string input = ctx.control.name;
        switch (input)
        {
            case "1":
                slot.SelectSlot(0);
                break;
            case "2":
                slot.SelectSlot(1);
                break;
            case "3":
                slot.SelectSlot(2);
                break;
            case "4":
                slot.SelectSlot(3);
                break;
            case "5":
                slot.SelectSlot(4);
                break;
            case "6":
                slot.SelectSlot(5);
                break;
            case "7":
                slot.SelectSlot(6);
                break;
            case "8":
                slot.SelectSlot(7);
                break;
            case "9":
                slot.SelectSlot(8);
                break;
            case "0":
                slot.SelectSlot(9);
                break;
        }
    }
    public void OnUseItem(InputAction.CallbackContext ctx)
    {
        if (!ctx.started)
        {
            return;
        }
        QuickSlot slotData = slot.CurrentSlot;
        if (slotData == null || slotData.IsEmpty || slotData.Data == null)//빈슬롯
        {
            return;
        }
        ItemData data = slotData.Data;

        if (slot.TryUseCurrentSlot(slot.CurrentIndex))
        {
            if (data.type == ItemType.Consumable && data.prefab != null)//폭탄
            {
                GameObject obj = Instantiate(data.prefab, transform.position, Quaternion.identity);
                obj.GetComponent<Bomb>().UseBomb();
            }
            //혹은 키아이템 사용을 따로 할것이라면 아래에 추가
        }

    }
    public void OnSlotPrev(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
        {
            return;
        }
        slot.SelectPreviousSlot();
    }
    public void OnSlotNext(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
        {
            return;
        }
        slot.SelectNextSlot();
    }
}

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    public Vector2 boxSize = new Vector2(0.5f, 0.1f); // 박스 크기 (가로, 세로)
    public float castDistance = 0.1f; // 바닥 감지 거리
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

    //추가 액션
    InputAction interactAction;
    InputAction useItemAction;
    InputAction prevSlotAction;
    InputAction nextSlotAction;
    InputAction wheelAction;
    InputAction selectSlotAction;

    [Header("Chat System")]
    public GameObject chatBubbleCanvas; // 에디터에서 ChatBubbleCanvas 연결
    public TextMeshProUGUI chatText;    // 에디터에서 BubbleText 연결
    public float chatDuration = 1.5f;   // 말풍선 떠있는 시간

    // Runtime values
    float moveX;
    bool isGrounded;

    float coyoteTimer;
    float jumpBufferTimer;

    bool jumpHeld;
    float holdTimer;

    [SerializeField] float playerScale = 5f;

    // 컷신(자동 이동) 중인지 체크하는 변수
    bool isCutscene = false;


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
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _failSoundClip;
    private ItemPickup _nearbyItem;
    private QuickSlotController slot;
    public QuickSlotController Slot => slot;
    public AudioSource AudioSource => _audioSource;
    public AudioClip FailSoundClip => _failSoundClip;

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
        //추가 액션
        interactAction = map.FindAction("Interact", true);
        useItemAction = map.FindAction("UseItem", true);
        prevSlotAction = map.FindAction("SlotPrev", true);
        nextSlotAction = map.FindAction("SlotNext", true);
        wheelAction = map.FindAction("WheelScroll", true);
        selectSlotAction = map.FindAction("SlotSelect", true);
        //251216 - 양현용 : 게임매니저에 플레이어 전달
        GameManager.Instance.Player = this;
    }

    void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();

        interactAction.Enable();
        useItemAction.Enable();
        prevSlotAction.Enable();
        nextSlotAction.Enable();
        wheelAction.Enable();
        selectSlotAction.Enable();

        jumpAction.started += OnJumpStarted;
        jumpAction.canceled += OnJumpCanceled;

        interactAction.started += OnInteract;
        useItemAction.started += OnUseItem;
        prevSlotAction.started += OnSlotPrev;
        nextSlotAction.started += OnSlotNext;
        wheelAction.started += OnWheelScroll;
        selectSlotAction.performed += OnSelectSlot;
    }

    void OnDisable()
    {
        jumpAction.started -= OnJumpStarted;
        jumpAction.canceled -= OnJumpCanceled;

        interactAction.started -= OnInteract;
        useItemAction.started -= OnUseItem;
        prevSlotAction.started -= OnSlotPrev;
        nextSlotAction.started -= OnSlotNext;
        wheelAction.started -= OnWheelScroll;
        selectSlotAction.performed -= OnSelectSlot;

        moveAction.Disable();
        jumpAction.Disable();

        interactAction.Disable();
        useItemAction.Disable();
        prevSlotAction.Disable();
        nextSlotAction.Disable();
        wheelAction.Disable();
        selectSlotAction.Disable();

    }

    void Update()
    {
        if (!isCutscene)
        {
            moveX = moveAction.ReadValue<Vector2>().x;
        }

        // 1️⃣ Ground Check (BoxCast로 변경)
        // groundCheck 위치에서 boxSize 크기의 사각형을 아래로(Vector2.down) castDistance만큼 발사
        bool rawGrounded = Physics2D.BoxCast(
            groundCheck.position,
            boxSize,
            0f,
            Vector2.down,
            castDistance,
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

    // Unity 생명주기 Start 추가
    void Start()
    {
        // 현재 씬 이름이 "Map_Zone1"이면 등장 이벤트 시작
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Map_Zone1" && !CheckPointData.Instance._onCheck)
        {
            StartCoroutine(IntroWalkRoutine());
        }
    }

    // 등장 이벤트 코루틴
    IEnumerator IntroWalkRoutine()
    {
        // 1. 제어권 가져오기
        isCutscene = true;

        // 2. 대사 띄우기 (걷기 시작하면서 동시에 말함)
        // CSV를 사용하는 경우:
        string introMsg = GetStringFromTable("char_chat_0007");
        StartCoroutine(ShowChatBubble(introMsg));

        // (만약 CSV 연동이 아직 안 되었다면 아래처럼 직접 넣으세요)
        // StartCoroutine(ShowChatBubble("여기가 어디지...?"));

        // 3. 걷기 시작
        float startX = transform.position.x;
        float targetX = startX + 3f; // 3칸 이동

        moveX = 1f; // 오른쪽으로 이동 입력

        // 목표 지점에 도달할 때까지 대기
        while (transform.position.x < targetX)
        {
            yield return null;
        }

        // 4. 도착 후 정지 및 제어권 반환
        moveX = 0f;
        isCutscene = false;

        // 5. BGM 적용
        SoundEffectManager.Instance.PlayBGM(GameManager.Instance.Bgms[GameManager.Instance.CurScene]);
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
    void LateUpdate()
    {
        if (chatBubbleCanvas != null)
        {
            // 현재 플레이어(부모)가 보고 있는 방향 (1 또는 -1)
            float parentDirection = Mathf.Sign(transform.localScale.x);

            // 말풍선의 현재 크기 가져오기
            Vector3 bubbleScale = chatBubbleCanvas.transform.localScale;

            // 부모의 방향과 동일한 부호를 갖게 설정
            // 원리: 
            // 1. 플레이어(양수) * 말풍선(양수) = 정상 출력
            // 2. 플레이어(음수) * 말풍선(음수) = 정상 출력 (마이너스끼리 곱해서 플러스가 됨)
            chatBubbleCanvas.transform.localScale = new Vector3(
                Mathf.Abs(bubbleScale.x) * parentDirection,
                bubbleScale.y,
                bubbleScale.z
            );
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

        // 2. 말풍선 출력 로직 추가
        // "char_chat_damage" 키를 사용하여 텍스트를 가져옴 (DataManager 구조에 따라 수정 필요)
        string damageMsg = GetStringFromTable("char_chat_0001");

        StopAllCoroutines(); // 기존 말풍선 코루틴이 있다면 중지
        StartCoroutine(ShowChatBubble(damageMsg));
    }

    // CSV 데이터 가져오는 헬퍼 함수
    public string GetStringFromTable(string key)
    {
        if (GameManager.Instance != null && GameManager.Instance.StringTable != null)
        {
            var data = GameManager.Instance.StringTable[key];
            if (data != null)
            {
                //언어 설정에 따라 반환
                return GameManager.Instance.currentLanguage == Language.KR ? data.kr : data.en;
            }
        }
        return "Missing Text";
    }

    // 말풍선 띄우는 코루틴
    public IEnumerator ShowChatBubble(string msg)
    {
        if (chatBubbleCanvas != null && chatText != null)
        {
            chatText.text = msg;
            chatBubbleCanvas.SetActive(true); // 말풍선 활성화

            // 텍스트가 바뀔 때 크기 재계산이 한 프레임 늦을 수 있으므로 강제 업데이트
            LayoutRebuilder.ForceRebuildLayoutImmediate(chatBubbleCanvas.GetComponent<RectTransform>());

            yield return new WaitForSeconds(chatDuration);

            chatBubbleCanvas.SetActive(false); // 말풍선 비활성화
        }
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
        Vector3 center = groundCheck.position + (Vector3.down * castDistance * 0.5f);
        Gizmos.DrawWireCube(center, boxSize);
    }

    //251216 - 양현용 : 기믹과의 상호작용
    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.started && onPortal == 1)
        {
            GameManager.Instance.LoadNextScene();
        }
        if (ctx.started && onPortal == 2)
        {
            if (GameManager.Instance.PrimedForPhaseTwo && !GameManager.Instance.FiredPhaseTwoDialogue && GameManager.Instance.PhaseTwoCoroutine == null)
            {
                GameManager.Instance.PhaseTwoCoroutine = StartCoroutine(GameManager.Instance.SenseiCheckClear.SenseiStartPhaseTwo());
                GameManager.Instance.FiredPhaseTwoDialogue = true;
            }
            else
            {
                if (GameManager.Instance.SenseiCheckClear != null)
                {
                    if (GameManager.Instance.SenseiCheckClear.PhaseTwoUI != null)
                    {
                        if (GameManager.Instance.SenseiCheckClear.PhaseTwoUI.activeSelf)
                        {
                            GameManager.Instance.SenseiCheckClear.PhaseTwoUI.SetActive(false);
                            return;
                        }
                        else
                        {
                            GameManager.Instance.LoadPrevScene();
                        }
                    }
                    else
                    {
                        GameManager.Instance.LoadPrevScene();
                    }
                }
                else
                {
                    GameManager.Instance.LoadPrevScene();
                }
                //GameManager.Instance.LoadPrevScene();
            }
        }

        if (ctx.started && _checkGimmick && !GameManager.Instance.OnProgressGimmick)
        {
            _curGimmick.StartGimmick();
        }
        //else if (ctx.started && _checkGimmick && GameManager.Instance.OnProgressGimmick)
        //{
        //    _curGimmick.ExitGimmick();
        //}

        //아이템 관련 추가 
        //if (ctx.started && _nearbyItem != null)
        //{
        //    if (slot != null && _nearbyItem.ItemData.type == ItemType.Special)
        //    {
        //        if (slot.TryPickup(_nearbyItem.ItemData))
        //        {
        //            _nearbyItem.Pickup();
        //            _nearbyItem = null;
        //            return;

        //        }
        //    }
        //}
        if (ctx.started && _nearbyItem != null)
        {
            ItemData data = _nearbyItem.ItemData;

            if (slot != null && data.canQuickSlot)
            {
                if (slot.TryPickup(data))
                {
                    _nearbyItem.Pickup();
                    _nearbyItem = null;
                    return;
                }
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
            if (GameManager.Instance.SenseiCheckClear!=null)
            {
                Debug.Log(GameManager.Instance.SenseiCheckClear.CheckQuickSlotItem());
                Debug.Log(GameManager.Instance.CurrentCutsceneIndex);
                //Debug.Log(GameManager.Instance.GetTotalSceneCount());
                Debug.Log(GameManager.Instance.GetTotalSceneCount() - 2);
                if (GameManager.Instance.SenseiCheckClear.CheckQuickSlotItem() != -1 && GameManager.Instance.GetCurrentSceneIndex() == GameManager.Instance.GetTotalSceneCount() -2)
                {
                    onPortal = 2;
                    GameManager.Instance.PrimedForPhaseTwo = true;
                    //GameManager.Instance.SenseiCheckClear
                    return;
                }
                else
                {
                    onPortal = 2;
                    GameManager.Instance.PrimedForPhaseTwo = false;
                    return;
                }
            }

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


    public void OnSelectSlot(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
        {
            return;
        }
        string input = ctx.control.name;
        if (!GameManager.Instance.OnSelection && !GameManager.Instance.OnProgressGimmick)
        {
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
        else
        {
            switch (input)
            {
                case "1":
                    _curGimmick.CheckNum(1);
                    break;
                case "2":
                    _curGimmick.CheckNum(2);
                    break;
                case "3":
                    _curGimmick.CheckNum(3);
                    break;
            }
        }
    }

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

    public void OnUseItem(InputAction.CallbackContext ctx)
    {
        if (!ctx.started)
        {
            return;
        }

        QuickSlot slotData = slot.CurrentSlot;
        if (slotData == null || slotData.IsEmpty || slotData.Data == null)//빈슬롯
        {
            FailSound();
            return;
        }

        ItemData data = slotData.Data;
        if (data.type == ItemType.Key)
        {
            FailSound();
            return;
        }
        
        if (data.prefab.TryGetComponent(out Potion potion))
        {
            if (GameManager.Instance.CurrentPlayerHealth >= GameManager.Instance.MaxPlayerHealth)
            {
                Debug.Log("체력이 최대라 물약 사용 불가");
                FailSound();
                return;
            }
        }
        if (!slot.TryUseCurrentSlot(slot.CurrentIndex))
        {
            FailSound();
            return;
        }
        if (data.type == ItemType.Consumable && data.prefab != null)
        {
            // 물약
            if (data.prefab.TryGetComponent(out Potion potion2))
            {
                potion2.UsePotion();
            }
            // 폭탄
            else if (data.prefab.TryGetComponent(out Bomb bombPrefab))
            {
                GameObject obj = Instantiate(data.prefab, transform.position, Quaternion.identity);
                obj.GetComponent<Bomb>().UseBomb();
            }
        }
    }
    private void FailSound()
    {
        if (_audioSource == null || _failSoundClip == null)
        {
            return;
        }

        _audioSource.PlayOneShot(_failSoundClip);
    }
    public void OnSlotPrev(InputAction.CallbackContext ctx)
    {
        if (!ctx.started)
        {
            return;
        }
        slot.SelectPreviousSlot();
    }
    public void OnSlotNext(InputAction.CallbackContext ctx)
    {
        if (!ctx.started)
        {
            return;
        }
        slot.SelectNextSlot();
    }
    public void OnWheelScroll(InputAction.CallbackContext ctx)
    {
        if (!ctx.started)
        {
            return;
        }

        float scroll = ctx.ReadValue<float>();
        slot.WheelScroll(scroll);
    }
}

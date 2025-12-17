using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTest : MonoBehaviour
{

    Vector2 _moveDir;
    Rigidbody2D _rb;
    [SerializeField] float _moveSpeed;
    [SerializeField] float _jumpForce;
    [SerializeField] float _knockBackForce;

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
        {  return _curGimmick; }
        set
        {
            _curGimmick = value;
        }
    }

    [SerializeField] private int onPortal=0;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        _rb.linearVelocity = new Vector2(_moveDir.x * _moveSpeed, _rb.linearVelocity.y);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        _moveDir = ctx.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext ctx)
    {
        _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
    }
    public void TakeDamage()
    {
        Debug.Log("데미지 입음");

        _rb.AddForce(-_rb.linearVelocity * _knockBackForce, ForceMode2D.Impulse);
    }

    public void OnIneract(InputAction.CallbackContext ctx)
    {
        if (ctx.started && onPortal==1)
        {
            GameManager.Instance.LoadNextScene();
        }
        if (ctx.started && onPortal==2)
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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Finish"))
        {
            onPortal=1;
        }
        else if (collision.CompareTag("Respawn"))
        {
            onPortal=2;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Finish")|| collision.CompareTag("Respawn"))
        {
            onPortal = 0;
        }
    }
}

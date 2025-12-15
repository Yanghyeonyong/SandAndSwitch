using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTest : MonoBehaviour
{

    Vector2 _moveDir;
    Rigidbody2D _rb;
    [SerializeField] float _moveSpeed;

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

    public void TakeDamage()
    {
        Debug.Log("데미지 입음");
    }

    public void OnIneract(InputAction.CallbackContext ctx)
    {
        if (ctx.started && _checkGimmick && !GameManager_Hyeonyong.Instance.OnProgressGimmick)
        {
            _curGimmick.StartGimmick();
        }
        else if (ctx.started && _checkGimmick && GameManager_Hyeonyong.Instance.OnProgressGimmick)
        {
            _curGimmick.ExitGimmick();
        }
    }
}

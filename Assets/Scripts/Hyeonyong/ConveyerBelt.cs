using System.Collections;
using UnityEngine;

public class ConveyerBelt : MonoBehaviour
{

    [SerializeField] float _moveForce = 3f;
    [SerializeField] float _maxForce = 5f;
    Coroutine _coroutine;
    Rigidbody2D _rb;
    bool _onPush=false;
    float relativeSpeed;
    [SerializeField] bool _checkPhase=false;
    private void Start()
    {

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if(_checkPhase && GameManager.Instance.CheckItem)
        //    return;
        if (collision.gameObject.CompareTag("Player"))
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            _coroutine =StartCoroutine(PlayerMove());
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (_checkPhase && GameManager.Instance.CheckItem)
            return;
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("플레이어와 떨어졌다");
            //_play = false;
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }
    }

    IEnumerator PlayerMove()
    {
        Debug.Log("플레이어와 닿았다");
        if (_rb==null)
        {
            _rb = GameManager.Instance._player.GetComponent<Rigidbody2D>();
        }

        if(_moveForce<0)
        {
            _maxForce = -_maxForce;
        }


        while (true)
        {

            _onPush = false;
            if (_rb.IsSleeping()) _rb.WakeUp();


            //서로 반대방향으로 움직일 경우 힘을 작용한다 
            if (_rb.linearVelocity.x * _moveForce <= 0)
            {
                //점프 후 힘이 튀는 것 방지
                if (_rb.linearVelocity.x > 1f)
                {
                    _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
                }
                _onPush = true;
            }
            else
            {
                //_onPush = false;
                //왼쪽으로 미는 힘일 경우 플레이어의 x 이동이 마이너스 맥스 힘보다 클 경우 힘이 작용한다
                if (_moveForce < 0)
                    _onPush = _rb.linearVelocity.x > _maxForce;
                //오른쪽으로 미는 힘일 경우 플레이어의 x 이동이 맥스 힘보다 작을 경우 힘이 작용한다
                else
                    _onPush = _rb.linearVelocity.x < _maxForce;
            }
            if (_onPush)
            {

                //결과적으로 작용하는 힘  = 맥스 힘 - 플레이어의 이동 힘
                relativeSpeed = Mathf.Abs(_maxForce - Mathf.Min(1f,_rb.linearVelocity.x));
                //근데 이러면 점프 후 일정 확률로 먹히지 않는다 하지만 힘이 적용 된다고 나타난다.
                //그렇다면 높은 확률로 relativeSpeed가0인거 같은데 이러면 _rb.linearvelocity가 maxForce와 한없이 가깝다는 것?
                Debug.Log("플레이어 힘 작용 : "+relativeSpeed+" 현재 플레이어 힘 : "+ _rb.linearVelocity.x);
                _rb.AddForce(Vector2.right * _moveForce * relativeSpeed, ForceMode2D.Force);
            }

            yield return new WaitForFixedUpdate();
        }
    }
}

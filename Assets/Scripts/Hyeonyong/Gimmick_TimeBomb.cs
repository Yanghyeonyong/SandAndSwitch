using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Gimmick_TimeBomb : Gimmick
{
    [SerializeField] float _timer;
    Animator _animator;
    AudioSource _audio;
    Coroutine _coroutine;
    Rigidbody2D _rb;
    [SerializeField] float _weight = 5f;
    [SerializeField] float _range = 6.5f;
    private void Start()
    {
        if (CheckClear())
        {
            gameObject.SetActive(false);
        }
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")&&_coroutine == null)
        {
            Debug.Log("타이머 플레이어와 닿았다");
            _coroutine = StartCoroutine(StartTimer());
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x / _weight, 0);
            //Debug.Log("플레이어와 충돌");
            //_rb.AddForce(-_rb.linearVelocity, ForceMode2D.Force);
        }
    }



    IEnumerator StartTimer()
    {
        Debug.Log("타이머 코루틴 시작");
        yield return new WaitForSeconds(_timer);
        _rb.bodyType = RigidbodyType2D.Static;
        //GetComponent<CircleCollider2D>().isTrigger = true;
        transform.localScale = Vector3.one * _range;
        _animator.SetTrigger("Explosion");

        int playerLayer = LayerMask.GetMask("Player");
        Collider2D hit = Physics2D.OverlapCircle(transform.position, _range / 2f, playerLayer);
        if (hit != null)
        {
            hit.gameObject.GetComponent<Player>().TakeDamage();
        }

        yield return null;
        float _animationLength = _animator.GetCurrentAnimatorStateInfo(0).length;
        _audio.Play();

        yield return new WaitForSeconds(_animationLength);
        IsClear=true;
        GameManager.Instance.IsGimmickClear[GimmickId] = true;
        gameObject.SetActive(false);

    }
}

using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Gimmick_TimeBomb : Gimmick
{
    [SerializeField] float _timer;
    Animator _animator;
    AudioSource _audio;
    Coroutine _coroutine;
    Rigidbody2D _rb;
    [SerializeField] float _weight = 5f;
    [SerializeField] float _range = 6.5f;
    public bool _isUse = false;

    [SerializeField] AudioClip[] _clip;
    private void Start()
    {

        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
        if (CheckClear())
        {
            BreakRadius();
            gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(StartTimer());
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player")&&_coroutine == null)
    //    {
    //        Debug.Log("타이머 플레이어와 닿았다");
    //        _coroutine = StartCoroutine(StartTimer());
    //    }
    //}

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
        if (_isUse) 
        {
            yield break;
                
        }
        _audio.clip = _clip[0];
        _audio.Play();
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
        BreakRadius();

        yield return null;
        float _animationLength = _animator.GetCurrentAnimatorStateInfo(0).length;
        //_audio.Play();
        _audio.clip = _clip[1];
        _audio.Play();

        yield return new WaitForSeconds(_animationLength);
        IsClear=true;
        GameManager.Instance.IsGimmickClear[GimmickId] = true;
        //BreakRadius();
        gameObject.SetActive(false);

    }

    private void BreakRadius()
    {
        Vector3 origin = transform.position;

        // 타일맵 탐색
        int breakWall = LayerMask.GetMask("BreakWall");
        Collider2D[] maps = Physics2D.OverlapCircleAll(origin, _range, breakWall);

        foreach (var col in maps)
        {
            Tilemap map = col.GetComponent<Tilemap>();
            if (map == null)
            {
                continue;
            }

            // 폭발 범위 내부의 타일만 제거
            BoundsInt bounds = map.cellBounds;

            for (int x = bounds.xMin; x <= bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y <= bounds.yMax; y++)
                {
                    Vector3Int cell = new Vector3Int(x, y, 0);
                    Vector3 world = map.GetCellCenterWorld(cell);

                    if (Vector2.Distance(origin, world) <= _range)
                    {
                        map.SetTile(cell, null);
                    }
                }
            }
        }
    }
}

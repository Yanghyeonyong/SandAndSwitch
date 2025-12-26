using System.Collections;
using UnityEngine;

public class MovingTile : MonoBehaviour
{
    [SerializeField] Transform[] _movingDirection;
    Vector3[] _dir;
    [SerializeField] float _moveSpeed = 0.1f;
    int _curDir = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _dir = new Vector3[_movingDirection.Length];

        for (int i = 0; i < _movingDirection.Length; i++)
        {
            if (i + 1 == _movingDirection.Length)
            {
                _dir[i] = (_movingDirection[i].position- _movingDirection[0].position).normalized;
                break;
            }
            _dir[i]= (_movingDirection[i].position- _movingDirection[i+1].position).normalized;
        }

        StartCoroutine(Move());

    }

    IEnumerator Move()
    {
        while (true)
        {
            //Debug.Log("거리: "+Vector3.Distance(transform.position, _movingDirection[_curDir].position));
            if (Vector3.Distance(transform.position, _movingDirection[_curDir].position) <= 0.3f)
            {
                //Debug.Log("전환");
                _curDir++;
                if (_curDir == _movingDirection.Length)
                {
                    _curDir = 0;
                }
            }


            transform.Translate(_dir[_curDir]*Time.deltaTime*_moveSpeed);
            yield return null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.SetParent(null);
        }
    }
}

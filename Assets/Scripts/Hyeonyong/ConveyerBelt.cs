using System.Collections;
using UnityEngine;

public class ConveyerBelt : MonoBehaviour
{

    [SerializeField] float _moveForce = 3f;
    Coroutine _coroutine;
    Rigidbody2D _rb;

    bool _checkPhase=false;
    private void Start()
    {

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(_checkPhase && GameManager.Instance.CheckItem)
            return;
        if (collision.gameObject.CompareTag("Player"))
        {
            
            _coroutine=StartCoroutine(PlayerMove());
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (_checkPhase && GameManager.Instance.CheckItem)
            return;
        if (collision.gameObject.CompareTag("Player"))
        {
            StopCoroutine(_coroutine);
        }
    }

    IEnumerator PlayerMove()
    {
        _rb = GameManager.Instance._player.GetComponent<Rigidbody2D>();
        while (true)
        {
            _rb.linearVelocity += Vector2.right*_moveForce;
            yield return null;
        }
    }
}

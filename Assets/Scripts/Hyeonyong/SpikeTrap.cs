using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    //플레이어와 충돌 시 데미지를 입히는 스크립트

    [SerializeField] float _damageDelay = 0.1f;
    ContactPoint2D _contactPoint;
    Vector2 _normal;
    [SerializeField] float _attackForce = 10f;
//WaitForSeconds _wait;
//    bool _active = false;
//    Coroutine _damageCoroutine;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _contactPoint = collision.contacts[0];
            _normal= _contactPoint.normal;
            //Debug.Log(_normal);
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(-_normal * _attackForce,ForceMode2D.Impulse);

            collision.gameObject.GetComponent<Player>().TakeDamage();
        }
    }



    //private void Start()
    //{
    //    _wait = new WaitForSeconds(_damageDelay);
    //}

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player"))
    //    {
    //        _active = true;
    //        //해당 스크립트는 테스트용이며, 이후 변경 예정
    //        collision.gameObject.GetComponent<Player>().TakeDamage();
    //    }
    //}

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player") && _damageCoroutine == null && _active== false)
    //    {
    //        Debug.Log("스테이");
    //        _damageCoroutine = StartCoroutine(DamageDelay(collision.gameObject.GetComponent<Player>()));
    //        _active = true;
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player") && _damageCoroutine != null)
    //    {
    //        StopCoroutine(_damageCoroutine);
    //        _damageCoroutine = null;
    //        _active = false;
    //    }
    //}

    //IEnumerator DamageDelay(Player player)
    //{
    //    yield return _wait;
    //    player.TakeDamage();
    //}
}

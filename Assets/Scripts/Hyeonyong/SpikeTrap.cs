using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    //플레이어와 충돌 시 데미지를 입히는 스크립트

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //해당 스크립트는 테스트용이며, 이후 변경 예정
            collision.gameObject.GetComponent<Player>().TakeDamage();
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        //해당 스크립트는 테스트용이며, 이후 변경 예정
    //        collision.gameObject.GetComponent<PlayerTest>().TakeDamage();
    //    }
    //}
}

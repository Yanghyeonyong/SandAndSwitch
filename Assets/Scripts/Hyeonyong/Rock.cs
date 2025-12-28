using UnityEngine;

public class Rock : MonoBehaviour
{
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        gameObject.SetActive(false);
    //        collision.gameObject.GetComponent<Player>().TakeDamage();
    //    }
    //    else if (collision.gameObject.layer==6)
    //    {
    //        gameObject.SetActive(false);
    //    }
    //}
    [SerializeField] float _attackForce = 2f;
    ContactPoint2D _contactPoint;
    Vector2 _normal;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("´ê¾Ò´Ù :" + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Player"))
        {
            _contactPoint = collision.contacts[0];
            _normal = _contactPoint.normal;
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(-_normal * _attackForce, ForceMode2D.Impulse);

            collision.gameObject.GetComponent<Player>().TakeDamage();
            gameObject.SetActive(false);
        }

        else if (collision.gameObject.layer == 6)
        {
            gameObject.SetActive(false);
        }
    }
}

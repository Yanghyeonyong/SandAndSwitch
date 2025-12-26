using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] float _moveSpeed;
    [SerializeField] float _attackForce = 10f;
    ContactPoint2D _contactPoint;
    Vector2 _normal;
    private void Update()
    {
        transform.Translate(Vector3.right * _moveSpeed * Time.deltaTime);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("´ê¾Ò´Ù :"+ collision.gameObject.name);
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

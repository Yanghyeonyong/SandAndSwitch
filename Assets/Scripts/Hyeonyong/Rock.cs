using UnityEngine;

public class Rock : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            collision.gameObject.GetComponent<Player>().TakeDamage();
        }
        else if (collision.gameObject.layer==6)
        {
            gameObject.SetActive(false);
        }
    }
}

using UnityEngine;

public class Gimmick_Weight : MonoBehaviour
{
    Rigidbody2D _rb;
    [SerializeField] float _weight = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb=GetComponent<Rigidbody2D>();
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) 
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x / _weight, _rb.linearVelocity.y);
            //Debug.Log("플레이어와 충돌");
            //_rb.AddForce(-_rb.linearVelocity, ForceMode2D.Force);
        }
    }
}

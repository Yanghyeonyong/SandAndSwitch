using UnityEngine;

public class Gimmick_Weight : Gimmick
{
    Rigidbody2D _rb;
    [SerializeField] float _weight = 5f;
    CheckGround _checkGround;

    AudioSource _audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb=GetComponent<Rigidbody2D>();
        if (CheckPos()!=null)
        {
            transform.position = CheckPos().position;
            transform.rotation = CheckPos().rotation;
        }
        _checkGround = transform.GetChild(0).GetComponent<CheckGround>();
        _audioSource=GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6 && !_checkGround._isGround)
        {
            _audioSource.Play();
            _checkGround._isGround = true;
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
    private void OnDisable()
    {
        ItemTransform myTransform = new ItemTransform(this.transform.position, this.transform.rotation, this.transform.localScale);
        GameManager.Instance.GimmickPos[GimmickId] = myTransform;
        Debug.Log("저장하기: "+transform) ;
    }
}

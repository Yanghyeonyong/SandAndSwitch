using System.Collections;
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
        StartCoroutine(CheckMyPos());   
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
            if(_checkGround._isGround) 
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x / _weight, 0);
            else
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x / _weight, _rb.linearVelocity.y);
            //Debug.Log("플레이어와 충돌");
            //_rb.AddForce(-_rb.linearVelocity, ForceMode2D.Force);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
           _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
        }
    }

    private void OnDisable()
    {
        //이게 더 늦게 실행되서 그런거 같은데
        ItemTransform myTransform = new ItemTransform(this.transform.position, this.transform.rotation, this.transform.localScale);
        GameManager.Instance.GimmickPos[GimmickId] = myTransform;
        Debug.Log("저장오기: "+transform.position) ;
    }

    public void SaveMyPos()
    {
        //이게 더 늦게 실행되서 그런거 같은데
        ItemTransform myTransform = new ItemTransform(this.transform.position, this.transform.rotation, this.transform.localScale);
        GameManager.Instance.GimmickPos[GimmickId] = myTransform;
        Debug.Log("저장오기: " + transform.position);
    }
    IEnumerator CheckMyPos()
    {
        yield return null;
        if (CheckPos() != null)
        {
            Debug.Log("위치값 변경");
            transform.position = CheckPos().position;
            transform.rotation = CheckPos().rotation;
        }
    }
}

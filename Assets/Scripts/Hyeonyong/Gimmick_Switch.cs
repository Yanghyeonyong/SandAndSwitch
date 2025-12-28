using UnityEngine;

public class Gimmick_Switch : Gimmick
{
    [SerializeField] bool _turnSwitch = false;
    Gimmick_Object _obj;
    Animator _animator;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioSource _gimmickObjAudioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
        if (CheckClear())
        {

        }
        _obj = GetComponent<Gimmick_Object>();
        //_obj._audioSource = _gimmickObjAudioSource;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Box"))
        {
            if(_audioSource != null) 
                {
                    _audioSource.Play();
                }
            if (_obj._audioSource == null)
            {
                _obj._audioSource = _gimmickObjAudioSource;
            }

            _obj.TurnOn();
            _animator.SetTrigger("TurnOn");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Box"))
        {
            if (_audioSource != null)
            {
                _audioSource.Play();
            }
            _obj.TurnOff();
            _animator.SetTrigger("TurnOff");
        }
    }
}

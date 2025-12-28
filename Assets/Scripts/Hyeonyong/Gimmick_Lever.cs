using UnityEngine;

public class Gimmick_Lever : Gimmick
{
    [SerializeField] bool _turnLever=false;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioSource _gimmickObjAudioSource;
    Gimmick_Object _obj;
    Animator _animator;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
        _obj = GetComponent<Gimmick_Object>();
        if (CheckClear())
        {
            _turnLever=true;
            _obj.TurnOn();
            _animator.SetTrigger("TurnOn");
        }


        //_audioSource = GetComponent<AudioSource>();

    }

    public override void StartGimmick()
    {
        if (_obj._audioSource == null)
        {
            _obj._audioSource = _gimmickObjAudioSource;
        }
        _turnLever = !_turnLever;
        _audioSource.Play();
        if (_turnLever)
        {

            _obj.TurnOn();
            _animator.SetTrigger("TurnOn");
            GameManager.Instance.IsGimmickClear[GimmickId] = true;
        }
        else
        {

            _obj.TurnOff();
            _animator.SetTrigger("TurnOff");
            GameManager.Instance.IsGimmickClear[GimmickId] = false;
        }
    }
}

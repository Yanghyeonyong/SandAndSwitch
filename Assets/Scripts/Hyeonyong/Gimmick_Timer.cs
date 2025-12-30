using System.Collections;
using UnityEngine;

public class Gimmick_Timer : Gimmick
{
    [SerializeField] bool _turnSwitch = false;
    Gimmick_Object _obj;
    Animator _animator;
    [SerializeField] float _timer;
    Coroutine _coroutine;
    AudioSource _audioSource;


    AudioSource _childAudioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
        //if (CheckClear())
        //{
        //    _obj.TurnOn();
        //    _animator.SetTrigger("TurnOn");
        //}
        _obj = GetComponent<Gimmick_Object>();
        _audioSource = GetComponent<AudioSource>();

        foreach (Transform child in transform)
        {
            if (child.gameObject.name == "ChildAudioSource")
            {
                _childAudioSource = child.GetComponent<AudioSource>();
            }
        }
    }

    public override void StartGimmick()
    {
        


        if (_coroutine != null)
        {
            StopCoroutine( _coroutine );
            _coroutine = null;
        }
       _coroutine = StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        _childAudioSource.Play();
        _audioSource.Play();
        _obj.TurnOn();
        _animator.Play("TurnOn", -1, 0f);
        Debug.Log("타이머 애니메이션 실행");
        yield return new WaitForSeconds(_timer);
        Debug.Log("시간 종료");
        _obj.TurnOff();
        _coroutine = null;
        //GameManager.Instance.IsGimmickClear[GimmickId] = true;
    }

    IEnumerator StartTimer_SecondClick()
    {
        _animator.Play("TurnOn", -1, 0f);
        yield return new WaitForSeconds(_timer);
        _obj.TurnOff();
        _coroutine = null;
        //GameManager.Instance.IsGimmickClear[GimmickId] = true;
    }

}

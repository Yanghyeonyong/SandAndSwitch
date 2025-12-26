using System.Collections;
using UnityEngine;

public class Gimmick_Timer : Gimmick
{
    [SerializeField] bool _turnSwitch = false;
    Gimmick_Object _obj;
    Animator _animator;
    [SerializeField] float _timer;
    Coroutine _coroutine;

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
    }

    public override void StartGimmick()
    {
        if (_coroutine == null)
        {
            _coroutine = StartCoroutine(StartTimer());
        }
        if (_coroutine != null)
        {
            StopCoroutine( _coroutine );
            _coroutine = null;
            _coroutine=StartCoroutine(StartTimer_SecondClick());
        }
    }

    IEnumerator StartTimer()
    {
        _obj.TurnOn();
        _animator.Play("TurnOn", -1, 0f);
        yield return new WaitForSeconds(_timer);
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

using System.Collections;
using UnityEngine;

public class Gimmick_Timer : Gimmick
{
    [SerializeField] bool _turnSwitch = false;
    Gimmick_Object _obj;
    Animator _animator;
    [SerializeField] float _timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
        if (CheckClear())
        {
            _obj.TurnOn();
            _animator.SetTrigger("TurnOn");
        }
        _obj = GetComponent<Gimmick_Object>();
    }

    public override void StartGimmick()
    {
        StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(_timer);
        _obj.TurnOn();
        _animator.SetTrigger("TurnOn");
        GameManager.Instance.IsGimmickClear[GimmickId] = true;
    }

}

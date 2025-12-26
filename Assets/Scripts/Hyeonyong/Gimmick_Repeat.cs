using System.Collections;
using UnityEngine;

public class Gimmick_Repeat : Gimmick_Object
{
    Gimmick_Object _obj;
    [SerializeField] float _repeatTime =2f;
    [SerializeField] bool _startRepeat=false;
    [SerializeField] Coroutine _coroutine;
    bool _turnOn=false;
    [SerializeField] bool _checkPhase=false;
    private void Start()
    {
        if (_checkPhase && GameManager.Instance.CheckItem)
            return;
        //주의 : 기믹리피트는 가장 상단에 배치할 것
        //다른 기믹들은 가장 상단에 있는 기믹 오브젝트를 가져오므로 만약 기믹 리피트를 쓴다면 가장 상단에 있어야 가져옴 
        Gimmick_Object[] _objs= GetComponents<Gimmick_Object>();
        _obj = _objs[_objs.Length-1];
        if (_startRepeat)
        {
            _coroutine = StartCoroutine(StartRepeat());
        }
    }

    public override void TurnOn()
    {
        if (_coroutine == null)
        {
            _coroutine = StartCoroutine(StartRepeat());
        }
    }
    public override void TurnOff() 
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    IEnumerator StartRepeat()
    {

        while (true)
        {
            if (!_turnOn)
            {
                _obj.TurnOn();
            }
            else
            {
                _obj.TurnOff();
            }
            _turnOn = !_turnOn;
            yield return new WaitForSeconds(_repeatTime);
        }
    }


}

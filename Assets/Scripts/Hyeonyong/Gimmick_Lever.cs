using UnityEngine;

public class Gimmick_Lever : Gimmick
{
    [SerializeField] bool _turnLever=false;
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

    }

    public override void StartGimmick()
    {
        _turnLever = !_turnLever;
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

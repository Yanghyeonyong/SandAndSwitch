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
        if (CheckClear())
        {
            
        }
        _obj = GetComponent<Gimmick_Object>();
    }

    public override void StartGimmick()
    {
        _turnLever = !_turnLever;
        if (_turnLever)
        {

            _obj.TurnOn();
            _animator.SetTrigger("TurnOn");
        }
        else
        {

            _obj.TurnOff();
            _animator.SetTrigger("TurnOff");
        }
    }
}

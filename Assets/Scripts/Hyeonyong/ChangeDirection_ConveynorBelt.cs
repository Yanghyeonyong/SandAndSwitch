using UnityEngine;

public class ChangeDirection_ConveynorBelt : Gimmick_Object
{
    [SerializeField] ConveyerBelt _conveynerBelt;
    float _moveForce;
    private void Start()
    {
        _moveForce = _conveynerBelt._moveForce;
    }
    public override void TurnOn()
    {
        _conveynerBelt._moveForce = - _moveForce;
    }
    public override void TurnOff()
    {
        _conveynerBelt._moveForce = _moveForce;
    }
}

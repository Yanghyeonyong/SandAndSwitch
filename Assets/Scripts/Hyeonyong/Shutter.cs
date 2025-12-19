using UnityEngine;

public class Shutter : Gimmick_Object
{
    [SerializeField] GameObject _shutterObject;
    Animator _animator;
    [SerializeField] GameObject _interactiveObj;
    private void Start()
    {
        _animator = _shutterObject.GetComponent<Animator>();
    }

    public override void TurnOn()
    {
        _interactiveObj.SetActive(true);
        //_shutterObject.transform.localPosition += new Vector3(0f,5f,0f);
        _animator.SetTrigger("TurnOn");
    }
    public override void TurnOff()
    {
        _interactiveObj.SetActive(false);
        //_shutterObject.transform.localPosition += new Vector3(0f, -5f, 0f);
        _animator.SetTrigger("TurnOff");
    }
}

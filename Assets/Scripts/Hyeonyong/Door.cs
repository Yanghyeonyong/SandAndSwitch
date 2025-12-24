using UnityEngine;

public class Door : Gimmick_Object
{
    [SerializeField] GameObject[] _doorObject;
    Animator[] _animator;
    [SerializeField] GameObject[] _interactiveObj;
    private void Start()
    {
        _animator = new Animator[_doorObject.Length];
        for (int i = 0; i < _doorObject.Length; i++)
        {
            _animator[i] = _doorObject[i].GetComponent<Animator>();
        }
    }

    public override void TurnOn()
    {
        for (int i = 0; i < _animator.Length; i++)
        {
            _interactiveObj[i].SetActive(true);
            //_shutterObject.transform.localPosition += new Vector3(0f,5f,0f);
            _animator[i].SetTrigger("TurnOn");
        }
    }
    public override void TurnOff()
    {
        for (int i = 0; i < _animator.Length; i++)
        {
            _interactiveObj[i].SetActive(false);
            //_shutterObject.transform.localPosition += new Vector3(0f, -5f, 0f);
            _animator[i].SetTrigger("TurnOff");
        }
    }
}

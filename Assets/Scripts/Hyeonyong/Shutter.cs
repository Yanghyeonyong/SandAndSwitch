using UnityEngine;

public class Shutter : Gimmick_Object
{
    [SerializeField] GameObject _shutterObject;
    private void Start()
    {
        //_animator = _shutterObject.GetComponent<Animator>();
    }

    public override void TurnOn()
    {
        _shutterObject.transform.localPosition += new Vector3(0f, 5f, 0f);
    }
    public override void TurnOff()
    {
        _shutterObject.transform.localPosition += new Vector3(0f, -5f, 0f);
    }
}

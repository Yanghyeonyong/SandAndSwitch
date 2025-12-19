using UnityEngine;

public class Shutter : Gimmick_Object
{
    [SerializeField] GameObject _shutterObject;
    
    public override void TurnOn()
    {
        _shutterObject.transform.localPosition += new Vector3(0f,5f,0f);
    }
    public override void TurnOff()
    {
        _shutterObject.transform.localPosition += new Vector3(0f, -5f, 0f);
    }
}

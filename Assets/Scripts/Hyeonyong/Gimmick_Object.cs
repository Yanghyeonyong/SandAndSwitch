using UnityEngine;

public abstract class Gimmick_Object : MonoBehaviour
{
    public AudioSource _audioSource;
    public virtual void TurnOn()
    { }
    public virtual void TurnOff() 
    { }
}

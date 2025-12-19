using UnityEngine;

public class Gimmick_Switch : Gimmick
{
    [SerializeField] bool _turnSwitch = false;
    Gimmick_Object _obj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (CheckClear())
        {

        }
        _obj = GetComponent<Gimmick_Object>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _obj.TurnOn();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _obj.TurnOff();
        }
    }
}

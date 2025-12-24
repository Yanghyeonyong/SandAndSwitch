using System.Collections;
using UnityEngine;

public class FlickerBlock : MonoBehaviour
{
    [SerializeField] float _flickerDelay = 3f;
    [SerializeField] GameObject _block;
    [SerializeField] Vector3 _pos;
    WaitForSeconds wait;
    Coroutine _coroutine;
    bool _onFlicker=false;

    [SerializeField] bool _checkPhase=false;
    private void Start()
    {
        wait= new WaitForSeconds(_flickerDelay);
        _coroutine=StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {

        if (GameManager.Instance.CheckItem&&_checkPhase)
        {
            yield break;
        }
        while (true)
        {
            if (_onFlicker)
            {
                _block.transform.position += _pos;
                _onFlicker = false;
            }
            else
            {
                _block.transform.position -= _pos;
                _onFlicker = true;
            }
            yield return wait;
        }
    }
    private void OnDisable()
    {
        StopCoroutine( _coroutine );
    }
}

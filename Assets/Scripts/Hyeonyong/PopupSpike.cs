using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PopupSpike : Gimmick_Object
{
    [SerializeField] GameObject _spikeObject;
    [SerializeField] float _moveSpeed = 10f;
    //[SerializeField] Transform _edge;
    [SerializeField] Transform[] _pos;
    Coroutine _curCoroutine;
    [SerializeField] SpikeTrap _spikeTrap;
    private void Start()
    {
    }

    public override void TurnOn()
    {
        Debug.Log("올라가기 시도");
        if (_curCoroutine != null)
        {
            StopCoroutine(_curCoroutine);
            _curCoroutine = null;
        }
        _curCoroutine = StartCoroutine(Open());
    }
    public override void TurnOff()
    {
        if (_curCoroutine != null)
        {
            StopCoroutine(_curCoroutine);
            _curCoroutine = null;
        }
        _curCoroutine = StartCoroutine(Close());
    }

    IEnumerator Open()
    {

        //대기시간이어도 올라갈때는 데미지 받아야함
        _spikeTrap._onDamage=false;
        //while (Vector3.Distance(_spikeObject.transform.position, _pos[0].position) > 0.1f)
        while (_spikeObject.transform.localPosition.y < _pos[0].localPosition.y)
        {
            //_spikeObject.transform.position = Vector3.MoveTowards(_spikeObject.transform.position,_pos[0].position, _moveSpeed);
            _spikeObject.transform.Translate(Vector3.up * Time.deltaTime * _moveSpeed* Vector3.Distance(_spikeObject.transform.position, _pos[0].position));
            yield return null;
            //if (_spikeObject.transform.localPosition.y >= _pos[0].localPosition.y)
            //{
            //    _spikeObject.transform.localPosition = _pos[0].localPosition;
            //    break;
            //}
        }
    }

    IEnumerator Close()
    {
        //while (Vector3.Distance(_spikeObject.transform.position, _pos[1].position) > 0.1f)
        while (_spikeObject.transform.localPosition.y > _pos[1].localPosition.y)
        {
            //_spikeObject.transform.position = Vector3.MoveTowards(_spikeObject.transform.position, _pos[1].position, _moveSpeed);
            _spikeObject.transform.Translate(Vector3.down * Time.deltaTime * _moveSpeed);
            yield return null;
            //if (_spikeObject.transform.localPosition.y < _pos[1].localPosition.y)
            //{
            //    _spikeObject.transform.localPosition = _pos[1].localPosition;
            //    break;
            //}
        }
    }
}

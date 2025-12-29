using System.Collections;
using UnityEngine;

public class Shutter : Gimmick_Object
{
    [SerializeField] GameObject _shutterObject;
    [SerializeField] float _moveSpeed = 3f;
    [SerializeField] Transform _edge;
    [SerializeField] Transform[] _pos;
    Coroutine _curCoroutine;
    private void Start()
    {
        //_animator = _shutterObject.GetComponent<Animator>();
    }

    public override void TurnOn()
    {
        Debug.Log("올라가기 시도");
        //_shutterObject.transform.localPosition += new Vector3(0f, 5f, 0f);
        if (_curCoroutine != null)
        {
            StopCoroutine(_curCoroutine);
            _curCoroutine = null;
        }
        _curCoroutine = StartCoroutine(Open());
    }
    public override void TurnOff()
    {
        //_shutterObject.transform.localPosition += new Vector3(0f, -5f, 0f);
        if (_curCoroutine != null)
        {
            StopCoroutine(_curCoroutine);
            _curCoroutine = null;
        }
        _curCoroutine = StartCoroutine(Close());
    }

    IEnumerator Open()
    {
        Debug.Log("올라가려고 함");
        while (Vector3.Distance(_edge.position, _pos[0].position) > 0.3f)
        {
            Debug.Log("올라간다");
            _shutterObject.transform.Translate(Vector3.up * Time.deltaTime * _moveSpeed);
            yield return null;

        }
    }

    IEnumerator Close()
    {
        while (Vector3.Distance(_edge.position, _pos[1].position) > 0.3f)
        {
            Debug.Log("내려간다");
            _shutterObject.transform.Translate(Vector3.down * Time.deltaTime * _moveSpeed);
            yield return null;

        }
    }
    private void OnDisable()
    {
        if (_curCoroutine != null)
        {
            StopCoroutine(_curCoroutine);
            _curCoroutine = null;
        }
    }
}

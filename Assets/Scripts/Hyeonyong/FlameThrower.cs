using System.Collections;
using UnityEngine;

public class FlameThrower : Gimmick_Object
{
    [SerializeField] Transform _colliderBox;
    Animator _animator;
    [SerializeField] float _startY = 0f;
    [SerializeField] float _rangeY = 2.2f;
    [SerializeField] bool _checkPhase = false;
    Coroutine _coroutine;
    private void Start()
    {
        _animator=GetComponent<Animator>();
    }
    public override void TurnOn()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
        _coroutine = StartCoroutine(StartFire());
    }
    public override void TurnOff()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
        _coroutine = StartCoroutine(FinishFire());
    }

    IEnumerator StartFire()
    {
        _animator.SetTrigger("TurnOn");
        yield return null;
        float _animationLength = _animator.GetCurrentAnimatorStateInfo(0).length;
        float curTime = 0f;
        Vector3 originScale = _colliderBox.localScale;

        while (curTime < _animationLength)
        {
            curTime += Time.deltaTime;

            float changeY = Mathf.Lerp(_startY, _rangeY, Mathf.Clamp01(curTime / _animationLength));

            // 스케일 적용
            _colliderBox.localScale = new Vector3(originScale.x, changeY, originScale.z);

            yield return null;
        }
        _colliderBox.localScale = new Vector3(originScale.x, _rangeY, originScale.z);

    }


    IEnumerator FinishFire()
    {
        _animator.SetTrigger("TurnOff");
        yield return null;
        float _animationLength = _animator.GetCurrentAnimatorStateInfo(0).length;
        float curTime = 0f;
        Vector3 originScale = _colliderBox.localScale;

        while (curTime < _animationLength)
        {
            curTime += Time.deltaTime;

            float changeY = Mathf.Lerp(_rangeY, _startY, Mathf.Clamp01(curTime / _animationLength));

            // 스케일 적용
            _colliderBox.localScale = new Vector3(originScale.x, changeY, originScale.z);

            yield return null;
        }
        _colliderBox.localScale = new Vector3(originScale.x, _startY, originScale.z);
    }
}

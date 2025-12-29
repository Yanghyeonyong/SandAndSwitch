using NUnit.Framework;
using System.Collections;
using UnityEngine;

public class Gimmick_Switch : Gimmick
{
    [SerializeField] bool _turnSwitch = false;
    Gimmick_Object _obj;
    Animator _animator;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioSource _gimmickObjAudioSource;
    [SerializeField] bool _checkFirst=false;

    bool _onPlayer=false;
    bool _onBox=false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
        if (CheckClear())
        {
            _checkFirst = true;
        }
        _obj = GetComponent<Gimmick_Object>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Box"))
        {
            if (collision.CompareTag("Player"))
            {
                _onPlayer = true;
            }
            else if (collision.CompareTag("Box"))
            {
                _onBox = true;
            }

            if (_audioSource != null) 
                {
                    _audioSource.Play();
                }
            if (_obj._audioSource == null)
            {
                if (!_checkFirst)
                {
                    Debug.Log("스위치 비정상화");
                    _obj._audioSource = _gimmickObjAudioSource;
                }
                else
                {
                    Debug.Log("스위치 정상화");
                    _checkFirst = true;
                }
            }

            if (!_turnSwitch)
            {
                _obj.TurnOn();
                _animator.SetTrigger("TurnOn");
                _turnSwitch = true;
            }
            //_obj.TurnOn();
            //_animator.SetTrigger("TurnOn");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Box"))
        {
            if (collision.CompareTag("Player"))
            {
                //_onPlayer = false;
                StartCoroutine(CheckFalse(0));
            }
            else if (collision.CompareTag("Box"))
            {
                StartCoroutine(CheckFalse(1));
            }
            if (_audioSource != null)
            {
                _audioSource.Play();
            }

            if (!_onPlayer && !_onBox)
            {
                _obj.TurnOff();
                _animator.SetTrigger("TurnOff");
                _turnSwitch=false;
            }
        }
    }

    private void OnDisable()
    {
        if (_onPlayer || _onBox)
        {
            GameManager.Instance.IsGimmickClear[GimmickId] = true;
            Debug.Log("스위치 트루");
        }
        else
        {
            GameManager.Instance.IsGimmickClear[GimmickId] = false;
            Debug.Log("스위치 펄스");
        }
            StopAllCoroutines();
    }

    IEnumerator CheckFalse(int num)
    {
        yield return null;
        switch (num)
        {
            case 0:
                _onPlayer = false;
                break;
            case 1:
                _onPlayer = false;
                break;
        }
    }
}

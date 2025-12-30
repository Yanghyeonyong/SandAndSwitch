using System.Collections;
using UnityEngine;

public class Gimmick_CheckPoint : Gimmick
{
    Animator _animator;
    bool _onCoolTime=false;
    [SerializeField] float _coolTime = 3f;
    AudioSource _audioSource;
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }
    public override void StartGimmick()
    {
        if (!_onCoolTime)
        {
            StartCoroutine(StartCoolTime());
            if (GameManager.Instance.CheckPointData == null)
            {
                GameManager.Instance.CheckPointData = new CheckPointData();
            }
            GameManager.Instance.CheckPointData.Init();
            Debug.Log("체크포인트 저장");
            _animator.Play("TurnOn", -1, 0f);
            _audioSource.Play();

        }
    }
    IEnumerator StartCoolTime()
    {
        _onCoolTime = true;
        yield return new WaitForSeconds(_coolTime);
        _onCoolTime = false;

    }
}

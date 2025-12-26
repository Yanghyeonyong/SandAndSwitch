using UnityEngine;

public class Gimmick_CheckPoint : Gimmick
{
    Animator _animator;
    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
    public override void StartGimmick()
    {
        if (GameManager.Instance.CheckPointData == null)
        {
            GameManager.Instance.CheckPointData = new CheckPointData();
        }
        GameManager.Instance.CheckPointData.Init();
        Debug.Log("체크포인트 저장");
        _animator.Play("TurnOn", -1, 0f);
    }
}

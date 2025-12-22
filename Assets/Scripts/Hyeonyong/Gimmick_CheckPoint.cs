using UnityEngine;

public class Gimmick_CheckPoint : Gimmick
{
    public override void StartGimmick()
    {
        GameManager.Instance.CheckPointData = null;
        GameManager.Instance.CheckPointData = new CheckPointData();
        Debug.Log("체크포인트 저장");
    }
}

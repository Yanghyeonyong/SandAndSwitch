using UnityEngine;

public class Gimmick_CheckPoint : Gimmick
{
    public override void StartGimmick()
    {
        if (GameManager.Instance.CheckPointData == null)
        {
            GameManager.Instance.CheckPointData = new CheckPointData();
        }
        GameManager.Instance.CheckPointData.Init();
        Debug.Log("체크포인트 저장");
    }
}

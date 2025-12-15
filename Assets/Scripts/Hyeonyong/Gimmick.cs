using UnityEngine;

public abstract class Gimmick : MonoBehaviour
{
    //해당 객체는 다형성을 위한 추상객체입니다
    //플레이어가 충돌중인 상태에서 E 버튼 클릭 시 메서드 실행
    [SerializeField] int _gimmickId=0;
    public int GimmickId => _gimmickId;

    private bool _isClear = false;
    public bool IsClear
    {
        get { return _isClear; }
        set { _isClear = value; }
    }
    public virtual void StartGimmick() { }
    public virtual void ExitGimmick() { }

    protected bool CheckClear()
    {
        //Debug.Log(GameManager_Hyeonyong.Instance.IsGimmickClear);
        if (GameManager_Hyeonyong.Instance.IsGimmickClear.ContainsKey(_gimmickId))
        {
            _isClear = GameManager_Hyeonyong.Instance.IsGimmickClear[_gimmickId];
            return _isClear;
        }
        else
        {
            GameManager_Hyeonyong.Instance.IsGimmickClear.Add(_gimmickId, _isClear);
            return _isClear;
        }
    }
}

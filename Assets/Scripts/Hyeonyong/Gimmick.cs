using UnityEngine;

public abstract class Gimmick : MonoBehaviour
{
    //해당 객체는 다형성을 위한 추상객체입니다
    //플레이어가 충돌중인 상태에서 E 버튼 클릭 시 메서드 실행
    [SerializeField] int _gimmickId=0;
    public int GimmickId => _gimmickId;

    protected bool _isClear = false;
    public bool IsClear
    {
        get { return _isClear; }
        set { _isClear = value; }
    }
    public virtual void StartGimmick() { }
    public virtual void ExitGimmick() { }

    protected bool CheckClear()
    {
        //Debug.Log(GameManager.Instance.IsGimmickClear);
        if (GameManager.Instance.IsGimmickClear.ContainsKey(_gimmickId))
        {
            _isClear = GameManager.Instance.IsGimmickClear[_gimmickId];
            return _isClear;
        }
        else
        {
            GameManager.Instance.IsGimmickClear.Add(_gimmickId, _isClear);
            return _isClear;
        }
    }

    protected ItemTransform CheckPos()
    {
        Debug.Log("불러오기");
        if (GameManager.Instance.GimmickPos.ContainsKey(_gimmickId))
        {
            Debug.Log(gameObject.name+" 저장된 값 반환 : "+ GameManager.Instance.GimmickPos[_gimmickId].position);

            return GameManager.Instance.GimmickPos[_gimmickId];
        }
        else
        {
            ItemTransform myTransform = new ItemTransform(this.transform.position, this.transform.rotation, this.transform.localScale);
            GameManager.Instance.GimmickPos.Add(_gimmickId, myTransform);
            Debug.Log(gameObject.name + " 저장된 값 없음");
            return null;
        }
    }

    public virtual void CheckNum(int num)
    { }

}

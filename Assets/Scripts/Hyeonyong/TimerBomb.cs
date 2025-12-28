using UnityEngine;

public class TimerBomb : Gimmick_Object
{
    [SerializeField] GameObject _bomb;
    [SerializeField] int _gimmickId = 0;
    public bool _isUse=false;
    private void Start()
    {
        if (GameManager.Instance.IsGimmickClear.ContainsKey(_gimmickId))
        {
            if (GameManager.Instance.IsGimmickClear[_gimmickId])
            {
                _bomb.SetActive(true);
                _isUse = true;
                return;
            }
        }
        _bomb.gameObject.SetActive(false);
    }
    public override void TurnOn()
    {
        if (_isUse)
            return;
        _bomb.SetActive(true);
        _isUse = true;
        GameManager.Instance.IsGimmickClear[_gimmickId] = true;
    }
}

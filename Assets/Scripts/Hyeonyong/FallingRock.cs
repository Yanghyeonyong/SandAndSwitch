using UnityEngine;

public class FallingRock : Gimmick_Object
{
    [SerializeField] GameObject _rock;
    [SerializeField] int _gimmickId = 0;
    public bool _isUse=false;
    private void Start()
    {
        if (GameManager.Instance.IsGimmickClear.ContainsKey(_gimmickId))
        {
            if (GameManager.Instance.IsGimmickClear[_gimmickId])
            {
                _isUse = true;
            }
        }
        _rock.gameObject.SetActive(false);
    }
    public override void TurnOn()
    {
        if (_isUse)
            return;
        _rock.SetActive(true);
        _isUse = true;
    }
}

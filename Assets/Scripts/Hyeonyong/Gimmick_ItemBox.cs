using System.Collections;
using UnityEngine;

public class Gimmick_ItemBox : Gimmick
{
    Animator _animator;
    ItemBox _itemBox;
    [SerializeField] GameObject _interactiveUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
        if (CheckClear())
        {
            _animator.SetTrigger("TurnOn");
        }
        _itemBox= GetComponent<ItemBox>();
    }

    public override void StartGimmick()
    {
        _animator.SetTrigger("TurnOn");
        _isClear = true;
        _interactiveUI.SetActive(false);
        GameManager.Instance.IsGimmickClear[GimmickId] = true;
        StartCoroutine(OpenBox());
    }
    IEnumerator OpenBox()
    {
        yield return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f;
        _itemBox.OpenBox();

    }
}

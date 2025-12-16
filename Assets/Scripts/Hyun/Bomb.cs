using UnityEngine;
using System;
using System.Collections;

public class Bomb : MonoBehaviour
{
    [SerializeField] private ItemData _itemData;


    //폭탄을 사용을 알리는 이벤트
    public static event Action OnUseBomb;
    public void UseBomb()
    {
        //조건
        StartCoroutine(ExplodeCoroutine());
        OnUseBomb?.Invoke();
    }
    private IEnumerator ExplodeCoroutine()//딜레이를 주기위한 코루틴
    {
        yield return new WaitForSeconds(_itemData.delay);
    }
    private void Explode()//폭발메서드
    {
        
    }
}

using UnityEngine;
using System;
using System.Collections;

public class Bomb : MonoBehaviour
{
    [SerializeField] private ItemData _itemData;
    [SerializeField] private Animator _animator;

    //폭탄을 사용을 알리는 이벤트
    public static event Action OnUseBomb;

    private void Start()
    {
        StartCoroutine(ExplodeCoroutine());
    }
    
    private IEnumerator ExplodeCoroutine()//딜레이를 주기위한 코루틴
    {

        yield return new WaitForSeconds(_itemData.delay);
        _animator.SetTrigger("Explosion");
        Explode();
        yield return new WaitForSeconds(0.5f);
        OnUseBomb?.Invoke();
        Destroy(gameObject);
    }
    private void Explode()//폭발메서드
    {
        
    }
}

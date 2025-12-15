using UnityEngine;
using System;

public class Bomb : MonoBehaviour
{
    [SerializeField] private ItemData _itemData;


    //폭탄을 사용을 알리는 이벤트
    public static event Action OnUseBomb;
    public void UseBomb()
    {
        //조건
        OnUseBomb?.Invoke();
    }
    private void Explode()
    {
        
    }
}

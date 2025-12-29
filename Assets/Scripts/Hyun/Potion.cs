using UnityEngine;
using System;

public class Potion : MonoBehaviour
{
    [SerializeField] private ItemData _itemData;
    //[SerializeField] private AudioSource _audio;//회복사운드를 따로 넣을것이라면..

    public static event Action<int> OnPotionUsed;//포션 사용을 알리는 이벤트

    public void UsePotion()
    {
        //GameManager.Instance.PlayerHeal(1);
        OnPotionUsed?.Invoke(1);//포션 사용 이벤트 발생
    }
}

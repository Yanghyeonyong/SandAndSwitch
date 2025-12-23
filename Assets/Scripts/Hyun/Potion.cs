using UnityEngine;
using System;

public class Potion : MonoBehaviour
{
    [SerializeField] private ItemData _itemData;
    //[SerializeField] private AudioSource _audio;//회복사운드를 따로 넣을것이라면..

    //포션 사용을 알리는 이벤트
    public static event Action OnUsePotion;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }
        
        OnUsePotion?.Invoke();//물약 사용을 알리는 이벤트 발생

        //회복효과 사운드를 넣을것이라면...
        //_audio.Play();

        Destroy(gameObject);
    }

}

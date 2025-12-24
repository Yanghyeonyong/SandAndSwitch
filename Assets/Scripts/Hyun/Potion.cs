using UnityEngine;
using System;

public class Potion : MonoBehaviour
{
    [SerializeField] private ItemData _itemData;
    //[SerializeField] private AudioSource _audio;//회복사운드를 따로 넣을것이라면..

    public void UsePotion()
    {
        GameManager.Instance.PlayerHeal(1);
    }
}

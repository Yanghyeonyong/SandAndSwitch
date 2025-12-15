using UnityEngine;
using System;

public class Potion : MonoBehaviour
{

    //읽기전용
    //포션 사용을 알리는 이벤트
    public static event Action OnUsePotion;
    public void UsePotion()
    {
        //조건
        OnUsePotion?.Invoke();

    }

}

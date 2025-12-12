using UnityEngine;
using UnityEngine.UI;

public class Bomb : MonoBehaviour
{
    [SerializeField] private Image _image;//이미지 UI에서 할것인지?...
    [SerializeField] private float _radius;//범위
    [SerializeField] private float _damage;//데미지 있게 만들것인지 없게 만들것인지?...



    private void OnTriggerEnter2D(Collider2D collision)
    {
        //특정지역에서 사용 가능하도록 충돌처리
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //특정지역밖에선 사용 불가능하도록 처리
    }
}

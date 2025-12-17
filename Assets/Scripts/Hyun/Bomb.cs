using UnityEngine;
using System;
using System.Collections;

public class Bomb : MonoBehaviour
{
    [SerializeField] private ItemData _itemData;
    [SerializeField] private Animator _animator;//폭발트리거용
    [SerializeField] private AudioSource _audio;//폭발사운드용

    private float _baseExplosionRadius = 1f;//스케일 1일때 폭발 반경

    //폭탄을 사용을 알리는 이벤트
    public static event Action OnUseBomb;

    private void Awake()
    {
        if (_audio == null)
        {
            _audio = GetComponent<AudioSource>();
        }
    }

    public void UseBomb()
    {
        StartCoroutine(ExplodeCoroutine());
    }

    private IEnumerator ExplodeCoroutine()//딜레이를 주기위한 코루틴
    {

        yield return new WaitForSeconds(_itemData.delay);
        _animator.SetTrigger("Explosion");
        Explode();
        yield return new WaitForSeconds(0.6f);
        OnUseBomb?.Invoke();
        Destroy(gameObject);
    }
    private void Explode()//폭발메서드
    {
        //설정된 폭발 범위에 따라 폭발 애니메이션 스케일 증가
        float explosionScale = _itemData.radius / _baseExplosionRadius;//설정된 폭발 범위 / 스케일
        transform.localScale = Vector3.one * explosionScale;
        //폭발 판정 확인(설치범위와 폭발 범위가 달라서 판정이 필요하다면 추가, 확정으로 설치만하면 파괴될것이라면 없어도됨)
        //폭탄 위치 기준, 범위(반지름), 대상
        Collider[] bombHits = Physics.OverlapSphere(transform.position, _itemData.radius, _itemData.targetLayer);
        _audio.Play();
        foreach (Collider hit in bombHits)
        {
            //지형파괴

        }
    }
}

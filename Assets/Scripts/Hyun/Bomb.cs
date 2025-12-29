using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Bomb : MonoBehaviour
{
    [SerializeField] private ItemData _itemData;
    [SerializeField] private Animator _animator;//폭발트리거용
    [SerializeField] private AudioSource _audio;//폭발사운드용

    private float _baseExplosionRadius = 1f;//스케일 1일때 폭발 반경
    public bool IsExploding { get; private set; } = false;
    public bool IsThrownBomb { get; set; } = false;
    private void Awake()
    {
        if (IsThrownBomb)
        {
            var pickup = GetComponent<ItemPickup>();
            if (pickup != null)
            {
                pickup.enabled = false;
            }
        }
           
    }
    public void UseBomb()
    {
        IsThrownBomb = true;
        var pickup = GetComponent<ItemPickup>();
        if (pickup != null)
        {
            pickup.enabled = false;
        }
            StartCoroutine(ExplodeCoroutine());
    }

    private IEnumerator ExplodeCoroutine()//딜레이를 주기위한 코루틴
    {
        yield return new WaitForSeconds(_itemData.delay);
        _animator.SetTrigger("Explosion");
        Explode();
        yield return new WaitForSeconds(0.6f);
        Destroy(gameObject);
    }
    private void Explode()//폭발메서드
    {
        //설정된 폭발 범위에 따라 폭발 애니메이션 스케일 증가
        float explosionScale = _itemData.radius / _baseExplosionRadius;//설정된 폭발 범위 / 스케일
        transform.localScale = Vector3.one * explosionScale;

        _audio.Play();
        BreakRadius();
    }

    private void BreakRadius()
    {
        Vector3 origin = transform.position;
        float radius = _itemData.radius;

        #region
        //foreach (var mapCollider in Physics2D.OverlapCircleAll(origin, radius, _itemData.targetLayer))
        //{
        //    Tilemap map = mapCollider.GetComponent<Tilemap>();
        //    if (map == null) continue;

        //    Vector3Int centerCell = map.WorldToCell(origin);

        //    int cellRadius = Mathf.CeilToInt(radius);

        //    for (int x = -cellRadius; x <= cellRadius; x++)
        //    {
        //        for (int y = -cellRadius; y <= cellRadius; y++)
        //        {
        //            Vector3Int cell = new Vector3Int(centerCell.x + x, centerCell.y + y, 0);

        //            Vector3 worldPos = map.GetCellCenterWorld(cell);

        //            if (Vector2.Distance(origin, worldPos) <= radius)
        //            {
        //                map.SetTile(cell, null);
        //            }
        //        }
        //    }
        //}
        #endregion

        // 타일맵 탐색
        Collider2D[] maps = Physics2D.OverlapCircleAll(origin, radius, _itemData.targetLayer);

        foreach (var col in maps)
        {
            Tilemap map = col.GetComponent<Tilemap>();
            if (map == null)
            {
                //범위에 휘말린 오브젝트도 삭제
                CircleCollider2D circle = col.GetComponent<CircleCollider2D>();
                if (circle == null)
                {
                    continue;
                }
                col.gameObject.SetActive(false);
                continue;
            }

            // 폭발 범위 내부의 타일만 제거
            BoundsInt bounds = map.cellBounds;

            for (int x = bounds.xMin; x <= bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y <= bounds.yMax; y++)
                {
                    Vector3Int cell = new Vector3Int(x, y, 0);
                    Vector3 world = map.GetCellCenterWorld(cell);

                    if (Vector2.Distance(origin, world) <= radius)
                    {
                        map.SetTile(cell, null);
                    }
                }
            }
        }
    }
}

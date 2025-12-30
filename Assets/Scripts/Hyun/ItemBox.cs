using System.Collections;
using System.Xml;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    [SerializeField] private GameObject itemPickupPrefab; //드랍될 아이템 프리팹
    [SerializeField] float _delay = 1f;
    GameObject _item;


    private bool _isOpened = false;


    public bool IsOpened => _isOpened;


    public void OpenBox()
    {

        if (_isOpened)
        {
            return;
        }
        _isOpened = true;
        _item = Instantiate(itemPickupPrefab, transform.position, Quaternion.identity);
        _item.GetComponent<CircleCollider2D>().enabled = false;
        StartCoroutine(ItemUsable());
    }

    IEnumerator ItemUsable()
    {
        yield return new WaitForSeconds(_delay);
        _item.GetComponent<CircleCollider2D>().enabled = true;
    }

}

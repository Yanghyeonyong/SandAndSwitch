using System.Collections;
using System.Xml;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    [SerializeField] private GameObject itemPickupPrefab; //드랍될 아이템 프리팹
    GameObject _item;
    [SerializeField] private string _uniqueID;

    private bool _isOpened = false;

    [SerializeField] float _delay = 1f;
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this))
        {
            _uniqueID = string.Empty;
            return;
        }

        if (string.IsNullOrEmpty(_uniqueID))
        {
            _uniqueID = System.Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif



    public void OpenBox()
    {
        //if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this))
        //{
        //    _uniqueID = string.Empty;
        //    return;
        //}

        //if (string.IsNullOrEmpty(_uniqueID))
        //{
        //    _uniqueID = System.Guid.NewGuid().ToString();
        //    UnityEditor.EditorUtility.SetDirty(this);
        //}

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

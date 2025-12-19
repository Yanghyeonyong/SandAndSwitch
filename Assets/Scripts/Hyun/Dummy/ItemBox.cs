using System.Xml;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    [SerializeField] private GameObject itemPickupPrefab; //드랍될 아이템 프리팹
    [SerializeField] private string _uniqueID;

    private bool _isOpened = false;

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
        if (_isOpened)
        {
            return;
        }
        _isOpened = true;
        Instantiate(itemPickupPrefab, transform.position, Quaternion.identity);
    }

}

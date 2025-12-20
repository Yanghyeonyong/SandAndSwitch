using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableTestImage : MonoBehaviour
{
    [SerializeField] AssetReferenceSprite _testSprite;

    Image _imageComponent;
    AsyncOperationHandle<Sprite> _handle;

    void Awake()
    {
        Addressables.InitializeAsync();

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _handle = _testSprite.LoadAssetAsync<Sprite>();
        _handle.Completed += handle =>
        {
            _imageComponent = GetComponent<Image>();
            _imageComponent.sprite = handle.Result;
        };

        //_testSprite.LoadAssetAsync<Sprite>().Completed += handle =>
        //{
        //    _iamgeComponent = GetComponent<Image>();
        //    _iamgeComponent.sprite = handle.Result;
        //};
    }

    //void PutAssetInImage(AsyncOperation)

    // Update is called once per frame
    void Update()
    {
        
    }
}

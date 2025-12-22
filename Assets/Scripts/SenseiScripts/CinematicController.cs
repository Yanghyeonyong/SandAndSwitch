using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.ResourceManagement.AsyncOperations;
//using System.Threading.Tasks;
using UnityEngine.Playables;
using UnityEngine.UI;

public class CinematicController : MonoBehaviour
{
    //temporary image refernces
    [SerializeField] List<Sprite> _tempIntroSprites = new List<Sprite>();
    [SerializeField] List<Sprite> _tempVictorySprites = new List<Sprite>();


    PlayableDirector _playableDirector;
    //List<AsyncOperationHandle> 
    //List<AsyncOperationHandle<Sprite>> _introStills = new List<AsyncOperationHandle<Sprite>>();
    //[SerializeField]
    //List<AssetReferenceSprite> _introStills;
    //[SerializeField]
    //List<AssetReferenceSprite> _victoryStills;
    //List<AsyncOperationHandle<Sprite>> _victoryStills = new List<AsyncOperationHandle<Sprite>>();

    //image refernce
    List<Sprite> _introSprites = new List<Sprite>();
    List<Sprite> _victorySprites = new List<Sprite>();


    //canvas
    GameObject _cutsceneCanvas;
    //image
    Image _custceneStill;
    bool _addressablesReady = false;
    static bool _initialized = false;
    private void Awake()
    {
        Addressables.InitializeAsync();

        _playableDirector = GetComponent<PlayableDirector>();
        //PlayIntroStills();

        _cutsceneCanvas = transform.GetChild(0).gameObject;
        if (_cutsceneCanvas.activeSelf)
        {
            _cutsceneCanvas.SetActive(false);
        }
        _custceneStill = _cutsceneCanvas.transform.GetChild(0).GetComponent<Image>();


        _waitForSeconds = new WaitForSecondsRealtime(_waitTime);
        
        
        //Addressables.InitializeAsync();
        
    }


    void Start()
    {

        GameManager.Instance.CinematicControllerSensei = this;
    }


    public void StartTimeline()
    {
        //_cutsceneCanvas.SetActive(true);
        InCutscene = true;
        _playableDirector.Play();
        Time.timeScale = 0f;
    }

    public bool InCutscene { get; private set; }

    public void PlayCutscene()
    {

        switch(GameManager.Instance.CurrentCutsceneIndex++)
        {
            
            case 0:
                InCutscene = true;
                PlayIntroStills();
                break;
            case 1:
                InCutscene = true;
                PlayVictoryStills();
                break;


        }


    }
    Coroutine _currentCoroutine;

    int _currentExecutionIndex = 0;

    private void PlayIntroStills()
    {
        Debug.Log($"[Addressables] Looking for stills at: {Addressables.RuntimePath}");
        string actualStreamingPath = Application.streamingAssetsPath;
        Debug.Log($"[System] Physical StreamingAssets Path: {actualStreamingPath}");
        Debug.Log($"[Addressables] RuntimePath variable: {Addressables.RuntimePath}");
        //_introStills.Clear();
        //_introSprites.Clear();

        //var handle = Addressables.LoadAssetAsync<UnityEngine.Object>(_introStills[0].RuntimeKey);
        //await handle.Task;
        //Debug.Log($"Loaded type: {handle.Result.GetType()}");

        
            int myId = ++_currentExecutionIndex;
            //var handle0 = _introStills[0].LoadAssetAsync<Sprite>();
            //var handle1 = _introStills[1].LoadAssetAsync<Sprite>();



            //_introStills.Add(Addressables.LoadAssetAsync<Sprite>("2.1.2[2.1.2]"));
            //_introStills.Add(Addressables.LoadAssetAsync<Sprite>("2.1.3[2.1.3]"));

            if (myId != _currentExecutionIndex)
            {
                // A newer execution has started, abort this one
                return;
            }

            //_introSprites.Add(await handle0.Task);

            //_introSprites.Add(await handle1.Task);

            //Debug.Log(handle0.Status);
            //Debug.Log(handle1.Status);

            //_currentCoroutine = StartCoroutine(PlayStills(_introSprites));
            _currentCoroutine = StartCoroutine(PlayStills(_tempIntroSprites));


    }

    public void PreloadVictoryBackground()
    {
        _cutsceneCanvas.SetActive(true);
        _custceneStill.sprite = _tempVictorySprites[0];
    }

    

    public void ResetPlayableDirector()
    {
        _currentExecutionIndex++;
        if (InCutscene)
        {
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
                _currentCoroutine = null;
            }
            _playableDirector.Stop();
            _playableDirector.time = 0;
            _cutsceneCanvas.SetActive(false);
        }
        ClearCutscene();

        InCutscene = false;
    }



    private void PlayVictoryStills()
    {
        //_custceneStills.Clear();
        _victorySprites.Clear();
        //_victoryStills.Clear();

        
            int myId = ++_currentExecutionIndex;
            //var handle0 = _victoryStills[0].LoadAssetAsync<Sprite>();
            //var handle1 = _victoryStills[1].LoadAssetAsync<Sprite>();
            //var handle2 = _victoryStills[2].LoadAssetAsync<Sprite>();
            //var handle3 = _victoryStills[3].LoadAssetAsync<Sprite>();
            //_victoryStills.Add(Addressables.LoadAssetAsync<Sprite>("2.4.4[2.4.4]"));
            //_victoryStills.Add(Addressables.LoadAssetAsync<Sprite>("2.4.5[2.4.5]"));
            //_victoryStills.Add(Addressables.LoadAssetAsync<Sprite>("2.4.6[2.4.6]"));
            //_victoryStills.Add(Addressables.LoadAssetAsync<Sprite>("2.4.7[2.4.7]"));
            if (myId != _currentExecutionIndex)
            {
                // A newer execution has started, abort this one
                return;
            }


            //_victorySprites.Add(await handle0.Task);
            //_victorySprites.Add(await handle1.Task);
            //_victorySprites.Add(await handle2.Task);
            //_victorySprites.Add(await handle3.Task);
            //_victorySprites.Add(await _victoryStills[0].Task);
            //_victorySprites.Add(await _victoryStills[1].Task);
            //_victorySprites.Add(await _victoryStills[2].Task);
            //_victorySprites.Add(await _victoryStills[3].Task);


            //_currentCoroutine = StartCoroutine(PlayStills(_victorySprites));
            _currentCoroutine = StartCoroutine(PlayStills(_tempVictorySprites));

    }

    [SerializeField] private float _waitTime = 3f;
    WaitForSecondsRealtime _waitForSeconds;

    IEnumerator PlayStills (List<Sprite> still)
    {
        int coroutineExecutionId = _currentExecutionIndex;
        _cutsceneCanvas.SetActive(true);
        foreach (var slide in still)
        {

            if (coroutineExecutionId != _currentExecutionIndex)
            {
                // A newer execution has started, abort this one
                yield break;
            }
            _custceneStill.sprite = slide;
            yield return _waitForSeconds; // Display each still for 3 seconds
        }
        
        if (coroutineExecutionId != _currentExecutionIndex)
        {
            // A newer execution has started, abort this one
            yield break;
        }

        yield return null;
        _cutsceneCanvas.SetActive(false);
        //ClearCutscene();
        InCutscene = false;
        Time.timeScale = 1f;

    }


    public void ClearCutscene()
    {
        //foreach (var handle in _introStills)
        //{
        //    if (handle.IsValid())
        //    {
        //       handle.ReleaseAsset();
        //    }
        //}
        ////_introStills.Clear();
        //_introSprites.Clear();
        //foreach (var handle in _victoryStills)
        //{
        //    if (handle.IsValid())
        //    {
        //        handle.ReleaseAsset();
        //    }
        //}
        ////_victoryStills.Clear();
        //_victorySprites.Clear();


        _currentCoroutine = null;

        InCutscene = false;
    }




}

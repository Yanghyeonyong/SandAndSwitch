//using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;


#if UNITY_EDITOR
//using UnityEngine.AddressableAssets;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif


public enum GameState
{
    PhaseOne,
    PhaseTwo
}

public enum Language { KR, EN }
public class GameManager : Singleton<GameManager>
{
    //수집품 변수
    public CollectSlotController _collectSlotContoller;
    [SerializeField] public int TotalCollectibleCount { get; private set; } = 10;
    public Image CollectibleIcon { get; set; }
    public TextMeshProUGUI CollectibleCountText { get; set; }


    //아이템 로그 변수
    public List<ItemLogLogScript> ItemLogs { get; set; } = new List<ItemLogLogScript>();
    public ItemLogCanvasScript ItemLogCanvas { get; set; }


    //busy loading game scene flag
    bool _isLoadingGameScene = false;

    //cutscene info
    public int CurrentCutsceneIndex = 0;
    public CinematicController CinematicControllerSensei { get; set; }

    //addresables references
    ItemData _bombScriptableObject;
    SpriteRenderer _bombPrefabImage;


    [Header("플레이어 체력 관련")]
    [SerializeField] private int _maxPlayerHealth = 3;
    [SerializeField] private int _currentPlayerHealth = 1;

    [Header("Settings")]
    public Language currentLanguage = Language.KR;

    public int CurrentPlayerHealth
    {
        get
        {
            return _currentPlayerHealth;
        }
        set
        {
            _currentPlayerHealth = value;
        }
    }
    public int MaxPlayerHealth => _maxPlayerHealth;

    public List<Image> HeartImages = new List<Image>();

    [SerializeField] string _heartEmptyHexColor = "000000";
    [SerializeField] string _heartFullHexColor = "FF0000";

    private Color _heartFullColor;
    private Color _heartEmptyColor;

    //아이템테이블,스트링테이블(주민규)
    public Table<int, ItemTableData> ItemTable { get; private set; }
    public Table<string, StringTableData> StringTable { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        //미리 컬렉트슬롯 캐싱 251226
        _collectSlotContoller = GetComponent<CollectSlotController>();
        LoadTables();      // 1) 파싱
        ResolveTableAssets(); // 2) 프리팹/아이콘 실제 로드

#if UNITY_EDITOR
        PutParsingResultsInScriptableObjects(); // 3) Addressables에 파싱 결과 넣기
#endif
    }

    //컬렉션컨트롤러 관련 이벤트 251226
    private void OnEnable()//이벤트 구독
    {
        _collectSlotContoller.OnCollectChanged += OnCollectSlotUpdated;
    }
    private void OnDisable()//이벤트 해제
    {
        _collectSlotContoller.OnCollectChanged -= OnCollectSlotUpdated;
    }

    //컬렉션컨트롤러의 이벤트를 받고 사용할 메서드 251226
    private void OnCollectSlotUpdated(CollectSlot slot)
    {
        ItemLogCanvas.PickupOrUseLogic(slot.Data, 1);

        CollectibleCountText.text = slot.Count + "/" + TotalCollectibleCount;

        if (CollectibleIcon.color.a != 1f)
        {
            CollectibleIcon.color = new Color(1f, 1f, 1f, 1f);
        }

        // 기존 GameManager UI 갱신 함수 사용
        UpdateCollectSlot(slot);
    }
#if UNITY_EDITOR
    public string GetAssociatedDescriptionFromImage(GameObject objectToCompare)

    {
        foreach (var entry in ItemTable.Data.Values)
        {


            if (entry.Icon == objectToCompare.GetComponent<SpriteRenderer>().sprite.ToString())
            {
                //return entry.ScriptableObject;
            }

        }

        return null;

    }

    void PutParsingResultsInScriptableObjects()
    {






        var settings = AddressableAssetSettingsDefaultObject.Settings;
        var group = settings.DefaultGroup;

        //사실 addressable 사용안해도됫엇다 허걱스

        //폭탄관련 데이터 스크립터블오브젝트에 프리팹, 아이콘 적용
        var bombScriptableObjectEntry = GetEntryFromAddressableGroup(group, "BombScriptableObject");
        var bombPrefabEntry = GetEntryFromAddressableGroup(group, "BombPrefab");

        _bombScriptableObject = AssetDatabase.LoadAssetAtPath<ItemData>(bombScriptableObjectEntry.AssetPath);
        _bombPrefabImage = AssetDatabase.LoadAssetAtPath<GameObject>(bombPrefabEntry.AssetPath).GetComponent<SpriteRenderer>();

        _bombScriptableObject.prefab = AssetDatabase.LoadAssetAtPath<GameObject>(ItemTable[101].Prefab);
        _bombScriptableObject.icon = AssetDatabase.LoadAssetAtPath<Sprite>(ItemTable[101].Icon);
        _bombPrefabImage.sprite = _bombScriptableObject.icon; //어차피 변경되어 

        //string test = StringTable["Test"].kr;


        EditorUtility.SetDirty(_bombScriptableObject);
        EditorUtility.SetDirty(_bombPrefabImage);
        AssetDatabase.SaveAssets();




    }

    AddressableAssetEntry GetEntryFromAddressableGroup(AddressableAssetGroup group, string targetAddress)
    {
        foreach (var entry in group.entries)
        {
            if (entry.address == targetAddress)
            {
                return entry;
            }

        }
        return null;
    }



#endif
    void LoadTables()
    {
        TextAsset itemCsv = Resources.Load<TextAsset>("Data/ItemTable"); // Resources/Data/ItemTable.csv
        ItemTable = TableParser.Parse<int, ItemTableData>(itemCsv, "ID");

        TextAsset stringCsv = Resources.Load<TextAsset>("Data/StringTable");
        StringTable = TableParser.Parse<string, StringTableData>(stringCsv, "key");
    }

    void ResolveTableAssets()
    {
        // 아래 foreach는 너 Table 구현에 맞춰서 "모든 row를 도는 방법"만 맞춰야 해.
        foreach (var row in ItemTable.Data.Values) // 예: Rows가 Dictionary라면
        {
            if (!string.IsNullOrEmpty(row.Prefab))
                row.PrefabObject = Resources.Load<GameObject>(row.Prefab);

            if (!string.IsNullOrEmpty(row.Icon))
                row.IconSprite = Resources.Load<Sprite>(row.Icon);
            if (!string.IsNullOrEmpty(row.Sound))
                row.SoundEffect = Resources.Load<AudioClip>(row.Sound);
        }
    }

    //UI 관련
    /*
    Canvas는 인덱스 순으로 MainMenu, IngameUI, PauseMenu, GameOverMenu
    버튼 동작은 UIManager에서 처리하되 그 함수를 GameManager에서 호출할 수 있게 만들어놨다
    버튼 순서는 각 와이어프레임에서 좌에서 우 혹은 위에서 아래 순이다

    MenuButton: Start, ControlGuide, Exit
    IngameButton: Pause
    PauseMenuButton: Restart, MainMenu, Resume, ControlGuide
    GameOverMenuButton: Restart, MainMenu

    */
    //251216 - 양현용 추가 " 기믹 확인 용도
    //이후엔 실제 퀵슬롯에서 원하는 타입의 아이템이 있는지를 체크할 예정

    public QuickSlot[] GameManagerQuickSlots { get; set; } = new QuickSlot[10];
    public Image[] GameManagerQuickSlotsImages { get; set; } = new Image[10];
    public TextMeshProUGUI[] GameManagerQuickSlotCountTexts { get; set; } = new TextMeshProUGUI[10];

    //민규님을 위한 예시

    //void Start()
    //{
    //    GameManagerQuickSlotCountTexts[0].text = ItemTable[101].Name;

    //}


    //퀵슬롯 UI 업데이트
    public void QuickSlotUIUpdate(int currentSelectedQuickslot)
    {
        foreach (var quickslottemp in GameManagerQuickSlotsImages)
        {
            if (quickslottemp != GameManagerQuickSlotsImages[currentSelectedQuickslot])
            {
                quickslottemp.gameObject.SetActive(false);
            }
            else
            {
                if (quickslottemp.gameObject.activeSelf == false)
                {
                    quickslottemp.gameObject.SetActive(true);
                }

            }
        }

    }
    //퀵슬롯컨트롤러 관련 UI 메서드로 따로 뺀 부분
    private readonly Color clear = new Color(1, 1, 1, 0);
    public void UpdateQuickSlot(int index, QuickSlot slot)
    {
        if (slot.IsEmpty)
        {
            GameManagerQuickSlotCountTexts[index].text = "";
            GameManagerQuickSlotIcons[index].sprite = null;
            GameManagerQuickSlotIcons[index].color = clear;
            GameManagerQuickSlotIcons[index].gameObject.SetActive(false);
            return;
        }

        GameManagerQuickSlotCountTexts[index].text = slot.Count.ToString();
        GameManagerQuickSlotIcons[index].sprite = slot.Data.icon;
        GameManagerQuickSlotIcons[index].color = Color.white;
        GameManagerQuickSlotIcons[index].gameObject.SetActive(true);
    }

    public void UpdateCollectSlot(CollectSlot slot)
    {
        if (slot.IsEmpty)
        {
            Debug.Log("빈 업데이트 수집품 슬롯 호출");
            CollectibleIcon.color = new Color(1f, 1f, 1f, 0.2f);
            CollectibleCountText.text = "0/" + TotalCollectibleCount;
            return;
        }
        else
        {
            Debug.Log("업데이트 수집품 슬롯 호출" + slot.Count.ToString());
            CollectibleIcon.color = new Color(1f, 1f, 1f, 1f);
            CollectibleCountText.text = slot.Count.ToString()+"/"+ TotalCollectibleCount;
        }
                
    }

    public Image[] GameManagerQuickSlotIcons { get; set; } = new Image[10];

    //아이템픽업 관련
    public List<Vector3> CollectedItemIDs = new List<Vector3>();


    public void RefreshAllQuickSlotUI()
    {
        for (int i = 0; i < GameManagerQuickSlots.Length; i++)
        {
            UpdateQuickSlot(i, GameManagerQuickSlots[i]);
        }

        QuickSlotUIUpdate(0);
    }


    [SerializeField] bool _checkItem;
    public bool CheckItem
    {
        get { return _checkItem; }
        set { _checkItem = value; }
    }
    [SerializeField] bool _onProgressGimmick = false;
    Player player;
    public Player Player
    {
        get
        {
            return player;
        }
        set
        {
            player = value;
        }
    }
    //public Player Player => player;
    public bool OnProgressGimmick
    {
        get { return _onProgressGimmick; }
        set { _onProgressGimmick = value; }
    }

    [SerializeField] private Dictionary<int, bool> _isGimmickClear = new Dictionary<int, bool>();
    //251221 - 양현용 추가 : 기믹 위치 저장 ex) 박스
    public Dictionary<int, bool> IsGimmickClear
    {
        get { return _isGimmickClear; }
        set
        {
            _isGimmickClear = value;
        }
    }
    [SerializeField] private Dictionary<int, ItemTransform> _gimmickPos = new Dictionary<int, ItemTransform>();
    public Dictionary<int, ItemTransform> GimmickPos
    {
        get { return _gimmickPos; }
        set
        {
            _gimmickPos = value;
        }
    }

    [SerializeField] int _curScene = 0;
    public int CurScene
    {
        get { return _curScene; }
        set { _curScene = value; }
    }
    //251222 - 양현용 추가 : 플레이어 선택지가 활성화되어 있는지 체크
    [SerializeField] bool onSelection = false;
    public bool OnSelection
    {
        get { return onSelection; }
        set { onSelection = value; }
    }
    //

    private CheckPointData _checkPointData;
    public CheckPointData CheckPointData
    {
        get { return _checkPointData; }
        set { _checkPointData = value; }
    }



    private GameState _gameState = GameState.PhaseOne;

    public GameState CurrentGameState
    {
        get { return _gameState; }
        private set { _gameState = value; }
    }

    [SerializeField] GameObject _playerPrefab;
    public GameObject _player;

    //0. 튜토리얼 시작 위치 1. 맵 2시작 위치 2. 맵 4 시작 위치 3. 맵2 복귀 위치
    [SerializeField] Vector3[] _playerSpawnPos;

    [SerializeField] float _gameOverCount = 60f;
    [SerializeField] float _curGameOverCount;
    [SerializeField] float _minusGameOverCount = 0.1f;
    WaitForSeconds _wait;
    private Coroutine _gameOverCoroutine;
    public Coroutine GameOverCoroutine
    {
        get
        {
            return _gameOverCoroutine;
        }
        set
        {
            _gameOverCoroutine = value;
        }
    }

    //BGM 목록
    // 0 타이틀 1. 튜토리얼 2. 스테이지 2 3. 스테이지 3 4. 스테이지 4 5. 페이즈 2 6. 사망 7. 승리
    //0. 튜토리얼 1. 스테이지 2 :Phase 1 2. 스테이지 4 3. 스테이지 2 : Phase 2
    [SerializeField] AudioClip[] _bgms;

    void Start()
    {
        //251216 - 양현용 추가 : 테스트용 플레이어 스크립트를 찾는 용도
        //해당 값을 찾는 기능은 현재 테스트 용으로  start에 있으나, 이후 플레이어 스폰 지점으로 이동 예정
        //player = GameObject.FindFirstObjectByType<Player>().GetComponent<Player>();
        _wait = new WaitForSeconds(_minusGameOverCount);
        _waitRealTime = new WaitForSecondsRealtime(_minusGameOverCount);
        ColorUtility.TryParseHtmlString("#" + _heartFullHexColor, out _heartFullColor);
        ColorUtility.TryParseHtmlString("#" + _heartEmptyHexColor, out _heartEmptyColor);

        _collectSlotContoller =GetComponent<CollectSlotController>();
        //addressable 로딩

        //PutParsingResultsInScriptableObjects();
        SoundEffectManager.Instance.PlayBGM(_bgms[_curScene]);

    }
   


    Coroutine _phaseTwoFadeCortouine;


    public void EnterPhaseTwo()
    {
        //페이즈 2 bgm은 끝에서 3번째
        SoundEffectManager.Instance.PlayBGM(_bgms[_bgms.Length - 3]);
        _gameState = (GameState)1;

        //플레이어의 Phase 2 대사 시작(주민규)
        if (Player != null)
        {
            Player.StartCoroutine(Player.PlayPhaseTwoDialogueSequence());
        }
        //기존 로직
        if (_gameOverCoroutine == null)
        {
            _gameOverCoroutine = StartCoroutine(CheckGameOver());
        }
    }
    public void EnterPhaseOne()
    {
        _gameState = (GameState)0;
        _checkItem = false;
        //SoundEffectManager.Instance.PlayBGM(_bgms[CurScene]);
        if (_gameOverCoroutine != null)
        {
            StopCoroutine(_gameOverCoroutine);
            _gameOverCoroutine = null;
        }
    }

    public int GetCurrentSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }


    //251216 - 양현용 추가 : 다음 씬, 이전 씬으로 넘어가는 용도
    public void LoadNextScene()
    {
        player = null;
        _curScene++;
        SceneManager.LoadScene(_curScene);

        if (_curScene >= 1 && _curScene <= 3)
        {
            //_player = Instantiate(_playerPrefab, _playerSpawnPos[_curScene-1], Quaternion.identity);
            //player = _player.GetComponent<Player>();
            StartCoroutine(SpawnPlayer());
        }
    }

    //25122 - 양현용 추가 : 원하는 씬으로 넘어가는 용도
    public void LoadIndexScene(int index)
    {
        player = null;
        SceneManager.LoadScene(index);

        if (_curScene >= 1 && _curScene <= 3)
        {
            //_player = Instantiate(_playerPrefab, _playerSpawnPos[_curScene-1], Quaternion.identity);
            //player = _player.GetComponent<Player>();
            StartCoroutine(SpawnPlayer_CheckPoint());
        }
    }

    [Header("페이즌2 화면 관련")]
    [SerializeField] Color _phaseTwoScreenColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    [SerializeField] float _maxAlpha = 0.8f;

    IEnumerator CheckGameOver()
    {
        //페이즈 하얘지는 연출 추가 251221 최정욱
        Color _currentWhiteFadeColor = _phaseTwoScreenColor;

        _curGameOverCount = _gameOverCount;
        _curGameOverCount -= _minusGameOverCount;
        while (_curGameOverCount > 0)
        {
            //페이즈 하얘지는 연출 추가 251221 최정욱

            if (_curGameOverCount / _gameOverCount >= 1 - _maxAlpha)
            {
                _currentWhiteFadeColor.a = _maxAlpha * (1f - (_curGameOverCount / _gameOverCount));
            }
            else
            {
                _currentWhiteFadeColor.a = _maxAlpha;
            }
            //_currentWhiteFadeColor.a = 1f - (_curGameOverCount / _gameOverCount);
            ExtraUITools[0].GetComponent<Image>().color = _currentWhiteFadeColor;
            yield return _wait;
            _curGameOverCount -= _minusGameOverCount;
        }
        PlayerDeath();
    }

    IEnumerator SpawnPlayer()
    {
        yield return GameSceneLoadAsyncOperation.isDone;
        yield return null;
        if (_gameState == GameState.PhaseOne)
        {
            SoundEffectManager.Instance.PlayBGM(_bgms[_curScene]);
        }
        _player = Instantiate(_playerPrefab, _playerSpawnPos[_curScene - 1], Quaternion.identity);
        player = _player.GetComponent<Player>();
        GameManager.Instance.RefreshAllQuickSlotUI();
    }
    IEnumerator SpawnPlayer_CheckPoint()
    {
        //_collectSlotContoller.Slots.Clear();
        _collectSlotContoller.CollectSlotClear();
        //수집품 상태 저장
        foreach (var d in _checkPointData._collectSlots)
        {
            Debug.Log("업데이트 : " + d.Value.Data.id);
            ItemData data = _checkPointData.FindItem(d.Value.Data.id);
            CollectSlot collect = new CollectSlot();
            collect.Init(data, d.Value.Count);
            if (_collectSlotContoller.Slots.ContainsKey(d.Key))
            {
                _collectSlotContoller.Slots[d.Key].Count = _checkPointData._collectSlots[d.Key].Count;
            }
            else
            {
                _collectSlotContoller.Slots.Add(d.Key, collect);
            }
            UpdateCollectSlot(_collectSlotContoller.Slots[d.Key]);
        }
        //Debug.Log("업데이트 : " + _collectSlotContoller.Slots);
        UpdateCollectSlot(_collectSlotContoller.Slots[401]);
        yield return GameSceneLoadAsyncOperation.isDone;
        yield return null;
        if (_gameState == GameState.PhaseOne)
        {
            SoundEffectManager.Instance.PlayBGM(_bgms[_curScene]);
        }
        _player = Instantiate(_playerPrefab, _checkPointData._playerPos, _checkPointData._playerRot);
        player = _player.GetComponent<Player>();
        GameManager.Instance.RefreshAllQuickSlotUI();
        if (_checkPointData == null)
        {
            Debug.Log("체크포인트가 없다");
        }
        Debug.Log("체크포인트 위치 정보 : " + _checkPointData._playerPos);

        for (int i = 0; i < GameManagerQuickSlots.Length; i++)
        {
            if (_checkPointData.GameManagerQuickSlots[i].Data != null)
            {
                ItemData data = _checkPointData.FindItem(_checkPointData.GameManagerQuickSlots[i].Data.id);
                GameManagerQuickSlots[i].Init(data, _checkPointData.GameManagerQuickSlots[i].Count);
                UpdateQuickSlot(i, _checkPointData.GameManagerQuickSlots[i]);
            }
        }


        CollectedItemIDs.Clear();
        foreach (Vector3 pos in _checkPointData.CollectedItemIDs)
        {
            CollectedItemIDs.Add(pos);
        }


    }
    IEnumerator SpawnPlayer_Prev(int spawnPos)
    {
        yield return GameSceneLoadAsyncOperation.isDone;
        yield return null;
        if (!_checkItem)
        {
            SoundEffectManager.Instance.PlayBGM(_bgms[_curScene]);
        }
        _player = Instantiate(_playerPrefab, _playerSpawnPos[spawnPos], Quaternion.identity);
        player = _player.GetComponent<Player>();
    }
    public void LoadPrevScene()
    {
        player = null;
        _curScene--;
        SceneManager.LoadScene(_curScene);
        //현재 페이즈가 전환되는 시점의 씬 넘버가 2라서 이와 같이 적용
        if (_curScene == 2)
        {
            //_player = Instantiate(_playerPrefab, _playerSpawnPos[3], Quaternion.identity);
            //player = _player.GetComponent<Player>();

            //테스트용으로 남는 bool값 아이템 보유 여부로 사용


            StartCoroutine(SpawnPlayer_Prev(3));
        }
    }

    public AsyncOperation GameSceneLoadAsyncOperation;
    public void LoadGameScene()
    {
        CollectibleIcon.color = new Color(1f, 1f, 1f, 0.2f);
        CollectibleCountText.text = "0/" + TotalCollectibleCount;

        if (CinematicControllerSensei == null)
        {
            CinematicControllerSensei = GetComponentInChildren<CinematicController>();
        }


        if (_isLoadingGameScene)
        {
            return;
        }
        _isLoadingGameScene = true;

        CollectedItemIDs.Clear();
        //251216 - 양현용 : 새게임시 기믹 초기화 및 씬 설정
        //수집품 초기화
        _collectSlotContoller.CollectSlotClear();
        UpdateCollectSlot(_collectSlotContoller.Slots[401]);

        Debug.Log("스타트");
        _curScene = 1;
        _isGimmickClear.Clear();
        _checkPointData.Clear();//기믹,아이템 초기화
        _checkPointData._onCheck = false;
        player = null;

        if (_gameOverCoroutine != null)
        {
            StopCoroutine(_gameOverCoroutine);
            _gameOverCoroutine = null;
        }


        if (_gameOverCoroutine != null)
        {
            StopCoroutine(_gameOverCoroutine);
            _gameOverCoroutine = null;
        }

        _currentPlayerHealth = _maxPlayerHealth;
        HeartLogic();
        SoundEffectManager.Instance.PlayBGM(_bgms[_curScene]);
        Time.timeScale = 1f;
        GameSceneLoadAsyncOperation = SceneManager.LoadSceneAsync(1);
        GameSceneLoadAsyncOperation.allowSceneActivation = false;

        StartCoroutine(WaitForGameSceneLoad());


        _checkItem = false;
        //SceneManager.LoadScene(1);
    }


    IEnumerator WaitForVictorySceneLoad()
    {
        while (GameSceneLoadAsyncOperation.progress < 0.9f)
        {
            yield return null;
        }

        //yield return null;

        GameSceneLoadAsyncOperation.allowSceneActivation = true;
        //yield return new WaitForSecondsRealtime(0.3f);
        while (CinematicControllerSensei.InCutscene)
        {
            yield return null;
        }

        yield return GameSceneLoadAsyncOperation.isDone;

        SoundEffectManager.Instance.PlayBGM(_bgms[_bgms.Length - 1]);
        foreach (var canvas in CanvasList)
        {
            if (canvas == CanvasList[4] || canvas == CanvasList[5])
            {
                canvas.SetActive(true);
            }
            else
            {
                canvas.SetActive(false);
            }
        }

        //StartCoroutine(SpawnPlayer());
        //_player = Instantiate(_playerPrefab, _playerSpawnPos[0], Quaternion.identity);
        //player = _player.GetComponent<Player>();
        //Debug.Log("플레이어 생성");

    }
    IEnumerator WaitForGameSceneLoad()
    {
        while (GameSceneLoadAsyncOperation.progress < 0.9f)
        {
            yield return null; ;
        }
        CinematicControllerSensei.PlayCutscene();
        //yield return null;

        //yield return null;
        GameSceneLoadAsyncOperation.allowSceneActivation = true;

        while (CinematicControllerSensei.InCutscene)
        {
            yield return null;
        }

        yield return GameSceneLoadAsyncOperation.isDone;

        _isLoadingGameScene = false;

        StartCoroutine(SpawnPlayer());
        //_player = Instantiate(_playerPrefab, _playerSpawnPos[0], Quaternion.identity);
        //player = _player.GetComponent<Player>();
        Debug.Log("플레이어 생성");

    }

    public void RestartGame()
    {
        if (!_checkPointData._onCheck)
        {
            CollectibleIcon.color = new Color(1f, 1f, 1f, 0.2f);
            CollectibleCountText.text = "0/" + TotalCollectibleCount;
            CurrentCutsceneIndex = 0;
            _checkPointData.Clear();//기믹,아이템 초기화
            CollectedItemIDs.Clear();//아이템픽업 관련 초기화
            //아이템 퀵슬롯 초기화 
            GameManagerQuickSlots = new QuickSlot[10];
            for (int i = 0; i < GameManagerQuickSlotCountTexts.Length; i++)
            {
                GameManagerQuickSlotCountTexts[i].text = "";
                GameManagerQuickSlotIcons[i].gameObject.SetActive(false);
                GameManagerQuickSlots[i] = null;
            }
            if (_gameOverCoroutine != null)
            {
                StopCoroutine(_gameOverCoroutine);
                _gameOverCoroutine = null;
            }
            Time.timeScale = 1f;
            //+추가로직
            _curScene = 1;


            _currentPlayerHealth = _maxPlayerHealth;
            HeartLogic();

            GameSceneLoadAsyncOperation = SceneManager.LoadSceneAsync(1);
            GameSceneLoadAsyncOperation.allowSceneActivation = false;
            StartCoroutine(WaitForGameSceneLoad());
        }
        else
        {
            EnterPhaseOne();
            Debug.Log("체크포인트 기반 다시하기");
            //_checkPointData.Clear();//기믹,아이템 초기화
            if (_gameOverCoroutine != null)
            {
                StopCoroutine(_gameOverCoroutine);
                _gameOverCoroutine = null;
            }
            Time.timeScale = 1f;

            //수집품에 대한 로직이 필요함!

            //CollectibleIcon.color = new Color(1f, 1f, 1f, 0.2f);
            //CollectibleCountText.text = "0/" + TotalCollectibleCount;


            _checkPointData.LoadCheckPointData();
            GameManager.Instance.RefreshAllQuickSlotUI();
            LoadIndexScene(_curScene);
        }


    }


    public void HeartLogic()
    {

        switch (_currentPlayerHealth)
        {
            case 3:
                foreach (var heart in HeartImages)
                {


                    heart.color = _heartFullColor;
                }
                break;
            case 2:
                HeartImages[0].color = _heartFullColor;
                HeartImages[1].color = _heartFullColor;
                HeartImages[2].color = _heartEmptyColor;
                break;
            case 1:
                HeartImages[0].color = _heartFullColor;
                HeartImages[1].color = _heartEmptyColor;
                HeartImages[2].color = _heartEmptyColor;
                break;
            case 0:
                HeartImages[0].color = _heartEmptyColor;
                HeartImages[1].color = _heartEmptyColor;
                HeartImages[2].color = _heartEmptyColor;
                break;
        }




    }


    public void LoadMainMenuScene()
    {
        CanvasList[0].SetActive(true);
        SceneManager.LoadScene(0);
        //1224 - 양현용 : 씬 넘버 적용
        _curScene = 0;
        CurrentCutsceneIndex = 0;
        CinematicControllerSensei.ClearCutscene();
        SoundEffectManager.Instance.PlayBGM(_bgms[_curScene]);

    }


    public int GetTotalSceneCount()
    {
        return SceneManager.sceneCountInBuildSettings;
    }

    [SerializeField] float _blackFadeToVictoryCutscenesTime = 1.5f;
    Coroutine _blackFadeToVictoryCoroutine;

    IEnumerator BlackFadeToVictoryCutscene()
    {
        Color tempColor = new Color(0f, 0f, 0f, 1f);
        float tempFadeTime = _blackFadeToVictoryCutscenesTime;
        //ExtraUITools[1].GetComponent<Image>().raycastTarget = true;

        CinematicControllerSensei.PreloadVictoryBackground();
        while (tempFadeTime > 0)
        {
            tempColor.a = tempFadeTime / _blackFadeToVictoryCutscenesTime;
            ExtraUITools[0].GetComponent<Image>().color = tempColor;
            yield return _wait;
            tempFadeTime -= _minusGameOverCount;
        }
        CinematicControllerSensei.PlayCutscene();
    }

    public void LoadVictoryScene()
    {
        GameManager.Instance.CollectedItemIDs.Clear();
        _blackFadeToVictoryCoroutine = StartCoroutine(BlackFadeToVictoryCutscene());
        _checkPointData.Clear();//기믹,아이템 초기화
        if (_gameOverCoroutine != null)
        {
            StopCoroutine(_gameOverCoroutine);
            _gameOverCoroutine = null;
        }
        EnterPhaseOne();

        int temp = SceneManager.sceneCountInBuildSettings;

        GameSceneLoadAsyncOperation = SceneManager.LoadSceneAsync(temp - 1);
        GameSceneLoadAsyncOperation.allowSceneActivation = false;
        StartCoroutine(WaitForVictorySceneLoad());
        //SceneManager.LoadScene(temp - 1);
        //Time.timeScale = 0f;

        //CanvasList[4].SetActive(true);
        //추가로직
    }




    public void PauseGame()
    {

        Time.timeScale = 0f;
        //추가로직
    }

    public void ResumeGame()
    {

        Time.timeScale = 1f;
        //추가로직
    }




    public void PlayerTakeDamage(int damage)
    {
        _currentPlayerHealth -= damage;
        HeartLogic();
        if (_currentPlayerHealth <= 0)
        {
            PlayerDeath();
        }
        //체력 감소 로직
        //추가로직
    }

    public void PlayerHeal(int heal)
    {
        _currentPlayerHealth += heal;
        if (_currentPlayerHealth > _maxPlayerHealth)
        {
            _currentPlayerHealth = _maxPlayerHealth;
        }
        HeartLogic();
        //체력 회복 로직
    }



    [SerializeField] float _deathWhiteToBlackFadeDuration = 1.5f;

    Coroutine _deathWhiteToBlackFadeCoroutine;

    WaitForSecondsRealtime _waitRealTime;
    IEnumerator DeathWhiteToBlackFade()
    {
        float fadeDuration = _deathWhiteToBlackFadeDuration;
        Color tempColor = new Color(1f, 1f, 1f, 1f);
        ExtraUITools[0].GetComponent<Image>().raycastTarget = true;
        while (fadeDuration > 0)
        {
            tempColor.g = fadeDuration / _deathWhiteToBlackFadeDuration;
            tempColor.b = fadeDuration / _deathWhiteToBlackFadeDuration;
            tempColor.r = fadeDuration / _deathWhiteToBlackFadeDuration;
            ExtraUITools[0].GetComponent<Image>().color = tempColor;
            yield return _waitRealTime;
            fadeDuration -= _minusGameOverCount;
        }


        fadeDuration = _deathWhiteToBlackFadeDuration;
        while (fadeDuration > 0)
        {
            //ExtraUITools[0].GetComponent<Image>().raycastTarget = true;
            tempColor.a = (fadeDuration / _deathWhiteToBlackFadeDuration);
            ExtraUITools[0].GetComponent<Image>().color = tempColor;
            yield return _waitRealTime;
            fadeDuration -= _minusGameOverCount;
        }
        ExtraUITools[0].GetComponent<Image>().raycastTarget = false;

    }

    public void PlayerDeath()
    {
        SoundEffectManager.Instance.PlayBGM(_bgms[_bgms.Length - 2]);

        _deathWhiteToBlackFadeCoroutine = StartCoroutine(DeathWhiteToBlackFade());

        foreach (GameObject canvas in CanvasList)
        {
            Debug.Log(canvas.name);
            if (canvas != CanvasList[3] && canvas != CanvasList[5])
            {
                canvas.SetActive(false);

            }



        }
        if (CanvasList[3].activeSelf == false)
        {

            CanvasList[3].SetActive(true);
        }
        if (CanvasList[5].activeSelf == false)
        {
            CanvasList[5].SetActive(true);
        }
        Time.timeScale = 0f;
        EnterPhaseOne();
        //추가로직

    }





    public void ExitGame()
    {
        Application.Quit();
    }


    public List<GameObject> ExtraUITools { get; set; } = new List<GameObject>();

    public List<GameObject> CanvasList { get; set; } = new List<GameObject>();
    public List<Button> MenuButton { get; set; } = new List<Button>();

    public List<Button> IngameButton { get; set; } = new List<Button>();
    public List<Button> PauseMenuButton { get; set; } = new List<Button>();
    public List<Button> GameOverMenuButton { get; set; } = new List<Button>();

    //Victory Menu Button
    public List<Button> VictoryMenuButton { get; set; } = new List<Button>();

    public List<Button> ControlGuideMenuButton { get; set; } = new List<Button>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {

    }
}

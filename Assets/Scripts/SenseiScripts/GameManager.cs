using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;
using System.Collections;
using UnityEditor;
using NUnit.Framework.Constraints;

public enum GameState
{
    PhaseOne,
    PhaseTwo
}

public class GameManager : Singleton<GameManager>
{


    [Header("플레이어 체력 관련")]
    [SerializeField] private int _maxPlayerHealth = 3;
    [SerializeField] private int CurrentPlayerHealth = 1;
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

        LoadTables();      // 1) 파싱
        ResolveTableAssets(); // 2) 프리팹/아이콘 실제 로드
    }

    void LoadTables()
    {
        TextAsset csv = Resources.Load<TextAsset>("Data/ItemTable"); // Resources/Data/ItemTable.csv
        ItemTable = TableParser.Parse<int, ItemTableData>(csv, "ID");
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

    public QuickSlot[] GameManagerQuickSlots { get; set; } = new QuickSlot[3];
    public TextMeshProUGUI[]GameManagerQuickSlotCountTexts { get; set; } = new TextMeshProUGUI[3];
    public Image[] GameManagerQuickSlotIcons { get; set; } = new Image[3];




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
    public Dictionary<int, bool> IsGimmickClear => _isGimmickClear;

    [SerializeField] int _curScene = 0;

    //


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
    //0. 튜토리얼 1. 스테이지 2 :Phase 1 2. 스테이지 4 3. 스테이지 2 : Phase 2
    [SerializeField] AudioClip[] _bgms;

    void Start()
    {
        //251216 - 양현용 추가 : 테스트용 플레이어 스크립트를 찾는 용도
        //해당 값을 찾는 기능은 현재 테스트 용으로  start에 있으나, 이후 플레이어 스폰 지점으로 이동 예정
        //player = GameObject.FindFirstObjectByType<Player>().GetComponent<Player>();
        _wait = new WaitForSeconds(_minusGameOverCount);

        ColorUtility.TryParseHtmlString("#" + _heartFullHexColor, out _heartFullColor);
        ColorUtility.TryParseHtmlString("#" + _heartEmptyHexColor, out _heartEmptyColor);
    }

    public void EnterPhaseTwo()
    {
        SoundEffectManager.Instance.PlayBGM(_bgms[3]);
        _gameState = (GameState)1;
        //
        if (_gameOverCoroutine == null)
        {
            _gameOverCoroutine = StartCoroutine(CheckGameOver());
        }
    }
    public void EnterPhaseOne()
    {
        _gameState = (GameState)0;
        _checkItem=false;
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

    IEnumerator CheckGameOver()
    {
        _curGameOverCount = _gameOverCount;
        _curGameOverCount -= _minusGameOverCount;
        while (_curGameOverCount > 0)
        {
            yield return _wait;
            _curGameOverCount -= _minusGameOverCount;
        }
        PlayerDeath();
    }

    IEnumerator SpawnPlayer()
    {
        yield return GameSceneLoadAsyncOperation.isDone;
        yield return null;
        SoundEffectManager.Instance.PlayBGM(_bgms[_curScene - 1]);
        _player = Instantiate(_playerPrefab, _playerSpawnPos[_curScene - 1], Quaternion.identity);
        player = _player.GetComponent<Player>();
    }
    IEnumerator SpawnPlayer_Prev(int spawnPos)
    {
        yield return GameSceneLoadAsyncOperation.isDone;
        yield return null;
        if (!_checkItem)
        {
            SoundEffectManager.Instance.PlayBGM(_bgms[_curScene - 1]);
        }
        _player = Instantiate(_playerPrefab, _playerSpawnPos[spawnPos], Quaternion.identity);
        player = _player.GetComponent<Player>();


        //if (phaseChange)
        //{
        //    EnterPhaseTwo();
        //}
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
        //251216 - 양현용 : 새게임시 기믹 초기화 및 씬 설정
        Debug.Log("스타트");
        _curScene = 1;
        _isGimmickClear.Clear();
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

        CurrentPlayerHealth = _maxPlayerHealth;
        HeartLogic();

        Time.timeScale = 1f;
        GameSceneLoadAsyncOperation = SceneManager.LoadSceneAsync(1);
        GameSceneLoadAsyncOperation.allowSceneActivation = false;
        StartCoroutine(WaitForGameSceneLoad());




        //SceneManager.LoadScene(1);
    }


    IEnumerator WaitForGameSceneLoad()
    {

        while (GameSceneLoadAsyncOperation.progress < 0.9f)
        {
            yield return new WaitForFixedUpdate();
        }

            GameSceneLoadAsyncOperation.allowSceneActivation = true;

        yield return GameSceneLoadAsyncOperation.isDone;
        StartCoroutine(SpawnPlayer());
        //_player = Instantiate(_playerPrefab, _playerSpawnPos[0], Quaternion.identity);
        //player = _player.GetComponent<Player>();
        Debug.Log("플레이어 생성");

    }

    public void RestartGame()
    {
        _isGimmickClear.Clear();
        if (_gameOverCoroutine != null)
        {
            StopCoroutine(_gameOverCoroutine);
            _gameOverCoroutine = null;
        }
        Time.timeScale = 1f;
        //+추가로직
        _curScene = 1;


        CurrentPlayerHealth = _maxPlayerHealth;
        HeartLogic();

        GameSceneLoadAsyncOperation = SceneManager.LoadSceneAsync(1);
        GameSceneLoadAsyncOperation.allowSceneActivation = false;
        StartCoroutine(WaitForGameSceneLoad());

        //StartCoroutine(SpawnPlayer());
    }
    

    private void HeartLogic()
    {

        switch(CurrentPlayerHealth)
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
    }


    public void LoadVictoryScene()
    {
        if (_gameOverCoroutine != null)
        {
            StopCoroutine(_gameOverCoroutine);
            _gameOverCoroutine = null;
        }
        EnterPhaseOne();

        int temp = SceneManager.sceneCountInBuildSettings;

        SceneManager.LoadScene(temp - 1);
        //Time.timeScale = 0f;
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
        CurrentPlayerHealth -= damage;
        HeartLogic();
        if (CurrentPlayerHealth <= 0)
        {
            PlayerDeath();
        }
        //체력 감소 로직
        //추가로직
    }


    public void PlayerDeath()
    {
        foreach (GameObject canvas in CanvasList)
        {
            Debug.Log(canvas.name);
            if (canvas != CanvasList[3] || canvas != CanvasList[5])
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

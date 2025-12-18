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
    public bool CheckItem => _checkItem;
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

    private Dictionary<int, bool> _isGimmickClear = new Dictionary<int, bool>();
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

    void Start()
    {
        //251216 - 양현용 추가 : 테스트용 플레이어 스크립트를 찾는 용도
        //해당 값을 찾는 기능은 현재 테스트 용으로  start에 있으나, 이후 플레이어 스폰 지점으로 이동 예정
        //player = GameObject.FindFirstObjectByType<Player>().GetComponent<Player>();
        _wait = new WaitForSeconds(_minusGameOverCount);
    }

    public void EnterPhaseTwo()
    {
        _gameState = (GameState)1;
        //
        _gameOverCoroutine = StartCoroutine(CheckGameOver());
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
        float _curCount = _gameOverCount;
        _curCount -= _minusGameOverCount;
        while (_curCount > 0)
        {
            yield return _wait;
            _curCount -= _minusGameOverCount;
        }
        PlayerDeath();
    }

    IEnumerator SpawnPlayer()
    {
        yield return GameSceneLoadAsyncOperation.isDone;
        _player = Instantiate(_playerPrefab, _playerSpawnPos[_curScene - 1], Quaternion.identity);
        player = _player.GetComponent<Player>();
    }
    IEnumerator SpawnPlayer_Prev(int spawnPos, bool phaseChange)
    {
        yield return GameSceneLoadAsyncOperation.isDone;
        _player = Instantiate(_playerPrefab, _playerSpawnPos[spawnPos], Quaternion.identity);
        player = _player.GetComponent<Player>();

        if (phaseChange)
        {
            EnterPhaseTwo();
        }
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
            StartCoroutine(SpawnPlayer_Prev(3, _checkItem));
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
        _player = Instantiate(_playerPrefab, _playerSpawnPos[0], Quaternion.identity);
        player = _player.GetComponent<Player>();
        Debug.Log("플레이어 생성");

    }

    

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene(0);
    }


    public void LoadVictoryScene()
    {
        int temp = SceneManager.sceneCountInBuildSettings;

        SceneManager.LoadScene(temp - 1);
        //Time.timeScale = 0f;
        foreach (var canvas in CanvasList)
        {
            if (canvas != CanvasList[4])
            {
                canvas.SetActive(false);
            }
            else
            {
                canvas.SetActive(true);
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


    public void RestartGame()
    {
        Time.timeScale = 1f;
        //+추가로직
        _curScene = 1;

        //현재 체크포인트 개념 없음
        if (SceneManager.GetActiveScene().buildIndex != 1)
        {
            SceneManager.LoadScene(1);
        }
    }


    public void PlayerTakeDamage(int damage)
    {
        //체력 감소 로직
        //추가로직
    }


    public void PlayerDeath()
    {
        foreach (GameObject canvas in CanvasList)
        {
            canvas.SetActive(false); 
        }

        Time.timeScale = 0f;
        CanvasList[3].SetActive(true);
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        
    }
}

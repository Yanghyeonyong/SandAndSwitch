using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    private List<GameObject> _canvasList = new List<GameObject>();
    private List<Button> _menuButton = new List<Button>();
    private List<Button> _ingameButton = new List<Button>();
    private List<Button> _pauseMenuButton = new List<Button>();
    private List<Button> _gameOverMenuButton = new List<Button>();
    private List<Button> _controlGuideMenuButton = new List<Button>();

    //승리메뉴 추가
    private List<Button> _victoryMenuButton = new List<Button>();


    InputAction _pauseGameAction;

    AudioSource _audioSource;

    void Start()
    {


        //UI 관련
        /*
        Canvas는 인덱스 순으로 MainMenu, IngameUI, PauseMenu, GameOverMenu
        버튼 동작은 UIManager에서 처리하되 그 함수를 GameManager에서 호출할 수 있게 만들어놨다
        버튼 순서는 각 와이어프레임에서 좌에서 우 혹은 위에서 아래 순이다

        MenuButton: Start, ControlGuide, Exit
        IngameButton: Pause
        PauseMenuButton: Restart, MainMenu, Resume, ControlGuide
        GameOVerMenuButton: Restart, MainMenu

        ControlGuide는 아직 기획표가 안나옴
        */

        int index = 0;
        foreach (Transform child in transform)
        {
            if (index ==1)
            {
                GameManager.Instance.HeartImages.Add(child.GetChild(0).GetComponent<Image>());
                GameManager.Instance.HeartImages.Add(child.GetChild(1).GetComponent<Image>());
                GameManager.Instance.HeartImages.Add(child.GetChild(2).GetComponent<Image>());
            }


            if (index==5)
            {
                _audioSource = child.GetComponent<AudioSource>();

                
            }

            int grandchildIndex = 0;
            if (index != 7 && index != 8 && index != 9)
            {
                GameManager.Instance.CanvasList.Add(child.gameObject);
                _canvasList.Add(child.gameObject);
            }

            if (index ==8)
            {
                foreach (Transform UITools in child)
                {
                    GameManager.Instance.ExtraUITools.Add(UITools.gameObject);
                }
            }

                Debug.Log(child.name);
            
                foreach (Transform grandChild in child)
            {
                if (grandChild.GetComponent<Button>() !=null)
                {
                    Button button = grandChild.GetComponent<Button>();
                    if (index == 0)
                    {
                        
                        GameManager.Instance.MenuButton.Add(button);
                        _menuButton.Add(button);
                        

                    }
                    else if (index == 1)
                    {
                        GameManager.Instance.IngameButton.Add(button);
                        _ingameButton.Add(button);

                    }
                    else if (index == 2)
                    {
                        GameManager.Instance.PauseMenuButton.Add(button);
                        _pauseMenuButton.Add(button);
                        //Debug.Log(button.name);
                    }
                    else if (index == 3)
                    {
                        GameManager.Instance.GameOverMenuButton.Add(button);
                        _gameOverMenuButton.Add(button);
                        //Debug.Log(button.name);
                    }
                    else if (index ==4)
                    {
                        //승리메뉴 버튼 추가
                        GameManager.Instance.VictoryMenuButton.Add(button);
                        _victoryMenuButton.Add(button);
                        //Debug.Log(button.name);
                    }
                    else if (index == 6)
                    {
                        GameManager.Instance.ControlGuideMenuButton.Add(button);
                        _controlGuideMenuButton.Add(button);
                        Debug.Log(button.name);
                        break;
                    }

                }

                else if (index ==1)
                {
                    switch (grandchildIndex)
                    {
                        case 5:
                            for (int greatGrandChildIndex = 0; greatGrandChildIndex < 10; greatGrandChildIndex ++)
                            
                            {
                                if(grandChild.GetChild(greatGrandChildIndex)==null)
                                {
                                    continue;
                                }
                                GameManager.Instance.GameManagerQuickSlotsImages[greatGrandChildIndex] = grandChild.GetChild(greatGrandChildIndex).GetChild(0).GetComponent<Image>();
                                GameManager.Instance.GameManagerQuickSlotCountTexts[greatGrandChildIndex] = grandChild.GetChild(greatGrandChildIndex).GetChild(3).GetComponent<TextMeshProUGUI>();
                                GameManager.Instance.GameManagerQuickSlotIcons[greatGrandChildIndex] = grandChild.GetChild(greatGrandChildIndex).GetChild(1).GetComponent<Image>();
                                
                                if (GameManager.Instance.GameManagerQuickSlotsImages[greatGrandChildIndex].gameObject.activeSelf)
                                {
                                GameManager.Instance.GameManagerQuickSlotIcons[greatGrandChildIndex].gameObject.SetActive(false);
                                    //continue;
                                }
                                //Debug.Log(GameManager.Instance.GameManagerQuickSlotCountTexts[greatGrandChildIndex].name);

                            }
                            break;

                    }



                }

                grandchildIndex++;



                
            }
            index++;
     
        }

        _menuButton[0].onClick.AddListener(GameManager.Instance.LoadGameScene);
        _menuButton[0].onClick.AddListener(LoadGameSceneLogic);
        _menuButton[0].onClick.AddListener(PlayUIClickSound);
        _menuButton[1].onClick.AddListener(ControlGuideLogic);
        _menuButton[1].onClick.AddListener(PlayUIClickSound);
        _menuButton[2].onClick.AddListener(GameManager.Instance.ExitGame);
        _menuButton[2].onClick.AddListener(PlayUIClickSound);

        _ingameButton[0].onClick.AddListener(GameManager.Instance.PauseGame);
        _ingameButton[0].onClick.AddListener(PauseLogic);
        _ingameButton[0].onClick.AddListener(PlayUIClickSound);

        _pauseMenuButton[0].onClick.AddListener(GameManager.Instance.RestartGame);
        _pauseMenuButton[0].onClick.AddListener(PlayUIClickSound);
        _pauseMenuButton[0].onClick.AddListener(RestartLogic);
        _pauseMenuButton[1].onClick.AddListener(GameManager.Instance.LoadMainMenuScene);
        _pauseMenuButton[1].onClick.AddListener(PlayUIClickSound);
        _pauseMenuButton[1].onClick.AddListener(LoadMainMenuLogic);
        _pauseMenuButton[2].onClick.AddListener(GameManager.Instance.ResumeGame);
        _pauseMenuButton[2].onClick.AddListener(PlayUIClickSound);
        _pauseMenuButton[2].onClick.AddListener(ResumeLogic);
        _pauseMenuButton[3].onClick.AddListener(ControlGuideLogic);
        _pauseMenuButton[3].onClick.AddListener(PlayUIClickSound);

        _gameOverMenuButton[0].onClick.AddListener(GameManager.Instance.RestartGame);
        _gameOverMenuButton[0].onClick.AddListener(PlayUIClickSound);
        _gameOverMenuButton[0].onClick.AddListener(RestartLogic);
        _gameOverMenuButton[1].onClick.AddListener(PlayUIClickSound);
        _gameOverMenuButton[1].onClick.AddListener(GameManager.Instance.LoadMainMenuScene);
        _gameOverMenuButton[1].onClick.AddListener(LoadMainMenuLogic);


        _victoryMenuButton[0].onClick.AddListener(GameManager.Instance.LoadMainMenuScene);
        _victoryMenuButton[0].onClick.AddListener(PlayUIClickSound);
        _victoryMenuButton[0].onClick.AddListener(LoadMainMenuLogic);

        _controlGuideMenuButton[0].onClick.AddListener(ControlGuideLogic);
        _controlGuideMenuButton[0].onClick.AddListener(PlayUIClickSound);

        _pauseGameAction = InputSystem.actions.FindActionMap("Player").FindAction("ESC");
        _pauseGameAction.performed += ESCAction;
    }

    private void OnDestroy()
    {
        if (_pauseGameAction != null)
        {
            _pauseGameAction.performed -= ESCAction;
        }
    }


    List<GameObject> _tempCanvasList = new List<GameObject>();

    void ControlGuideLogic()
    {
        if (_canvasList[6].activeSelf)
        {
            _canvasList[6].SetActive(false);
            foreach (GameObject canvas in _tempCanvasList)
            {
                
                canvas.SetActive(true);
            }
            _tempCanvasList.Clear();
            //return;
        }

        else if (!_canvasList[6].activeSelf)
        {
            foreach (GameObject canvas in _canvasList)
            {
                if (canvas.activeSelf & canvas != _canvasList[5])
                {
                    _tempCanvasList.Add(canvas);
                    canvas.SetActive(false);
                }
            }
            _canvasList[6].SetActive(true);
        }




    }

    void ESCAction(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.CinematicControllerSensei.InCutscene)
        {
                       return;
        }
        //251222 - 양현용 추가 : 기믹 UI 끄기
        if (GameManager.Instance.OnSelection)
        {
            GameManager.Instance.Player.CurGimmick.CheckNum(-1);
            return;
        }
        else if (GameManager.Instance.Player.CheckGimmick && GameManager.Instance.OnProgressGimmick)
        {
            GameManager.Instance.Player.CurGimmick.ExitGimmick();
            return;
        }

        if (GameManager.Instance.GetCurrentSceneIndex() != 0 && _canvasList[1].activeSelf)
        {
            if (!_canvasList[2].activeSelf)
            {
                _ingameButton[0].onClick.Invoke();
            }
            else if (_canvasList[2].activeSelf)
            {
                _pauseMenuButton[2].onClick.Invoke();
            }
        }

        else if (_canvasList[6].activeSelf)
        {
            _controlGuideMenuButton[0].onClick.Invoke();
        }


        //GameManager.Instance.CinematicControllerSensei.ResetPlayableDirector();
    }

    private void PlayUIClickSound()
    {
        _audioSource.PlayOneShot(_audioSource.clip);
    }

    private void PauseLogic()
    {
        _canvasList[2].SetActive(true);
    }

    private void ResumeLogic()
    {
        _canvasList[2].SetActive(false);
    }

    private void RestartLogic()
    {

        for (int i = 0; i < GameManager.Instance.GameManagerQuickSlotCountTexts.Length; i++)
        {
            GameManager.Instance.GameManagerQuickSlotCountTexts[i].text = "";
            GameManager.Instance.GameManagerQuickSlotIcons[i].gameObject.SetActive(false);
            GameManager.Instance.GameManagerQuickSlots[i] = null;
        }


        StartCoroutine(WaitForAsyncGameSceneLoad());
        
    }

    private void LoadGameSceneLogic()
    {
        for (int i = 0; i < GameManager.Instance.GameManagerQuickSlotCountTexts.Length; i++)
        {
            GameManager.Instance.GameManagerQuickSlotCountTexts[i].text = "";
            GameManager.Instance.GameManagerQuickSlotIcons[i].gameObject.SetActive(false);
            GameManager.Instance.GameManagerQuickSlots[i] = null;
        }
        //GameManager.Instance.GameManagerQuickSlotCountTexts[0].text = "";
        //GameManager.Instance.GameManagerQuickSlotIcons[0].gameObject.SetActive(false);
        StartCoroutine(WaitForAsyncGameSceneLoad());
    }

    IEnumerator WaitForAsyncGameSceneLoad()
    {

        while (GameManager.Instance.GameSceneLoadAsyncOperation.progress < 0.95f)
        {
            yield return null;
        }
        
        if(_canvasList[0].activeSelf)
        {
            _canvasList[0].SetActive(false);
        }
        if (!_canvasList[1].activeSelf)
        {
            _canvasList[1].SetActive(true);
        }

        if (_canvasList[2].activeSelf)
        {
            _canvasList[2].SetActive(false);
        }
        if (_canvasList[3].activeSelf)
        {
            _canvasList[3].SetActive(false);
        }
    }




    private void LoadSaveGameSceneLogic()
    {
        _canvasList[0].SetActive(false);
        _canvasList[1].SetActive(true);
    }

    private void LoadMainMenuLogic()
    {
        foreach (GameObject canvas in _canvasList)
        {
            if (canvas == _canvasList[0] || canvas == _canvasList[5])

            {
                if (canvas.activeSelf == false)
                {
                    canvas.SetActive(true);
                }
            }
            else
            {
                canvas.SetActive(false);
            }
        }
    }


    //private void LoadVictoryMenuLogic()
    //{
    //    foreach (GameObject canvas in _canvasList)
    //    {
    //        if (canvas == _canvasList[4])

    //        {
    //            canvas.SetActive(true);
    //        }
    //        else
    //        {
    //            canvas.SetActive(false);
    //        }
    //    }
    //}

    //private void GameOverLogic()
    //{         
    //    _canvasList[3].SetActive(true);
    //}



    GameObject _hoveredUI;



    GameObject GetCurrentHoveredUI()
    {


        PointerEventData mouseEventData = new PointerEventData(EventSystem.current);
        if (Mouse.current != null)
        {
            mouseEventData.position = Mouse.current.position.ReadValue();
        }

        List<RaycastResult> pointerRaycastHits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(mouseEventData, pointerRaycastHits);


        if (pointerRaycastHits.Count > 0)
        {
            return pointerRaycastHits[0].gameObject;
        }
        return null;

    }

    // Update is called once per frame
    void Update()
    {
        
        if (GameManager.Instance.GetCurrentSceneIndex() !=0 && GameManager.Instance.GetCurrentSceneIndex() != GameManager.Instance.GetTotalSceneCount()-1)
        {
            if (EventSystem.current == null)
            {
                return;
            }
            if (EventSystem.current.IsPointerOverGameObject())
            {
            if ( GetCurrentHoveredUI() != null)
            {
               // if (GetCurrentHoveredUI().GetComponent<Image>.sprite 

            }
                //_hoveredUI = GetCurrentHoveredUI();
            }

            else
            {
                
            }

        }


    }
}

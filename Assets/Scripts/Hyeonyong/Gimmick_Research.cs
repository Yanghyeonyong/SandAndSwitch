using System.Collections;
using UnityEngine;

public class Gimmick_Research : Gimmick
{
    [SerializeField] string _tableId;
    [SerializeField] GameObject _interactiveUI;
    Coroutine _coroutine;
    bool _onPlayer=false;   
    public override void StartGimmick()
    {
        string text = GameManager.Instance.Player.GetStringFromTable(_tableId);
        if (_coroutine == null)
        {
            _interactiveUI.SetActive(false);
            _coroutine = StartCoroutine(GameManager.Instance.Player.ShowChatBubble(text));
            StartCoroutine(StartResearch());
        }
    }

    IEnumerator StartResearch()
    {
        yield return new WaitForSeconds(GameManager.Instance.Player.chatDuration);
        if (_onPlayer)
        {
            _interactiveUI.SetActive(true);
        }
        _coroutine = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _onPlayer = true;   
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _onPlayer = false;
        }
    }
}

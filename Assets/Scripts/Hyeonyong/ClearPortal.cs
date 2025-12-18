using System.Collections;
using UnityEngine;

public class ClearPortal : MonoBehaviour
{
    bool check;
    private void Start()
    {
        if (GameManager.Instance.CheckItem == false)
        {
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.Instance.GameOverCoroutine != null)
            {
                StopCoroutine(GameManager.Instance.GameOverCoroutine);
                GameManager.Instance.GameOverCoroutine = null;
            }
            GameManager.Instance.LoadVictoryScene();
            GameManager.Instance.EnterPhaseOne();
        }
    }
}

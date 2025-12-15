using UnityEngine;

public class UIStart : MonoBehaviour
{
    [SerializeField] GameObject[] inGameUI;
    private void Start()
    {
        foreach (GameObject ui in inGameUI)
        {
            ui.SetActive(false);
        }
    }
}

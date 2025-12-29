using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UIAutoSelectGamepad : MonoBehaviour
{

    void OnEnable()
    {
        //if (Gamepad.all.Count > 0)
        //{
        //    EventSystem.current.SetSelectedGameObject(null);
        //    EventSystem.current.SetSelectedGameObject(GetComponentInChildren<Button>().gameObject);
        //}

    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Gamepad.all.Count > 0 && EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            Button temp = GetComponentInChildren<Button>();

            EventSystem.current.SetSelectedGameObject(temp.gameObject);

        }

    }

    void OnDisable()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

}

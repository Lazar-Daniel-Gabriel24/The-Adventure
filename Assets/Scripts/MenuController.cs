using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas;

    void Start()
    {
        menuCanvas.SetActive(false);
    }

    void Update()
    {
        if (KeybindSettings.Instance.GetKeyDown("Menu"))
        {
            menuCanvas.SetActive(!menuCanvas.activeSelf);
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerConnectUIController : MonoBehaviour
{
    public static ServerConnectUIController instance;

    public GameObject startMenu;
    public InputField userNameField;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void ConnectToServer()
    {
        startMenu.SetActive(false);
        userNameField.interactable = false;
        ConnectionToServer.instance.ConnectToServer();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ConnectToServer();
        }
    }
}
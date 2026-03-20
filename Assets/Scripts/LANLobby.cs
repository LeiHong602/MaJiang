using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.FizzySteam;

public class LANLobby : MonoBehaviour
{
    NetworkManager networkManager;
    public static LANLobby Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            networkManager = GetComponent<NetworkManager>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 创建局域网大厅点击时
    /// </summary>
    public void OnCreatLANLobbyButtonClicked()
    {
        networkManager.networkAddress = "localhost";
        GetComponent<TelepathyTransport>().enabled=true;
        networkManager.transport = GetComponent<TelepathyTransport>();
        Transport.active = networkManager.transport;//需要更改Transport的单例，单单更改NetworkManager的transport并没有影响到整个游戏的transport，它只会在初始化的时候将transport赋值到Transport的单例
        networkManager.StartHost();
    }

    /// <summary>
    /// 加入局域网大厅点击时
    /// </summary>
    public void OnEnterLANLobbyButtonClicked()
    {
        GetComponent<TelepathyTransport>().enabled = true;
        networkManager.transport = GetComponent<TelepathyTransport>();
        Transport.active = networkManager.transport;//需要更改Transport的单例，单单更改NetworkManager的transport并没有影响到整个游戏的transport，它只会在初始化的时候将transport赋值到Transport的单例
        networkManager.StartClient();
    }
}

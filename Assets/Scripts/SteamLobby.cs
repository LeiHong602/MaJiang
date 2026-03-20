using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using Mirror.FizzySteam;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance { get; private set; }
    public TextMeshProUGUI debugText;

    private MyNetworkRoomManager networkRoomManager;
    private const string hostAddressKey = "HostAddress";//主机端地址的键


    protected Callback<LobbyCreated_t> LobbyCreated;//创建大厅时
    protected Callback<GameLobbyJoinRequested_t> LobbyJoinRequested;//收到大厅加入申请的时候
    protected Callback<LobbyEnter_t> LobbyEnter;//进入大厅

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            networkRoomManager = GetComponent<MyNetworkRoomManager>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (!SteamManager.Initialized)//未初始化Steam时
        {
            debugText.text = "Steam初始化失败/未连接到Steam服务器";
            return;
        }
        debugText.text = "Steam初始化成功/已连接到Steam服务器";

        //networkRoomManager.transport = GetComponent<FizzySteamworks>();

        //注册回调函数
        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        LobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequested);
        LobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            debugText.text = "创建大厅失败";
            return;
        }
        debugText.text = "创建大厅成功";
        networkRoomManager.StartHost();//启动主机
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey, SteamUser.GetSteamID().ToString());//设置大厅数据
    }

    private void OnLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        debugText.text = "收到大厅加入申请";
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);//加入大厅
    }

    private void OnLobbyEnter(LobbyEnter_t callback)
    {
        debugText.text = "有玩家进入大厅";
        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey);//获取大厅数据,主机端地址的键对应的主机端地址
        networkRoomManager.networkAddress = hostAddress;//设置网络地址

        if (!networkRoomManager.isNetworkActive)//如果没有启用网络服务，说明本机不是主机端或者已连接的客户端
        {
            networkRoomManager.StartClient();//启动客户端
            debugText.text = "玩家正在连接到主机...请稍候...";
        }
    }

    //创建大厅的按钮
    public void HostLobby()
    {
        GetComponent<FizzySteamworks>().enabled = true;
        networkRoomManager.transport = GetComponent<FizzySteamworks>();
        Transport.active = networkRoomManager.transport;
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkRoomManager.maxConnections);//创建好友大厅，最大玩家数量
    }
}

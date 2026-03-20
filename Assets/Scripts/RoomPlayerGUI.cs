using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomPlayerGUI : MonoBehaviour
{

    [SerializeField]
    GameObject playerPanelPrefab;

    Button readyButton;
    Button cancelButton;
    Button removeButton;

    TextMeshProUGUI playerName;
    TextMeshProUGUI readyState;
    GameObject playerList;
    GameObject playerPanel;
    NetworkRoomPlayer player;

    private void Start()
    {
        InitializeUI();
    }
    private void OnEnable()
    {
        //SceneManager.sceneLoaded += OnSceneLoaded;       ?????????
    }
    private void OnDisable()
    {
        //SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //检查是否是当前场景
        if (scene.name == "RoomScene")
        {
            InitializeUI();
        }
    }


    private void InitializeUI()
    {
        player = GetComponent<NetworkRoomPlayer>();
        playerList = GameObject.FindWithTag("PlayerList");
        playerPanel = Instantiate(playerPanelPrefab, playerList.transform) as GameObject;
        readyButton = playerPanel.transform.Find("ReadyButton").GetComponent<Button>();
        cancelButton = playerPanel.transform.Find("CancelButton").GetComponent<Button>();
        removeButton = playerPanel.transform.Find("RemoveButton").GetComponent<Button>();
        playerName = playerPanel.transform.Find("PlayerName").GetComponent<TextMeshProUGUI>();
        readyState = playerPanel.transform.Find("ReadyState").GetComponent<TextMeshProUGUI>();

        readyButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        removeButton.gameObject.SetActive(false);

        if (NetworkClient.active && player.isLocalPlayer)
        {
            readyButton.onClick.AddListener(OnReadyButtonClicked);
            cancelButton.onClick.AddListener(OnCancelButtonClicked);
        }

        if (player.isServer && !player.isLocalPlayer)
        {
            removeButton.gameObject.SetActive(true);
            removeButton.onClick.AddListener(OnRemoveButtonClicked);

        }
    }

    private void Update()
    {
        if (playerName != null)
        {
            playerName.text = $"Player [{player.index + 1}]";
        }
        if (readyState != null)
        {
            readyState.text = player.readyToBegin ? "已准备" : "未准备";
        }
        if (NetworkClient.active && player.isLocalPlayer)
        {
            if (readyButton != null && cancelButton != null)
            {
                readyButton.gameObject.SetActive(!player.readyToBegin);
                cancelButton.gameObject.SetActive(player.readyToBegin);
            }
        }
    }

    private void OnReadyButtonClicked()
    {
        readyButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(true);
        player.CmdChangeReadyState(true);
    }

    private void OnCancelButtonClicked()
    {
        readyButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(false);
        player.CmdChangeReadyState(false);
    }

    private void OnRemoveButtonClicked()
    {
        GetComponent<NetworkIdentity>().connectionToClient.Disconnect();//???????????
        if (playerPanel != null)
        {
            Destroy(playerPanel);
        }
    }

    private void OnDestroy()
    {
        readyButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
        removeButton.onClick.RemoveAllListeners();
    }
}

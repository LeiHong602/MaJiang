using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMenuController : MonoBehaviour
{
    [SerializeField]
    GameObject startButton;

    private void Awake()
    {
        MyNetworkRoomManager.Instance.startGameButton = startButton;
    }

    private void Start()
    {
        startButton.SetActive(false);
    }

    public void StartGame()
    {
        MyNetworkRoomManager.Instance.StartGame();
    }
}

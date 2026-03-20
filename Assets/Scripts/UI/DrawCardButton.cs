using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCardButton : MonoBehaviour
{
    public PlayerCardController playerCardController;

    public void OnDrawCardButtonClick()
    {
        playerCardController.CmdInitCardPos(playerCardController.initCardPos.position, playerCardController.transform.eulerAngles.y);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuCardButtonController : MonoBehaviour
{
    public PlayerCardController playerCardController;

    public void OnHuCardButtonClick()
    {
        playerCardController.CmdAllServerCardChangeToNormal();
    }
}

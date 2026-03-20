using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarCardButtonController : MonoBehaviour
{
    public void OnBarCardButtonClick()
    {
        if (!MouseManager.Instance.selectedCardList.Count.Equals(3))
        {
            Debug.Log("没选中三张牌");
            return;
        }
        CardManager.Instance.BarCard(MouseManager.Instance.selectedCardList[0], MouseManager.Instance.selectedCardList[1], MouseManager.Instance.selectedCardList[2], CardManager.Instance.GetSyncNowOutCard());
    }
}

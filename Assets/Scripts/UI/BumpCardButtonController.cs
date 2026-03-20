using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumpCardButtonController : MonoBehaviour
{
    public void OnBumpCardButtonClick()
    {
        if (!MouseManager.Instance.selectedCardList.Count.Equals(2))
        {
            Debug.Log("没选中两张牌");
            return;
        }
        CardManager.Instance.BumpCard(MouseManager.Instance.selectedCardList[0], MouseManager.Instance.selectedCardList[1], CardManager.Instance.GetSyncNowOutCard());
    }
}

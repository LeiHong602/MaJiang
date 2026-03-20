using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatCardButtonController : MonoBehaviour
{
    public PlayerCardController playerCardController;
    public void OnEatCardButtonClick()
    {
        if(!MouseManager.Instance.selectedCardList.Count.Equals(2))
        {
            Debug.Log("没选中两张牌");
            return;
        }
        //CardManager.Instance.EatCard(MouseManager.Instance.selectedCardList[0], MouseManager.Instance.selectedCardList[1], CardManager.Instance.GetSyncNowOutCard());
        playerCardController.CmdEatCard(MouseManager.Instance.selectedCardList[0], MouseManager.Instance.selectedCardList[1], CardManager.Instance.GetSyncNowOutCard(),CardManager.Instance.originalCombinationX,CardManager.Instance.originalCombinationZ,CardManager.Instance.rowNumCombination);
        CardManager.Instance.combinationCardList.Add(MouseManager.Instance.selectedCardList[0]);
        CardManager.Instance.combinationCardList.Add(MouseManager.Instance.selectedCardList[1]);
        CardManager.Instance.combinationCardList.Add(CardManager.Instance.GetSyncNowOutCard());
        CardManager.Instance.rowNumCombination++;
    }
}

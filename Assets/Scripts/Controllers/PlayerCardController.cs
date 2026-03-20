using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerCardController : NetworkBehaviour
{
    public Transform initCardPos;
    public GameObject cardPrefab;//卡牌预制体

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        CardManager.Instance.playerCardController = this;
        FindAnyObjectByType<EatCardButtonController>().playerCardController=this;
        FindAnyObjectByType<DrawCardButton>().playerCardController = this;
        FindAnyObjectByType<HuCardButtonController>().playerCardController = this;

    }


    //服务器设置全局目前所出的牌
    [Command]
    public void CmdSetSyncNowOutCard(GameObject card)
    {
        CardManager.Instance.syncNowOutCard = card;
    }

    [Command]
    public void CmdEatCard(GameObject card1, GameObject card2, GameObject card3,float x,float z,int num)
    {
        CardManager.Instance.EatCard(card1, card2, card3,x,z,num);
        ServerChangeCardToNormal(card1);
        ServerChangeCardToNormal(card2);
        ServerChangeCardToNormal(card3);
        
    }

    //服务器将牌放到组合区
    [Command]
    public void CmdBumpCard(GameObject card1, GameObject card2, GameObject card3, float originalCombinationX, float originalCombinationZ, float offsetCombinationX, float offsetCombinationZ, int rowNumCombination)
    {
        // 请求权限转移
        NetworkIdentity card1NetId = card1.GetComponent<NetworkIdentity>();
        NetworkIdentity card2NetId = card2.GetComponent<NetworkIdentity>();
        NetworkIdentity card3NetId = card3.GetComponent<NetworkIdentity>();

        card1NetId.RemoveClientAuthority();
        card2NetId.RemoveClientAuthority();
        card3NetId.RemoveClientAuthority();

        //删除服务器中的手牌
        CardManager.Instance.serverCardList.Remove(card1);
        CardManager.Instance.serverCardList.Remove(card2);
        CardManager.Instance.serverCardList.Remove(card3);

        card3.transform.rotation = card1.transform.rotation;//朝向统一

        card1.transform.position = new Vector3(originalCombinationX, 0, originalCombinationZ);
        card1.transform.Translate(new Vector3(0, 0, rowNumCombination * offsetCombinationZ), Space.Self);
        card2.transform.position = new Vector3(originalCombinationX, 0, originalCombinationZ);
        card2.transform.Translate(new Vector3(offsetCombinationX, 0, rowNumCombination * offsetCombinationZ), Space.Self);
        card3.transform.position = new Vector3(originalCombinationX, 0, originalCombinationZ);
        card3.transform.Translate(new Vector3(2 * offsetCombinationX, 0, rowNumCombination * offsetCombinationZ), Space.Self);

        ServerChangeCardToNormal(card1);
        ServerChangeCardToNormal(card2);
        ServerChangeCardToNormal(card3);
    }

    //杠的时候服务器将牌放到组合区
    [Command]
    public void CmdBarCard(GameObject card1, GameObject card2, GameObject card3, GameObject card4, float originalCombinationX, float originalCombinationZ, float offsetCombinationX, float offsetCombinationZ, int rowNumCombination)
    {
        // 请求权限转移
        NetworkIdentity card1NetId = card1.GetComponent<NetworkIdentity>();
        NetworkIdentity card2NetId = card2.GetComponent<NetworkIdentity>();
        NetworkIdentity card3NetId = card3.GetComponent<NetworkIdentity>();
        NetworkIdentity card4NetId = card4.GetComponent<NetworkIdentity>();

        card1NetId.RemoveClientAuthority();
        card2NetId.RemoveClientAuthority();
        card3NetId.RemoveClientAuthority();
        card4NetId.RemoveClientAuthority();

        card4.transform.rotation = card1.transform.rotation;//朝向统一

        //删除服务器中的手牌
        CardManager.Instance.serverCardList.Remove(card1);
        CardManager.Instance.serverCardList.Remove(card2);
        CardManager.Instance.serverCardList.Remove(card3);
        CardManager.Instance.serverCardList.Remove(card4);

        card1.transform.position = new Vector3(originalCombinationX, 0, originalCombinationZ);
        card1.transform.Translate(new Vector3(0, 0, rowNumCombination * offsetCombinationZ), Space.Self);
        card2.transform.position = new Vector3(originalCombinationX, 0, originalCombinationZ);
        card2.transform.Translate(new Vector3(offsetCombinationX, 0, rowNumCombination * offsetCombinationZ), Space.Self);
        card3.transform.position = new Vector3(originalCombinationX, 0, originalCombinationZ);
        card3.transform.Translate(new Vector3(2 * offsetCombinationX, 0, rowNumCombination * offsetCombinationZ), Space.Self);
        card4.transform.position = new Vector3(originalCombinationX, 0, originalCombinationZ);
        card4.transform.Translate(new Vector3(3 * offsetCombinationX, 0, rowNumCombination * offsetCombinationZ), Space.Self);

        //改变麻将材质
        ServerChangeCardToNormal(card1);
        ServerChangeCardToNormal(card2);
        ServerChangeCardToNormal(card3);
        ServerChangeCardToNormal(card4);
    }

    //开局摸牌
    [Command]
    public void CmdInitCardPos(Vector3 CardPos, float ry)
    {
        int count = 0;
        for (int i = 0; i < 16; i++)
        {
            if (CardManager.Instance.remainingCardMeshList.Count <= 0)
            {
                Debug.Log("没有牌了");
                return;
            }
            int index = Random.Range(0, CardManager.Instance.remainingCardMeshList.Count);
            GameObject card = Instantiate(cardPrefab, CardPos ,Quaternion.Euler(0,ry,0));
            card.transform.Translate(new Vector3(CardManager.Instance.offsetX * count, 0, 0), Space.Self);
            card.GetComponent<Card>().meshName = CardManager.Instance.remainingCardMeshList[index].name;//初始值应该放在spawn之前
            NetworkServer.Spawn(card, gameObject);


            CardManager.Instance.remainingCardMeshList.RemoveAt(index);
            CardManager.Instance.serverCardList.Add(card);

            count++;
        }

    }

    //服务器通知客户端改变卡牌
    [Command]
    public void CmdChangeCardToNormal(GameObject card)
    {
        ChangeCardToNormalClientRpc(card);
    }

    [Server]
    public void ServerChangeCardToNormal(GameObject card)
    {
        ChangeCardToNormalClientRpc(card);

    }

    //所有客户端都改变卡牌
    [ClientRpc]
    public void ChangeCardToNormalClientRpc(GameObject card)
    {
        Debug.Log(card.name + "恢复了正常材质");
        card.GetComponent<Renderer>().material = CardManager.Instance.normalMaterial;
    }

    [Command]
    public void CmdAllServerCardChangeToNormal()
    {
        foreach(GameObject card in CardManager.Instance.serverCardList)
        {
            ChangeCardToNormalClientRpc(card);
        }
    }

    //减少服务器手牌数量
    [Command]
    public void CmdReduceServerCardList(GameObject card)
    {
        CardManager.Instance.serverCardList.Remove(card);
    }
}

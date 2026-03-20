using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] 
    private Camera playerCamera; // 拖入玩家子物体的Camera

    [Header("出牌和吃碰杠位置相关")]
    public Transform outCardPos; // 出牌位置
    public Transform combinationCardPos; // 吃碰杠位置


    [SerializeField]
    private GameObject dealCard;//玩家自身的卡牌生成器


    // 网络对象生成时调用（客户端/服务器都会执行）
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        for(int i=0;i<4;i++)
        {
            //旋转玩家使其看向桌子
            if (GamePlayManager.Instance.startPos[i].position!=this.transform.position)
            {
                continue;
            }
            if(i==0)
            {
            }
            else if(i==1)
            {
                this.transform.Rotate(0, -90, 0);
            }
            else if(i==2)
            {
                this.transform.Rotate(0, 180, 0);
            }
            else if(i==3)
            {
                this.transform.Rotate(0, 90, 0);
            }
            break;
        }

        CardManager.Instance.originalX = outCardPos.position.x;
        CardManager.Instance.originalZ = outCardPos.position.z;
        CardManager.Instance.originalCombinationX = combinationCardPos.position.x;
        CardManager.Instance.originalCombinationZ = combinationCardPos.position.z;

        // 仅本地玩家激活摄像机
        if (playerCamera != null)
        {
            playerCamera.enabled = true;
            // 设为主摄像机（覆盖场景默认摄像机）
            playerCamera.tag = "MainCamera";
        }
        dealCard.SetActive(true);//激活玩家自身的卡牌生成器

        // 禁用场景中默认的主摄像机（避免冲突）
        Camera mainCam = Camera.main;
        if (mainCam != null && mainCam.gameObject != playerCamera.gameObject)
        {
            mainCam.gameObject.SetActive(false);
        }

        Debug.Log("本地玩家摄像机激活");
    }

    // 远程玩家生成时调用（本地玩家不会走这个逻辑）
    //public override void OnStartClient()
    //{
    //    base.OnStartClient();
    //    // 非本地玩家：禁用摄像机和音频监听
    //    if (!isLocalPlayer)
    //    {
    //        if (playerCamera != null) playerCamera.enabled = false;
    //        if (cameraAudioListener != null) cameraAudioListener.enabled = false;
    //    }
    //}
}

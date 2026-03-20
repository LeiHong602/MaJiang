using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Numerics;
using UnityEngine.UIElements;

public class CardManager : NetworkBehaviour
{
    public static CardManager Instance { get; private set; }

    [SerializeField]
    private List<Mesh> allCardMeshList;//所有牌

    public PlayerCardController playerCardController;

    //[SerializeField]
    public SyncList<Mesh> remainingCardMeshList=new SyncList<Mesh>();//所有牌的剩余牌

    [Header("出牌相关")]
    public float originalX;
    public float originalZ;
    public float offsetX;
    public float offsetZ;
    public int numX;//每行牌数
    public List<GameObject> playCardList;//已出牌的牌

    [Header("吃碰杠相关")]
    public float originalCombinationX;
    public float originalCombinationZ;
    public float offsetCombinationX;
    public float offsetCombinationZ;
    public int rowNumCombination;//行数
    public List<GameObject> combinationCardList;

    public List<GameObject> myCardList;//我的牌
    public List<GameObject> serverCardList;//服务器所有人手中的牌
    [SerializeField]
    private GameObject cardPrefab;//牌预制体

    [SyncVar]
    public GameObject syncNowOutCard;//需同步的全局最后出的牌

    [Header("麻将材质相关")]
    public Material hideMaterial;
    public Material normalMaterial;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        //将牌×4放入剩余牌堆中
        if (isServer)
        {
            Debug.Log("服务器将牌乘四");
            int c = allCardMeshList.Count;
            for (int i = 0; i < c; i++)
            {
                Mesh mesh = allCardMeshList[i];
                for (int j = 0; j < 4; j++)
                {
                    remainingCardMeshList.Add(mesh);
                }
            }
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (isClient)
        {
            Debug.Log("客户端绑定回调");
            remainingCardMeshList.Callback += OnRemainingCardMeshListChanged;
        }
    }

    private void Start()
    {
        playCardList = new List<GameObject>();
        myCardList = new List<GameObject>();
        combinationCardList=new List<GameObject>();
        serverCardList = new List<GameObject>();
    }

    private void OnDestroy()
    {
        remainingCardMeshList.Callback -= OnRemainingCardMeshListChanged;
    }

    /// <summary>
    /// 剩余牌堆变化回调
    /// </summary>
    /// <param name="operation"></param>
    /// <param name="arg2"></param>
    /// <param name="mesh1"></param>
    /// <param name="mesh2"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    private void OnRemainingCardMeshListChanged(SyncList<Mesh>.Operation operation, int arg2, Mesh mesh1, Mesh mesh2)
    {
        GamePlayUIManager.Instance.SetRemainCard(remainingCardMeshList.Count);
    }

    public GameObject GetSyncNowOutCard()
    {
        return syncNowOutCard;
    }

    //拿牌
    [Server]
    public void CreateCard(UnityEngine.Vector3 position,GameObject player, UnityEngine.Quaternion quaternion)
    {
        if (remainingCardMeshList.Count <= 0)
        {
            Debug.Log("没有牌了");
            return;
        } 
        int index = Random.Range(0, remainingCardMeshList.Count);
        GameObject card = Instantiate(cardPrefab, position, quaternion);
        card.GetComponent<Card>().meshName = remainingCardMeshList[index].name;//初始值应该放在spawn之前
        NetworkServer.Spawn(card,player);
        
        remainingCardMeshList.RemoveAt(index);
        serverCardList.Add(card);
        
        return;
    }

    ////开局摸牌
    //public void DrawCard()
    //{
    //    for(int i=0;i<16;i++)
    //    {
    //        if (remainingCardMeshList.Count <= 0)
    //        {
    //        Debug.Log("没有牌了");
    //        return;
    //        }
    //        int index = Random.Range(0, remainingCardMeshList.Count);
    //        GameObject card = Instantiate(cardPrefab);
    //        card.GetComponent<Card>().meshName = remainingCardMeshList[index].name;//初始值应该放在spawn之前
    //        //NetworkServer.Spawn(card, player);


    //        remainingCardMeshList.RemoveAt(index);
    //        myCardList.Add(card);
    //    }
    //    playerCardController.CmdInitCardPos(myCardList,offsetX);
    //}

    //出牌行为
    public void PlayCard(GameObject card)
    {
        int Z=(int)(playCardList.Count/numX);
        int X=playCardList.Count%numX;
        card.transform.position = new UnityEngine.Vector3(originalX , 0, originalZ );
        card.transform.Translate(new UnityEngine.Vector3( X * offsetX, 0,  - Z * offsetZ), Space.Self);//局部坐标系移动
        Debug.Log("X:" + X.ToString() + "Z:" + Z.ToString());
        playCardList.Add(card);
        playerCardController.CmdSetSyncNowOutCard(card);
        playerCardController.CmdChangeCardToNormal(card);
        playerCardController.CmdReduceServerCardList(card);
        MouseManager.Instance.ClearSelectedCardList();
        
    }

    /// <summary>
    /// 吃牌逻辑
    /// </summary>
    /// <param name="card1"></param>
    /// <param name="card2"></param>
    /// <param name="card3"></param>
    [Server]
    public void EatCard(GameObject card1, GameObject card2, GameObject card3,float x,float z,int num)
    {
        // 请求权限转移
        NetworkIdentity card1NetId = card1.GetComponent<NetworkIdentity>();
        NetworkIdentity card2NetId = card2.GetComponent<NetworkIdentity>();
        NetworkIdentity card3NetId = card3.GetComponent<NetworkIdentity>();

        card1NetId.RemoveClientAuthority();
        card2NetId.RemoveClientAuthority();
        card3NetId.RemoveClientAuthority();

        card3.transform.rotation=card1.transform.rotation;//朝向统一
        //card1NetId.AssignClientAuthority(connectionToClient);
        //card2NetId.AssignClientAuthority(connectionToClient);
        //card3NetId.AssignClientAuthority(connectionToClient);
        serverCardList.Remove(card1);
        serverCardList.Remove(card2);
        serverCardList.Remove(card3);

        card1.transform.position = new UnityEngine.Vector3(x , 0.2f, z);
        card1.transform.Translate(new UnityEngine.Vector3(0,0, num * offsetCombinationZ),Space.Self);
        card2.transform.position = new UnityEngine.Vector3(x  , 0.2f, z);
        card2.transform.Translate(new UnityEngine.Vector3(offsetCombinationX, 0, num * offsetCombinationZ), Space.Self);
        card3.transform.position = new UnityEngine.Vector3(x, 0.2f, z);
        card3.transform.Translate(new UnityEngine.Vector3(2*offsetCombinationX, 0, num * offsetCombinationZ), Space.Self);
        //Debug.Log("card1:" + card1.transform.position + ",card2:" + card2.transform.position + ",card3:" + card3.transform.position);
        
    }

    /// <summary>
    /// 碰牌逻辑
    /// </summary>
    /// <param name="card1"></param>
    /// <param name="card2"></param>
    /// <param name="card3"></param>
    public void BumpCard(GameObject card1, GameObject card2, GameObject card3)
    {
        //card1.transform.position = new Vector3(originalCombinationX, 0, originalCombinationZ + rowNumCombination * offsetCombinationZ);
        //card2.transform.position = new Vector3(originalCombinationX + offsetCombinationX, 0, originalCombinationZ + rowNumCombination * offsetCombinationZ);
        //card3.transform.position = new Vector3(originalCombinationX + 2 * offsetCombinationX, 0, originalCombinationZ + rowNumCombination * offsetCombinationZ);
        playerCardController.CmdBumpCard(card1, card2, card3, originalCombinationX, originalCombinationZ, offsetCombinationX, offsetCombinationZ, rowNumCombination);
        combinationCardList.Add(card1);
        combinationCardList.Add(card2);
        combinationCardList.Add(card3);
        rowNumCombination++;
    }

    public void BarCard(GameObject card1, GameObject card2, GameObject card3, GameObject card4)
    {
        playerCardController.CmdBarCard(card1, card2, card3,card4, originalCombinationX, originalCombinationZ, offsetCombinationX, offsetCombinationZ, rowNumCombination);
        combinationCardList.Add(card1);
        combinationCardList.Add(card2);
        combinationCardList.Add(card3);
        combinationCardList.Add(card4);
        rowNumCombination++;
    }


    


}

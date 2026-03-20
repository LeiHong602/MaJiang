using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.VisualScripting;
using System;

public class Card : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnMeshChanged))]
    public string meshName;//服务器生成时更改的mesh

    public List<Mesh> meshList = new List<Mesh>();//本身也有一个网格列表，mesh没法sync，只能通过名字
    public bool flag = true;

    [SyncVar(hook = nameof(OnSyncPosChanged))]
    public Vector3 syncPos;

    public override void OnStartAuthority()
    {
        Debug.Log("卡牌OnStartAuthority");
        GetComponent<Renderer>().material = CardManager.Instance.normalMaterial;//改变材质为可见
    }


    private void OnSyncPosChanged(Vector3 oldSyncPos,Vector3 newSyncPos)
    {
        transform.position = newSyncPos;
    }

    private void OnMeshChanged(string oldMeshName, string newMeshName)
    {
        Debug.Log("麻将的mesh改变了" + newMeshName);
        foreach (Mesh mesh in meshList)
        {
            if(mesh!=null && mesh.name==newMeshName)
            {
                GetComponent<MeshFilter>().mesh = mesh;
                break;
            }
        }
    }

    private void OnMouseDrag()
    {
        //Debug.Log("鼠标拖拽");
        
        transform.position = GetMousePos();
    }

    private Vector3 GetMousePos()
    {
        //Debug.Log("获取鼠标位置");
        // 1. 从摄像机向屏幕点击点发射射线
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 只检测“Ground”层（先在Layer面板创建Backgroud层，赋值为8）
        int groundLayer = LayerMask.GetMask("Background");
        // 2. 检测射线与地面的交点（真正的世界坐标）
        if (Physics.Raycast(ray, out RaycastHit hit,1000,groundLayer))
        {
            return new Vector3(hit.point.x,0,hit.point.z); // hit.point 就是准确的世界坐标
        }
        return new Vector3(0, 0, 0);
    }
}

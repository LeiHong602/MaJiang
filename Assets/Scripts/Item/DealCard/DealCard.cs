using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DealCard : NetworkBehaviour
{

    private void OnMouseDown()
    {
        Debug.Log("DealCard OnMouseDown");
        // 1. 从摄像机向屏幕点击点发射射线
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 只检测“DealCard”层（先在Layer面板创建DealCard层，赋值为8）
        int groundLayer = LayerMask.GetMask("DealCard");
        // 2. 检测射线与地面的交点（真正的世界坐标）
        if (Physics.Raycast(ray, out RaycastHit hit, 1000, groundLayer))
        {
            CmdCreateCard(hit.point, this.transform.parent.localEulerAngles.y);
        }
    }


    [Command]
    public void CmdCreateCard(Vector3 position,float y)
    {
        CardManager.Instance.CreateCard(position,this.transform.parent.gameObject,Quaternion.Euler(0, y, 0));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance;
    // 选中物体的高亮材质（可选，用于视觉反馈）
    public Material highlightMaterial;
    // 物体原本的材质（用于取消选中时恢复）
    public Material originalMaterial;
    // 当前选中的物体
    public GameObject selectedObject;

    public List<GameObject> selectedCardList;//选中的卡牌列表

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            SelectObject();
        }

        // 可选：按ESC取消选中
        if (Input.GetKeyDown(KeyCode.Escape) && selectedObject != null)
        {
            ClearSelectedCardList();
        }
    }

    /// <summary>
    /// 核心：鼠标选中物体逻辑
    /// </summary>
    void SelectObject()
    {
        //若鼠标位于ui上则退出
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        // 2. 生成从摄像机到鼠标位置的射线
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // 3. 射线检测（检测距离设为100米，可根据需求调整）
        int cardLayer = LayerMask.GetMask("Card");
        int BackgroundLayer = LayerMask.GetMask("Background");
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, cardLayer))
        {
            // 4. 获取击中的物体
            selectedObject = hit.collider.gameObject;
            if(selectedCardList.Contains(selectedObject) == false)//判断selectedObjcet是否已经在选中卡牌列表中
            {
                selectedCardList.Add(selectedObject);//添加到选中卡牌列表
            }
            
            Debug.Log("选中了物体：" + selectedObject.name);

            // 5. 选中视觉反馈（可选，比如变色）
            if (highlightMaterial != null && selectedObject.GetComponent<Renderer>() != null)
            {
                selectedObject.GetComponent<Renderer>().material = highlightMaterial;
            }
        }
        else if (Physics.Raycast(ray, out RaycastHit hit3, 100f, BackgroundLayer))
        {
            if (selectedCardList != null && selectedCardList.Count > 0)
            {
                ClearSelectedCardList();
            }
        }
    }

    /// <summary>
    /// 取消选中物体
    /// </summary>
    void UnselectObject()
    {
        Debug.Log("取消选中：" + selectedObject.name);
        // 恢复物体原本的材质
        if (originalMaterial != null && selectedObject.GetComponent<Renderer>() != null)
        {
            selectedObject.GetComponent<Renderer>().material = originalMaterial;
        }
        // 清空选中状态
        selectedObject = null;
        originalMaterial = null;
    }

    /// <summary>
    /// 清空已选择卡牌列表
    /// </summary>
    public void ClearSelectedCardList()
    {
        foreach (GameObject card in selectedCardList)
        {
            card.GetComponent<Renderer>().material = originalMaterial;
        }
        selectedObject = null;
        selectedCardList.Clear();
    }
}

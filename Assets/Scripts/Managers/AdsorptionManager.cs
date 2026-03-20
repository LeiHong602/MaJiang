using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AdsorptionManager : MonoBehaviour
{
    public Vector2 Size = new Vector2(1, 1);
    //标志距离阀值
    public float Distance = 0.5f;
    public LayerMask m_TankMask;
    // Used to filter what the explosion affects, this should be set to "Players".
    public float m_ExplosionRadius = 0.3f;
    // The maximm distance away from the explosion tanks can be and are still affected.

    public float offset;//麻将的宽的一半
    //分别获取两个模型的接口坐标信息
    Transform m_door;
    Transform T_door;

    private void Awake()
    {
        m_door = transform;
        T_door = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("enter");
        other.GetComponent<Card>().flag = false;
        if(other.gameObject==MouseManager.Instance.selectedObject)
        {
            Transform ot = other.transform;
            Transform pt = transform.parent;
            if (pt.position.x > ot.position.x)
            {
                Vector3 v = new Vector3(pt.position.x - offset, pt.position.y, pt.position.z);
                Vector3 moveVector = v - ot.position;
                other.transform.Translate(moveVector, Space.World);
            }
            else
            {
                
                Vector3 v = new Vector3(pt.position.x + offset, pt.position.y, pt.position.z);
                Vector3 moveVector = v - ot.position;
                other.transform.Translate(moveVector, Space.World);
            }
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    Debug.Log("exit");
    //    T_door = null; 
    //}
    //// Update is called once per frame0个引用
    //void Update()
    //{
    //    if (IsClose())
    //    {
    //        Debug.Log("接近");
    //        RotateThisl();
    //        MoveTo();
    //    }
    //}

    ////判断两个模型接口是否很接近
    //public bool IsClose()
    //{
    //    if(m_door!= null && T_door!= null)
    //    {
    //        float tempDistance = Vector3.Distance(m_door.position, T_door.position);
    //        Debug.Log(tempDistance);
    //        if(tempDistance < Distance && tempDistance > 0)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    //public void MoveTo()
    //{

    //    if(T_door.position.x>m_door.position.x)
    //    {
    //        Vector3 v = new Vector3(m_door.position.x - offset, m_door.position.y, m_door.position.z);
    //        Vector3 moveVector = v - m_door.position;
    //        m_door.parent.Translate(moveVector, Space.World);
    //    }
    //    else
    //    {
    //        Vector3 v = new Vector3(m_door.position.x + offset, m_door.position.y, m_door.position.z);
    //        Vector3 moveVector = v - m_door.position;
    //        m_door.parent.Translate(moveVector, Space.World);
    //    }
        
        
    //}

    //public void RotateThisl()
    //{
    //    Vector3 RotateAix = Vector3.Cross(m_door.forward, T_door.forward);
    //    float angle = Vector3.Angle(m_door.forward, T_door.forward) + 180;
    //    m_door.parent.Rotate(RotateAix, angle, Space.World);

    //    float Angle = Vector3.Angle(m_door.up, T_door.up);
    //    Debug.Log(Angle);
    //    m_door.parent.Rotate(T_door.forward, Angle, Space.World);
    //}

    private void OnDrawGizmos()
    {
        Vector2 halfSize = Size * 0.5f;
        Gizmos.color = Color.red;
        float lineLength = Mathf.Min(Size.x, Size.y);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * lineLength);
        Gizmos.color = Color.blue;

        Vector3 topLeft = transform.position - (transform.right * halfSize.x) + (transform.up * Size.y) / 2;
        Vector3 topRight = transform.position + (transform.right * halfSize.x) + (transform.up * Size.y) / 2;
        Vector3 bottomLeft = transform.position - (transform.right * halfSize.x) - (transform.up * Size.y) / 2;
        Vector3 bottomRight = transform.position + (transform.right * halfSize.x) - (transform.up * Size.y) / 2;

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);

        //
        Gizmos.DrawWireSphere(transform.position, m_ExplosionRadius);
    }
}

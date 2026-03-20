using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayACardButtonController : MonoBehaviour
{
    public void PlayACard()
    {
        if(MouseManager.Instance.selectedObject!= null)
        {
            CardManager.Instance.PlayCard(MouseManager.Instance.selectedObject);
        }
    }
}

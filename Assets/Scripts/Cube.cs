using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Cube : NetworkBehaviour
{
    private void Update()
    {
        if(isLocalPlayer)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            transform.Translate(new Vector3(h, v,0 ) * Time.deltaTime * 10);
        }
    }
}

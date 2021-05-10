using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{

    public Transform target;
    public Vector3 offset;
   

   
    void Update()
    {
        transform.position = target.position + offset; //카메라 해당 위치에 고정
    }
}

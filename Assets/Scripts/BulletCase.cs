using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCase : MonoBehaviour
{
    public float destroyTime;
   
    // Start is called before the first frame update
    private void Awake()
    {
     
        
    }

    
    void deActive()
    {
        gameObject.SetActive(false);
    }
   

    private void Update()
    {
        
    }
    private void OnEnable()
    {
        Invoke("deActive", destroyTime);

    }
    private void OnDisable()
    {
        CancelInvoke();
    }
}

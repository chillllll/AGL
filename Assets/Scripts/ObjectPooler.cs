using UnityEngine;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;
    public GameObject poolObjPrefab;//풀링할 오브젝트 프리펩
    
    GameObject[] poolObj; //풀링한 오브젝트가 들어갈 배열
    public int poolQuan; //풀링할 갯수
    Queue<GameObject> poolGameObjects;

    private void Awake()
    {
        poolObj = new GameObject[poolQuan];
        poolGameObjects = new Queue<GameObject>();
        Generate(); //시작시 미리 생성
        //Initialize(poolQuan);
    }

   //배열형식
    private void Generate() //풀 갯수만큼 미리 생성
    {
        
         for (int index = 0; index < poolObj.Length; index++)
         {
             poolObj[index] = Instantiate(poolObjPrefab);
             poolObj[index].SetActive(false);
         }
        

    }

    public GameObject MakeObj()
    {     
        for (int index = 0; index < poolObj.Length; index++)
        {
            if (!poolObj[index].activeSelf) //활성화 되있지 않으면
            {              
                poolObj[index].SetActive(true); //활성화               
                return poolObj[index];               
            }
        }      
        return null;
    }

    

    /*
     //큐 형식
    private GameObject CreateNewObj()
    {
        var newObj = Instantiate(poolObjPrefab, transform).GetComponent<GameObject>();
        newObj.gameObject.SetActive(false);
        return newObj;
    }

    private void Initialize(int count)
    {
        for(int i=0; i<count; i++)
        {
            poolGameObjects.Enqueue(CreateNewObj());
        }
    }

    public GameObject GetObj()
    {
        if (Instance.poolGameObjects.Count > 0)
        {
            var obj = Instance.poolGameObjects.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            var newObj = Instance.CreateNewObj();
            newObj.transform.SetParent(null);
            newObj.gameObject.SetActive(true);
            return newObj;
        }

    }

    public static void ReturnObjecet(GameObject gameobj)
    {
        gameobj.gameObject.SetActive(false);
        gameobj.transform.SetParent(Instance.transform);
        Instance.poolGameObjects.Enqueue(gameobj);
    }


    */
    
    
    
}

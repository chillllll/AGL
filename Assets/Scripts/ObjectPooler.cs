using UnityEngine;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;
    public GameObject poolObjPrefab;//Ǯ���� ������Ʈ ������
    
    GameObject[] poolObj; //Ǯ���� ������Ʈ�� �� �迭
    public int poolQuan; //Ǯ���� ����
    Queue<GameObject> poolGameObjects;

    private void Awake()
    {
        poolObj = new GameObject[poolQuan];
        poolGameObjects = new Queue<GameObject>();
        Generate(); //���۽� �̸� ����
        //Initialize(poolQuan);
    }

   //�迭����
    private void Generate() //Ǯ ������ŭ �̸� ����
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
            if (!poolObj[index].activeSelf) //Ȱ��ȭ ������ ������
            {              
                poolObj[index].SetActive(true); //Ȱ��ȭ               
                return poolObj[index];               
            }
        }      
        return null;
    }

    

    /*
     //ť ����
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

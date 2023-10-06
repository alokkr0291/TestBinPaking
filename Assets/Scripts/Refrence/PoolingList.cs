using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingList : MonoBehaviour
{
    public List<GameObject> SKU_01L = new List<GameObject>();
   // public List<GameObject> SKU_01S = new List<GameObject>();
    public List<GameObject> SKU_04L = new List<GameObject>();
  //  public List<GameObject> SKU_04S = new List<GameObject>();
  //  public List<GameObject> SKU_10D = new List<GameObject>();
    public List<GameObject> SKU_10L = new List<GameObject>();
 //   public List<GameObject> SKU_20D = new List<GameObject>();
    public List<GameObject> SKU_20L = new List<GameObject>();
    public List<GameObject> SKU_PLY = new List<GameObject>();
    public List<GameObject> SKU_STR = new List<GameObject>();


    public int _count = 300;
    public bool allowInstantiate = false;

    public void Start()
    {
        if(allowInstantiate)
            Init();
    }


    private void Init()
    {

        for (int i = 0; i < _count; i++)
        {
            var obj = Instantiate(SKU_01L[0], new Vector3(2000, 2000, 0), Quaternion.identity);
            obj.name = "SKU_01L";
            obj.transform.SetParent(this.transform);
                SKU_01L.Add(obj);



            var obj3 = Instantiate(SKU_04L[0], new Vector3(2000, 2000, 0), Quaternion.identity);
            obj3.transform.SetParent(this.transform);
            obj3.name = "SKU_04L";
            SKU_04L.Add(obj3);

          

            var obj6 = Instantiate(SKU_10L[0], new Vector3(2000, 2000, 0), Quaternion.identity);
            obj6.transform.SetParent(this.transform);
            obj6.name = "SKU_10L";
            SKU_10L.Add(obj6);

        

            var obj8 = Instantiate(SKU_20L[0], new Vector3(2000, 2000, 0), Quaternion.identity);
            obj8.transform.SetParent(this.transform);
            obj8.name = "SKU_20L";
            SKU_20L.Add(obj8);

        }


      
    }

}

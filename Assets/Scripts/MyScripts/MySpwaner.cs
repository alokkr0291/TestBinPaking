using System;
using System.Collections;
using System.Collections.Generic;
using MobackPacker;
using UnityEngine;

public class MySpwaner : MonoBehaviour
{ 
    readonly Vector3 size_1l = new Vector3(2.5f,1.43f,3.6f);
    readonly Vector3 size_4l = new Vector3(3.8f,2.2f,3.8f);
    readonly Vector3 size_10l = new Vector3(2.72f,1.54f,2.72f);
    readonly Vector3 size_20l = new Vector3(3.26f,1.9f,3.26f);
    [SerializeField] private Vector3 truckDim;

    [SerializeField] private GameObject prefab_1L;
    [SerializeField] private GameObject prefab_4L;
    [SerializeField] private GameObject prefab_10L;
    [SerializeField] private GameObject prefab_20L;

    [SerializeField]private int noOf_1L;
    [SerializeField]private int noOf4L;
    [SerializeField]private int noOf_10L;
    [SerializeField]private int noOf_20L;

    [SerializeField] private int maxLayerValue =0;

  
    //TODO remove storing in different array
    private List<GameObject> inst_1L = new List<GameObject>();
    private List<GameObject> inst_4L = new List<GameObject>();
    private List<GameObject> inst_10L = new List<GameObject>();
    private List<GameObject> inst_20L = new List<GameObject>();

    private Dictionary<int, List<GameObject>> cuboidsByLayer = new Dictionary<int, List<GameObject>>();
  

    void Start()
    {
        Init();
    }


    void Init()
    {
    
        int index1L=0,  index4L=0, index10L=0, index20L =0;
        //BinPackResult packedboxes = binPackWebBoxes(jsonBoxes, jsonDeserializeRoom(room));
        string[][] mySKUS = GetBoxes();
        string[] myTRuck = GetRoom();
        Debug.Log($"*****skusLEngth --> {mySKUS.Length}");
        BinPackResult packedboxes = binPackBoxes(mySKUS, myTRuck);
        for (int i = 0; i < packedboxes.BestResult[0].Count; i++)
        {
            Cuboid box = packedboxes.BestResult[0][i];
  
            if ( box.Tag.Equals ("SKU_01L") )
            {
        
                var obj = inst_1L[index1L];
                AssignInfo(obj, box,"SKU_01L");
                index1L++;
            }
            if ( box.Tag.Equals ("SKU_04L") )
            {
                var obj = inst_4L[index4L];
                AssignInfo(obj, box,"SKU_04L");
                index4L++;
        
            }
      
            if ( box.Tag.Equals ("SKU_10L") )
            {
                var obj = inst_10L[index10L];
                AssignInfo(obj, box,"SKU_10L");
                index10L++;
            }
      
            if ( box.Tag.Equals ("SKU_20L") )
            {
                var obj = inst_20L[index20L];
                AssignInfo(obj, box,"SKU_20L");
                index20L++;
            }
      
        }

       // StartCoroutine(ShowContentLayerwise());

    }

    private  void AssignInfo(GameObject obj, Cuboid box, string tag)
    {
        obj.transform.position = new Vector3((float) (box.X), (float) box.Y, (float) box.Z);
        obj.transform.localScale = new Vector3((float) (box.Width), (float) box.Height, (float) box.Depth);
        var script = obj.GetComponent<SKU_Script>();
        script.Dimension = new Vector3((float) (box.Width), (float) box.Height, (float) box.Depth);
        script.Position = new Vector3((float) (box.X), (float) box.Y, (float) box.Z);
    
        script.RotationDir = box.RotationDir;
        script.IsPlaced = true;
        script.Tag = tag;
        script.Layer = box.Layer;

        if ( box.Layer > maxLayerValue)
            maxLayerValue = box.Layer;
    
        int layer = box.Layer;

        // Check if the layer exists in the dictionary
        if (!cuboidsByLayer.ContainsKey(layer))
        {
            // If not, create a new list for that layer
            cuboidsByLayer[layer] = new List<GameObject>();
        }
        cuboidsByLayer[layer].Add(gameObject);
    
        obj.SetActive(true);
    }

    private IEnumerator ShowContentLayerwise()
    {
        yield return new WaitForSeconds(1);
        for (int i = maxLayerValue; i >= 1; i--)
        {
            Debug.Log($"lFistayer" + i);
            yield return new WaitForSeconds(1);
            if (cuboidsByLayer.ContainsKey(i))
            {
                List<GameObject> cuboidsOnLayer = cuboidsByLayer[i];
                Debug.Log($"layer" + i);
                foreach (GameObject cuboid in cuboidsOnLayer)
                {
                    if (cuboid != null)
                    {
                        cuboid.SetActive(false);
                    }
                }
            }

        } 
    
    
    }

    string[] GetRoom() 
    {
        string[] roomarray = new string[3];        
        roomarray[0] = truckDim.x.ToString();
        roomarray[1] = truckDim.y.ToString();
        roomarray[2] = truckDim.z.ToString();        
        return roomarray;

    }


    BinPackResult binPackBoxes(string[][] boxes, string[] room) 
    {
        Debug.Log($"binPackWebBoxes --> {boxes.Length}");
        List<Cuboid> cubes = new List<Cuboid>();
        foreach(string[] box in boxes) 
        {
            //  Debug.Log($" -->{box[0]}- {box[1]} -{box[2]}- {box[3]}");
            Cuboid cube = new Cuboid(Convert.ToDecimal(box[1]), Convert.ToDecimal(box[2]), Convert.ToDecimal(box[3]), 0, box[0]);
            cubes.Add(cube);
        }
        var binPacker = BinPacker.GetDefault(BinPackerVerifyOption.BestOnly);
        BinPackParameter parameter = new BinPackParameter(Convert.ToDecimal(room[0]), Convert.ToDecimal(room[1]), Convert.ToDecimal(room[2]), cubes);
        return binPacker.Pack(parameter);               
    }
  
    string[][] GetBoxes()
    {
        int length = noOf_1L+ noOf4L + noOf_10L + noOf_20L;
        string[][] boxarray = new string[length][];
        int addindex = 0;
    
        for (int i = 0; i < noOf_20L; i++)
        {
            GameObject obj = Instantiate(prefab_20L, Vector3.one*1000, Quaternion.identity);
            inst_20L.Add(obj);
      
            string[] temp = new string[7];
            temp[0] = "SKU_20L";
            temp[1] = size_20l[0].ToString();
            temp[2] = size_20l[1].ToString();
            temp[3] = size_20l[2].ToString();
            boxarray[addindex] = temp;
            addindex++;
        }
        
        for (int i = 0; i < noOf_10L; i++)
        {
            GameObject obj = Instantiate(prefab_10L, Vector3.one*1000, Quaternion.identity);
            inst_10L.Add(obj);
            string[] temp = new string[7];
            temp[0] = "SKU_10L";
            temp[1] = size_10l[0].ToString();
            temp[2] = size_10l[1].ToString();
            temp[3] = size_10l[2].ToString();
            boxarray[addindex] = temp;
            addindex++;
        }
    
        for (int i = 0; i < noOf4L; i++)
        {
            GameObject obj = Instantiate(prefab_4L, Vector3.one*1000, Quaternion.identity);
            inst_4L.Add(obj);
            string[] temp = new string[7];
            temp[0] = "SKU_04L";
            temp[1] = size_4l[0].ToString();
            temp[2] = size_4l[1].ToString();
            temp[3] = size_4l[2].ToString();
            boxarray[addindex] = temp;
            addindex++;
        }

        for (int i = 0; i < noOf_1L; i++)
        {
            GameObject obj = Instantiate(prefab_1L, Vector3.one*1000, Quaternion.identity);
            inst_1L.Add(obj);
            string[] temp = new string[7];
            temp[0] = "SKU_01L";
            temp[1] = size_1l[0].ToString();
            temp[2] = size_1l[1].ToString();
            temp[3] = size_1l[2].ToString();
            boxarray[addindex] = temp;
            addindex++;
        }
   

    

   
        return boxarray;
    }


}
using UnityEngine;
using MobackPacker;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using Newtonsoft.Json;

public class SpawnerScript : MonoBehaviour
{
    //The BoxGen prefab is what will be initialized and instantiated
    public Transform prefab;
    //Holds the array of integers that gives the dimensions of each box
    public int[][] dimensions;
    //Holds the starting positions of each box after they were generated with the 3DBinPacking
    public int[][] coordinates;
    static string WRITE_FILE_PATH = "Assets/Scripts/text_files/write.txt";
    public  TextAsset mockjson;

 
    /// Deserializes json into list of cubes from file.
    /// </summary>
    /// <param name="path"> path of input file</param>
    /// <returns></returns>
    private static List<Cuboid> getCuboidsFromFile(string path)
    {
        List<Cuboid> cubes = new List<Cuboid>();
        string line;
        try
        {
            using (StreamReader streamReader = new StreamReader(path))
            {
                while ((line = streamReader.ReadLine()) != null)
                {
                    cubes = JsonConvert.DeserializeObject<List<Cuboid>>(line);
                }
            }
        }
        catch (IOException e)
        {
            //Console.Write(e.ToString());
            Debug.Log($" --> {e.ToString()}");
        }

        return cubes;
    }

    void Start()
    {
     
        generateWebBoxes(mockjson.text);
       // generateBoxes(dimensions, coordinates);
    }
    

    BinPackResult binPackWebBoxes(string[][] boxes, string[] room) 
    {
        List<Cuboid> cubes = new List<Cuboid>();
        foreach(string[] box in boxes) 
        {
            Cuboid cube = new Cuboid(Convert.ToDecimal(box[1]), Convert.ToDecimal(box[2]), Convert.ToDecimal(box[3]), 0, box[0]);
            cubes.Add(cube);
        }
        var binPacker = BinPacker.GetDefault(BinPackerVerifyOption.BestOnly);
        BinPackParameter parameter = new BinPackParameter(Convert.ToDecimal(room[0]), Convert.ToDecimal(room[1]), Convert.ToDecimal(room[2]), cubes);
        return binPacker.Pack(parameter);               
    }


    /// Deserialize json into box array
    /// </summary>
    /// <param name="boxesjson"></param>
    /// <returns></returns>
    string[][] jsonDeserializeBox(BoxListJSON boxesjson) 
    {
        string[][] boxarray = new string[boxesjson.boxes.Length][];
        for(int i = 0; i < boxesjson.boxes.Length; i++) {
            string[] temp = new string[7];
            temp[0] = boxesjson.boxes[i].id;
            temp[1] = boxesjson.boxes[i].x;
            temp[2] = boxesjson.boxes[i].y;
            temp[3] = boxesjson.boxes[i].z;
            temp[4] = boxesjson.boxes[i].r;
            temp[5] = boxesjson.boxes[i].g;
            temp[6] = boxesjson.boxes[i].b;
            boxarray[i] = temp;           
         
        }
        return boxarray;
    }


    /// Deserializes room in json into roomarray
    /// </summary>
    /// <param name="room">Room json</param>
    /// <returns></returns>
    string[] jsonDeserializeRoom(RoomJSON room) {
        string[] roomarray = new string[3];        
            roomarray[0] = room.x;
            roomarray[1] = room.y;
            roomarray[2] = room.z;        
        return roomarray;

    }


    /// Preliminary Method for Working With Database, generates the webboxes from json, and displays them in unity.
    /// </summary>
    /// <param name="json"></param>
    void generateWebBoxes(string json) {
        BoxListJSON boxes = JsonUtility.FromJson<BoxListJSON>(json);
        RoomJSON room = boxes.room;
        string[][] jsonBoxes = jsonDeserializeBox(boxes);
        
        BinPackResult packedboxes = binPackWebBoxes(jsonBoxes, jsonDeserializeRoom(room));
        for(int i = 0; i < packedboxes.BestResult[0].Count; i++)
        {
            Cuboid box = packedboxes.BestResult[0][i];
            var newobj = Instantiate(prefab, new Vector3((float)box.Width, (float)box.Height, (float)box.Depth), Quaternion.identity);
            newobj.name = box.Tag + "-" + i;
            //MeshRenderer creates the meshes needed to visualize each box
            MeshRenderer meshrend = newobj.GetComponent<MeshRenderer>();
            float red = 0;
            float green = 0;
            float blue = 0;
            // Compares ID to fetch box colors from original box list
            for(int j = 0; j < jsonBoxes.Length; j++)
            {
               // Debug.Log($"{jsonBoxes[i][0]} --> {box.Tag}");
               // if((string)box.Tag == jsonBoxes[i][0]) 
                {
                    red = float.Parse(jsonBoxes[i][4]);
                    
                    green = float.Parse(jsonBoxes[i][5]);
                    
                    blue = float.Parse(jsonBoxes[i][6]);
                    
                }
            }
           // Debug.Log("Color " + red+" " + green+" " + blue + " ");
         
            //Assigns a random color for each box to allow differentiation
            meshrend.material.color = new Color(red/255, green/255, blue/255);

            //Sets the label for each box
            float newx = ((float)box.X + (float)(box.Width / 2));
            float newy = ((float)box.Y + (float)(box.Height / 2));
            float newz = ((float)box.Z + (float)(box.Depth / 2));

            //Runs the prefab script so the box is generated
            int[] dimensions = new int[3] {(int)box.Width, (int)box.Height, (int)box.Depth};
            int[] coordinates = new int[3] {(int)box.X, (int)box.Y, (int)box.Z};
            var newobjscript = newobj.GetComponent<BoxGenerator>();
            newobjscript.testdimensions = dimensions;
            newobjscript.testcoordinates = coordinates;
            newobjscript.webboxid = box.Tag.ToString();
        }
    }
 
    void generateBoxes(int[][] dimensions, int[][] coordinates)
    {
        if(dimensions.Length == coordinates.Length)
        {
            for(int i = 0; i < dimensions.Length; i++) 
            {
                var newobj = Instantiate(prefab, new Vector3(coordinates[i][0], coordinates[i][1], coordinates[i][2]), Quaternion.identity);

                //MeshRenderer creates the meshes needed to visualize each box
                MeshRenderer meshrend = newobj.GetComponent<MeshRenderer>();
                //Assigns a random color for each box to allow differentiation
                meshrend.material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

                //Sets the label for each box
                float newx = (coordinates[i][0] / 1f+ (dimensions[i][0] / 2f));
                float newy = (coordinates[i][1] / 1f + (dimensions[i][1] / 2f));
                float newz = coordinates[i][2] / 1f + 0.5f;
                //newobj.Find("Canvas").localPosition = transform.InverseTransformPoint(new Vector3(newx, newy, newz));
                
                //Sets the text for the box's label
                //TMPro.TextMeshProUGUI labeltext = newobj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                //labeltext.text = "Box #" + i.ToString();
                
                //Provides a collider so that boxes do not clip into each other
                //boxcoll.size = transform.TransformVector(new Vector3(dimensions[i][0], dimensions[i][1], dimensions[i][2]));
                //boxcoll.center = transform.InverseTransformPoint(new Vector3(coordinates[i][0], coordinates[i][1] + (dimensions[i][2] / 2f), coordinates[i][2]));
                

                //Runs the prefab script so the box is generated
                var newobjscript = newobj.GetComponent<BoxGenerator>();
                newobjscript.testdimensions = dimensions[i];
                newobjscript.testcoordinates = coordinates[i];
                newobjscript.boxid = i;
            }
        }
    }


}

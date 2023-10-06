using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MobackPacker;
using UnityEngine;

public class MyTest1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Call();
    }

    
    List<Cuboid> cuboids = new List<Cuboid>
    {
        new Cuboid { Width = 2, Height = 3, Depth = 4 },
        new Cuboid { Width = 1, Height = 2, Depth = 3 },
        new Cuboid { Width = 1, Height = 1, Depth = 6 },
        new Cuboid { Width = 5, Height = 1, Depth = 5 },
        // Add more cuboids as needed
    };


    void Call()
    {
        int shuffleCount = 3;

        var permutations = BinPacker.GetCuboidsPermutations(cuboids, shuffleCount);

        Debug.Log($" permutation count --> {permutations.Count()}");
        foreach (var permutation in permutations)
        {
            
            foreach (var cuboid in permutation)
            {
                
                Debug.Log($"Width: {cuboid.Width}, Height: {cuboid.Height}, Depth: {cuboid.Depth}");
            }

            Debug.Log("----"); // Separator between permutations
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}

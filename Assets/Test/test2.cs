using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class test2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float originalValue = 18.76f;
        int roundedValue = (int)Math.Ceiling(originalValue);
        Debug.Log(roundedValue); 
    }

    
    
  
}


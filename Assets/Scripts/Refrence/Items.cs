    
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    //public string itemName  = String.Empty;
    //This is for the STRAP and PLY as APaint gave this seperately
    public Vector3 LBH = new Vector3();
    public Vector3 startPos = Vector3.zero;
    public Vector3 endPos = Vector3.zero;
    public string layerNo;
    public int ROBO_ID;
    public string  SKU_ID;
    public int itemNo;// For the ref with Xcel Sheet
	public string UniqOp_ID;
    public string Alingment;

    // this is for the SKU lenght breath Height as this was seperated by Apaint
    public Vector3 sku_LBH;


    public delegate void ItemInformationDel(Items item);
    public static ItemInformationDel ItemInfoEvt;

    public void SetLenghtBreathHeight()
    {

    }

    public void SetData(Vector3 star)
    {
    }

    //public void OnVis
    void OnBecameInvisible()
    {
       // this.gameObject.SetActive(false);
    }

}

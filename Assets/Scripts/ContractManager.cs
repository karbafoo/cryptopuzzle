using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
public class ContractManager : MonoBehaviour
{
    public string Url = "https://mainnet.infura.io";
    public string UrlFull = "https://mainnet.infura.io/v3/7238211010344719ad14a89db874158c";
    // Start is called before the first frame update
    void Start()
    {
        string fex = "farad";
        byte[] fexBytes = Encoding.UTF8.GetBytes (fex);
        Debug.Log(BitConverter.ToString(fexBytes).Replace("-",""));
        //TODO LOOK AT THE ETHER JS PROJECT HOW TO CALL DIRECTLY hexing function name + params
    }

}

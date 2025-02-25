using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GetIPs : MonoBehaviour
{
    public TMP_InputField ipID1;
    public TMP_InputField ipID2;
    public TMP_InputField ipID3;
    public static string ip1;
    public static string ip2;
    public static string ip3;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ip1 = ipID1.text;
        ip2 = ipID2.text;
        ip3 = ipID3.text;
    }
}

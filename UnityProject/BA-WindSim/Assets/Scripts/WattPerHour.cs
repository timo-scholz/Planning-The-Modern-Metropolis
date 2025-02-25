using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WattPerHour : MonoBehaviour
{
    public TextMeshProUGUI textContent;
    private float current_kwh;
    public bool isTUI = false;
    public WindReceiverGUI[] GUIRot = new WindReceiverGUI[3];
    public ConnectToESP[] TUIRot = new ConnectToESP[3];

    // Start is called before the first frame update
    void Start()
    {
        current_kwh = 0f;
        if(isTUI){
            for (int i = 0; i < 3; i++){
                current_kwh += Mathf.Abs(TUIRot[i].currentRotation);
            }
        }else{
            for (int i = 0; i < 3; i++){
                current_kwh += Mathf.Abs(GUIRot[i].currentRot);
            }
        }
        
    }

    void UpdateCurrent_kwh(){
        current_kwh = 0f;
        if(isTUI){
            for (int i = 0; i < 3; i++){
                current_kwh += Mathf.Abs(TUIRot[i].currentRotation);
            }
        }else{
            for (int i = 0; i < 3; i++){
                current_kwh += Mathf.Abs(GUIRot[i].currentRot);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    UpdateCurrent_kwh();
       textContent.text = ((current_kwh * 500f).ToString("F2") + " kWh");
    }
}

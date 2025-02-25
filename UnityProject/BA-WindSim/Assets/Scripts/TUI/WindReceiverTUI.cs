using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Net.Sockets;
using System.Text;
using System;

public class WindReceiverTUI : MonoBehaviour
{
    //TODO: Pos auf Sim abfragen
    private float speed;
    private Vector2 receivedWind;
    private float currentRotation;
    private float similarity;



    void Start()
    {
        
    }

    public void SetSpeed(float pSpeedX, float pSpeedY){
        receivedWind = new Vector2 (pSpeedX, pSpeedY);
        speed = Mathf.Abs(pSpeedX) + Mathf.Abs(pSpeedY);
    }

    private Vector2 GetForward(){
        //Debug.Log("up: " + this.transform.up);
        return this.transform.up;
    }

    float map(float s, float a1, float a2, float b1, float b2)
	{
    	return b1 + (s-a1)*(b2-b1)/(a2-a1);
	}

    private float CalculateDirectionSimilarity(Vector2 v1, Vector2 v2)
    {
        //Normalisieren der Vektoren, um nur die Richtung zu vergleichen
        v1 = Vector3.Normalize(v1);
        v2 = Vector3.Normalize(v2);

        //Skalarprodukt der beiden Vektoren
        float dotProduct = Vector3.Dot(v1, v2);

        return dotProduct;
    }

    

    public float UpdateRotationSpeed(){
        if((similarity > 0.15f || similarity < -0.15f) && speed > 0.3f){
                currentRotation = speed * similarity;
        }else
        {
            currentRotation = 0f;
        }
        //Debug.Log("New RotationSpeed to send: " + currentRotation +" \n");
        //SetCurrentRotation(currentRotation);
        return currentRotation;
    }

    

    

    void FixedUpdate()
    {
        //Debug.Log("Wind:    " + receivedWind);
        //Debug.Log("Forward: " + GetForward());
        similarity = CalculateDirectionSimilarity(receivedWind, GetForward());
        //similarity = 1f;
        //Debug.Log("Die Vektoren zeigen zu " + (similarity * 100) + "% in die gleiche Richtung.");
        //UpdateRotationSpeed(speed, similarity);  
        //Debug.Log(": " + speed + " " + similarity);
    }
}

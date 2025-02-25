using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System;

public class WindReceiverGUI : MonoBehaviour
{
    public GameObject wings;
    private float speed;
    private Vector2 receivedWind;
    public float currentRot;

    public void SetSpeed(float pSpeedX, float pSpeedY){
        receivedWind = new Vector2 (pSpeedX, pSpeedY);
        speed = Mathf.Abs(pSpeedX) + Mathf.Abs(pSpeedY);
    }

    private Vector2 GetForward(){
        Vector3 forwardVector = new Vector3 (this.transform.forward.x, 0, this.transform.forward.z);
        return new Vector2 (forwardVector.normalized.x, forwardVector.normalized.z);
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

    void FixedUpdate()
    {
        //Debug.Log("Wind:    " + receivedWind);
        //Debug.Log("Forward: " + GetForward());
        float similarity = CalculateDirectionSimilarity(receivedWind, GetForward());
        //Debug.Log("Die Vektoren zeigen zu " + (similarity * 100) + "% in die gleiche Richtung.");

        if((similarity > 0.15f || similarity < -0.15f) && speed > 0.3f){
                currentRot = speed * similarity;
                wings.transform.Rotate(0f, 0f, currentRot * 10f, Space.Self);
        }else{
            currentRot = 0f;
        }
    }
}

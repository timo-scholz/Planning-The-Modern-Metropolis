using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VeloArrows : MonoBehaviour
{
    public GameObject arrowPrefab; //Pfeil Prefrab
    public int gridWidth = 20; //Breite des Gitters
    public int gridHeight = 20; //Höhe des Gitters
    public float spacing = 1.0f; //Abstand zwischen den Punkten
    private int hOffset = 0;
    private int wOffset = 0;

    public GameObject[] arrowInstances; //2D-Array zum Speichern der Instanzen

    void Start()
    {
        if(gridHeight % 2 != 0){
            wOffset = 1;
        }else{
            wOffset = 0;
        }

        if(gridWidth % 2 != 0){
            hOffset = 1;
        }else{
            hOffset = 0;
        }

        //Initialisiere das Array basierend auf der Gittergröße
        arrowInstances = new GameObject[gridWidth * gridHeight];

        int xIndex = 0; //Index für das Array
        int yIndex = 0;



        //Schleife über die Breite und Höhe des Gitters
        for (int x = 0 - gridWidth/2; x < gridWidth/2 + wOffset; x++)
        {
            yIndex = 0; //Zurücksetzen des y-Index für jede neue x-Zeile
            for (int y = 0 - gridHeight/2; y < gridHeight/2 + hOffset; y++)
            {
                //Berechne die Position für das aktuelle Gitterelement
                Vector3 position = new Vector3(x * spacing, this.transform.position.y, y * spacing);
                
                //Instanziere das Prefab an der berechneten Position und setze das aktuelle GameObject als Parent
                GameObject arrowInstance = Instantiate(arrowPrefab, position, Quaternion.identity);
                
                //Setze das aktuelle GameObject als Parent
                arrowInstance.transform.parent = this.transform;

                //Speichere die Instanz im Array
                arrowInstances[(xIndex * gridHeight) + yIndex] = arrowInstance;

                yIndex++;
            }
            xIndex ++;
        }

        for(int i = 0; i < gridWidth * gridHeight; i++)
        {
                //Debug.Log("VeloArrow " + i + " : " + arrowInstances[i].transform.position);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System;

public class WindReadback : MonoBehaviour
{
    public bool isTUI;
    public Fluid fluid;
    public ComputeShader computeShader;
    [SerializeField] private RenderTexture inputTextureRW;

    public Vector2[] positions;

    public GameObject[] gameObjects = new GameObject[3];  //Referenzen auf 3 GameObjects

    public GameObject[] esp32 = new GameObject[3];

    private ComputeBuffer positionsBuffer;
    private ComputeBuffer outputBuffer;
    public Color[] outputColors;

    private GameObject[] arrows;
    public VeloArrows arrowGrid;


    void Start()
    {
        positions = new Vector2[gameObjects.Length + (arrowGrid.gridWidth * arrowGrid.gridHeight)];
        if (inputTextureRW == null || computeShader == null)
        {
            Debug.LogError("RenderTexture oder ComputeShader nicht gesetzt!");
            return;
        }

        //Initialisiere den ComputeBuffer für Positionen und gebe die Positionen hinein
        positionsBuffer = new ComputeBuffer(positions.Length + (arrowGrid.gridWidth * arrowGrid.gridHeight), sizeof(float) * 2);
        positionsBuffer.SetData(positions);

        outputColors = new Color[positions.Length + (arrowGrid.gridWidth * arrowGrid.gridHeight)];
        outputBuffer = new ComputeBuffer(positions.Length + (arrowGrid.gridWidth * arrowGrid.gridHeight), sizeof(float) * 4);

        computeShader.SetTexture(0, "inputTexture", inputTextureRW);
        computeShader.SetBuffer(0, "pPositions", positionsBuffer);
        computeShader.SetBuffer(0, "outputBuffer", outputBuffer);

        
    }

    public void FillWindMillArray(GameObject _windmill, int tangibleId){
        gameObjects[tangibleId] = _windmill;
        Debug.Log(_windmill + " mit ID: " + tangibleId);
    }

    void FixedUpdate(){
        StartCoroutine(ReadBack());
    }

    private IEnumerator ReadBack()
    {
        
        //Aktualisiere das positions-Array mit den Positionen der GameObjects
        for (int i = 0; i < gameObjects.Length; i++)
        {
            if (gameObjects[i] != null)
            {
                Vector3 position = gameObjects[i].transform.position;
                
                
                if(isTUI){
                    Debug.Log("Pos: " + fluid.mapTUIPostion(position.x, position.y));
                    positions[i] = fluid.mapTUIPostion(position.x, position.y);  //x und y Position verwenden
                    //positions[i] = new Vector2(position.x, position.y);  //x und y Position verwenden
                }else{
                    positions[i] = fluid.mapGUIPostion(position.x, position.z);  //x und z Position verwenden
                }
            }
        }

        

        arrows = arrowGrid.arrowInstances;

        for (int i = gameObjects.Length; i < gameObjects.Length + arrows.Length; i++)
        {
            //Debug.Log("Index: " + i);
            if (arrows[i - gameObjects.Length] != null)
            {
                Vector3 position = arrows[i - gameObjects.Length].transform.position;

                //Debug.Log(positions[i]);
                
                positions[i] = fluid.mapGUIPostion(position.x, position.z);  //x und z Position verwenden
            }
        }

        positionsBuffer.SetData(positions);

        int threadGroups = Mathf.CeilToInt(positions.Length / 64.0f);
        computeShader.Dispatch(0, threadGroups, 1, 1);

        AsyncGPUReadbackRequest request = AsyncGPUReadback.Request(outputBuffer);

        //Wait for the request to complete
        while (!request.done)
        {
            yield return null;
        }

        if (request.hasError)
        {
            Debug.Log("GPU readback error detected.");
        }
        else
        {

        outputBuffer.GetData(outputColors);

        for (int i = 0; i < gameObjects.Length; i++)
        {
            //Debug.Log($"Color at position {positions[i]}: {outputColors[i]}");
            
            if(!isTUI){
                gameObjects[i].GetComponent<WindReceiverGUI>().SetSpeed(outputColors[i].r, outputColors[i].g);
            }else{
                if(gameObjects[i] != null){
                    gameObjects[i].GetComponent<WindReceiverTUI>().SetSpeed(outputColors[i].r, outputColors[i].g);
                    esp32[i].GetComponent<ConnectToESP>().SetCurrentRotation(gameObjects[i].GetComponent<WindReceiverTUI>().UpdateRotationSpeed());
                    //Debug.Log("rot: " + gameObjects[i].GetComponent<WindReceiverTUI>().UpdateRotationSpeed());
                }
                
            }
        }

        for (int i = gameObjects.Length; i < arrows.Length + gameObjects.Length; i++)
        {
            Vector2 direction = new Vector2 (outputColors[i].r, outputColors[i].g);
            if (direction != Vector2.zero)
            {
                //Berechne den Winkel zwischen dem Richtungsvektor und der positiven X-Achse (Vector2.right)
                float angle = Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg;

                //Setze die Rotation des Sprites (nur auf der Z-Achse)
                Quaternion rotation = Quaternion.Euler(new Vector3(90, 0, angle));
                arrows[i - gameObjects.Length].transform.GetChild(0).rotation = rotation;
            }

        }
        }
    }

    void OnDestroy()
    {
        positionsBuffer.Release();
        outputBuffer.Release();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System;

public class WindVisualizer : MonoBehaviour
{
    public ComputeShader computeShader;
    public RenderTexture sourceTexture;
    public RenderTexture targetTexture;

    private int kernelHandle;

    void Start()
    {
        kernelHandle = computeShader.FindKernel("CopyTexture");
    }

    void Update()
    {
        //Compute Shader mit den RWTextures konfigurieren
        
        computeShader.SetTexture(kernelHandle, "Source", sourceTexture);
        computeShader.SetTexture(kernelHandle, "Result", targetTexture);

        //Compute Shader ausführen
        computeShader.Dispatch(kernelHandle, sourceTexture.width / 8, sourceTexture.height/ 8, 1);
    }
}

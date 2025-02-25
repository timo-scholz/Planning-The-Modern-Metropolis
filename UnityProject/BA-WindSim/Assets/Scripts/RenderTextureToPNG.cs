using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class RenderTextureToPNG : MonoBehaviour
{
    public RenderTexture renderTexture;

    //Der Dateipfad, wohin das Bild gespeichert wird
    public string filePath = "Assets/RenderTextureImage.png";

    void Start()
    {
        //Exportiere die RenderTexture als PNG, wenn das Spiel startet
        ExportRenderTextureToPNG();
    }

    public void ExportRenderTextureToPNG()
    {
        //Mache die RenderTexture zum aktiven Renderziel
        RenderTexture.active = renderTexture;

        //Erstelle ein neues Texture2D, das die Daten der RenderTexture speichern wird
        Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

        //Kopiere die RenderTexture-Daten in das Texture2D
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        //Wandelt das Texture2D in PNG-Byte-Daten um
        byte[] pngData = texture2D.EncodeToPNG();

        //Speicher die PNG-Daten als Datei
        if (pngData != null)
        {
            File.WriteAllBytes(filePath, pngData);
            Debug.Log("Bild erfolgreich gespeichert: " + filePath);
        }

        //RenderTexture nicht mehr als aktives Ziel setzen
        RenderTexture.active = null;

        //Optional: Zerstören des Texture2D-Objekts, um Speicher freizugeben
        Destroy(texture2D);
    }
}

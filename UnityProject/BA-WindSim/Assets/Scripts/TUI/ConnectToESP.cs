using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Net.Sockets;
using System.Text;
using System;

public class ConnectToESP : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    public string serverIP = "192.168.178.71"; // Replace with your ESP32 IP address
    private const int serverPort = 12345;
    public float currentRotation = 0;

    public WindReadback windreadback;

    public int WindMillID = 0;

    // Start is called before the first frame update
    void Start()
    {
        switch (WindMillID)
        {
        case 3:
            serverIP = GetIPs.ip3;
            break;
        case 2:
            serverIP = GetIPs.ip2;
            break;
        case 1:
            serverIP = GetIPs.ip1;
            break;
        default:
            Debug.Log("Error no IP ID given");
            break;
        }
        ConnectToServer();
    }

    void ConnectToServer()
    {
        try
        {
            client = new TcpClient(serverIP, serverPort);
            stream = client.GetStream();
            Debug.Log("Connected to server");
        }
        catch (SocketException e)
        {
            Debug.Log("SocketException: " + e.ToString());
        }
    }

    public void SetCurrentRotation(float newRot){
        currentRotation = newRot;
    }

    void SendData()
    {
        if (client != null && client.Connected)
        {
            byte[] data = Encoding.UTF8.GetBytes(currentRotation.ToString("F2") + "\n");
            stream.Write(data, 0, data.Length);
            Debug.Log("Sent Rotation Speed: " + currentRotation);
            
        }
        else
        {
            Debug.Log("Not connected to server");
        }
    }

    void OnApplicationQuit()
    {
         if (stream != null)
             stream.Close();
         if (client != null)
             client.Close();
    }

    // Update is called once per frame
    void Update()
    {
        if(windreadback.gameObjects[WindMillID - 1] != null){
            SendData();
            Debug.Log("ich sende" + currentRotation + " an: " + WindMillID);
        }else{
            SendData();
            currentRotation = 0f;
            Debug.Log("ich sende" + currentRotation + " an: " + WindMillID);
        }
    }
}

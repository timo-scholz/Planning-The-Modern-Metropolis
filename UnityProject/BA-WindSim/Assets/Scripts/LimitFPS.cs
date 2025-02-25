using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitFPS : MonoBehaviour
{
    public int fps = 30;
    void Start()
    {
        Application.targetFrameRate = fps;
    }
}

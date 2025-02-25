using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInputToVarible : MonoBehaviour
{
    public TMP_InputField inputFliedX;
    public TMP_InputField inputFliedY;
    private float numberX;
    private float numberY;

    public Button confirmButton;

    public Fluid fluid;

    void Start()
    {
        confirmButton.onClick.AddListener(TaskOnClick);
    }

    public void TaskOnClick()
    {
        storeNumbers();
        fluid.setglobalWindDir(numberX, numberY);
    }

    void storeNumbers()
    {
        string inputTextX = inputFliedX.text;
        string inputTextY = inputFliedY.text;

        try
        {
            numberX = float.Parse(inputFliedX.text);
            numberY = float.Parse(inputFliedY.text);
        }
        catch (System.FormatException)
        {
            Debug.LogError("Input was not a valid float.");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target; //Object um das rotiert wird
    public float distance = 5.0f; //Abstand zum Object
    public float orbitSpeed = 100.0f; //Rotationsgeschwindigkeit
    public KeyCode orbitKey = KeyCode.Space; //Taste zum Rotieren
    public float scrollSensitivity = 2.0f; //Scrollgeschwindigkeit für Zoom
    public float minDistance = 10.0f; //Minimum Abstand zum Zielobjekt
    public float maxDistance = 20.0f; //Maximum Abstand zum Zielobjekt

    private float currentX = 0.0f;
    private float currentY = 0.0f;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("No target assigned for CameraOrbit script.");
            return;
        }

        //Startposition
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;

        UpdateCameraPosition();
    }

    void Update()
    {
        if (Input.GetKey(orbitKey))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            currentX += mouseX * orbitSpeed * Time.deltaTime;
            currentY -= mouseY * orbitSpeed * Time.deltaTime;

            //Clamp Y-Achse damit die Kamera sich nicht überschlägt
            currentY = Mathf.Clamp(currentY, 5, 80);

            UpdateCameraPosition();
        }

        //Zooming mit Mausrad
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            distance -= scroll * scrollSensitivity;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            UpdateCameraPosition();
        }
    }

    void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 direction = new Vector3(0, 0, -distance);
        transform.position = target.position + rotation * direction;
        transform.LookAt(target);
    }
}

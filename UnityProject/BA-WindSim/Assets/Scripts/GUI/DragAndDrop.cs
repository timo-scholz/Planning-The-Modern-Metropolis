using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = false;
    private Plane dragPlane;
    private Vector3 offset;
    private bool isRotating = false;
    public float rotationSpeed = 100f;

    void Start()
    {
        mainCamera = Camera.main;
        dragPlane = new Plane(Vector3.up, Vector3.zero); //Die Ebene, auf der das Objekt "haften" soll
    }

    void Update()
    {
        //Überprüfen, ob eine Maustaste gedrückt wird
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            HandleMouseDown();
        }

        //Überprüfen, ob die Maustaste losgelassen wird
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isRotating = false;
        }

        //Rotation aufrufen
        if (isRotating)
        {
            RotateObject();
        }
    }

    void HandleMouseDown()
    {
        //Raycast Kamera -> Mausposition
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        //Überprüfen ob Objekt mit Script getroffen wurde
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform == transform)
            {
                if (Input.GetMouseButtonDown(0))//Linke Maustaste
                {
                    StartDragging(hit.point);
                }

                if (Input.GetMouseButtonDown(1))//Rechte Maustaste
                {
                    isRotating = true;
                }
            }
        }
    }

    void StartDragging(Vector3 hitPoint)
    {
        //Start Drag
        isDragging = true;
        offset = transform.position - hitPoint;
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            //Raycast Kamera -> Mausposition
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            //Wenn Raycast Ebene trifft, bewege Objekt
            if (dragPlane.Raycast(ray, out float enter))
            {
                Vector3 targetPosition = ray.GetPoint(enter) + offset;

                float objectHeight = 0f;

                //Setzen der Position des Objekts auf der Höhe der Ebene
                targetPosition.y = dragPlane.distance + objectHeight;

                transform.position = targetPosition;
            }
        }
    }

    void RotateObject()
    {
        //Rotation um die Y-Achse basierend auf der horizontalen Mausbewegung
        float rotation = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, rotation, Space.World);
    }
}

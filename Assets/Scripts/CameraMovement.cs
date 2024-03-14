using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//(mostly) GPT4 Generated
public class CameraMovement : MonoBehaviour
{
    public float zoomSpeed = 2f; // Speed of zooming in/out
    public float minSize = 5f; // Minimum orthographic size
    public float maxSize = 20f; // Maximum orthographic size

    private Camera cam; // Reference to the camera component
    private Vector3 dragOrigin; // The point where the camera dragging started

    void Start()
    {
        cam = GetComponent<Camera>(); // Get the Camera component attached to this GameObject
    }

    void Update()
    {
        // Zoom in and out with the scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cam.orthographicSize -= scroll * zoomSpeed * 1000 * cam.orthographicSize;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minSize, maxSize);

        // Initiate camera movement
        if (Input.GetMouseButtonDown(1)) // Right mouse button pressed
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        // During the movement
        if (Input.GetMouseButton(1)) // Right mouse button held down
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            cam.transform.position += difference;
        }
    }
}
